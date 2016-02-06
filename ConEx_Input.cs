using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ConEx
{
    public static class ConEx_Input
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern short GetKeyState(VK_Code nVirtKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetAsyncKeyState(VK_Code vkey);

        //private static extern bool GetAysncKeyboardState(byte[] lpKeys);
        //private static Dictionary<ConsoleKeyInfo,bool> _keys;
        //public static Dictionary<ConsoleKeyInfo, bool> Keys { get { return _keys; } }

        public static bool ShiftDown { get { return IsKeyPressed(VK_Code.VK_SHIFT); } }
        public static bool AltDown { get { return IsKeyPressed(VK_Code.VK_MENU); } }
        public static bool CtrlDown { get { return IsKeyPressed(VK_Code.VK_CONTROL); } }
        
        public static void Init(int delay)
        {

        }

        private static bool _treatControlCAsInput = false;
        
        public static bool TreatControlCAsInput
        {
            get
            {
                return _treatControlCAsInput;
            }
            set
            {
                if (value == true)
                {
                     Console.CancelKeyPress += CancelCtrlC;
                    _treatControlCAsInput = true;
                }
                else
                {
                    Console.CancelKeyPress -= CancelCtrlC;
                    _treatControlCAsInput = false;
                }
            }
        }
        
        private static void CancelCtrlC (object sender, ConsoleCancelEventArgs e) 
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Ask the keyboard if a key is currently down
        /// </summary>
        /// <param name="testKey">The VK_Code to test</param>
        /// <returns>True if it is down, false it it is not or possibly toggled</returns>
        public static bool IsKeyPressed(VK_Code testKey)
        {
            bool keyPressed = false;
            short result = (short)GetKeyState(testKey);
            
            switch( result )
            {
                case 0:
                // Not pressed and not toggled on.
                keyPressed = false;
                break;
                case 1:
                // Not pressed, but toggled on
                keyPressed = false;
                break;
                default:
                // Pressed (and may be toggled on)
                keyPressed = true;
                break;
            }
            return keyPressed;
        }

        /// <summary>
        /// Get which keys are currently pressed down and return them
        /// </summary>
        public static ConsoleKeyInfo[] GetInput()
        {
            // A list of characters
            List<ConsoleKeyInfo> input = new List<ConsoleKeyInfo>();

            // Loop while keys are available or we hit 10 keys
            for (int i = 0; Console.KeyAvailable && i < 10; i++)
            {
                // Read a key (preventing it from being printed) 
                // and put it in the key list (if it's not in there yet)
                ConsoleKeyInfo info = Console.ReadKey(true);
                if (!input.Contains(info))
                {
                    input.Add(info);
                }
            }

            // Use up any remaining key presses
            while (Console.KeyAvailable)
            {
                // Read a single key
                Console.ReadKey(true);
            }

            // Convert the list to an array and return
            return input.ToArray();
        }
        
        //TODO: Maybe we should not leave the user in an endless loop of trying to get matching input...
        #region WaitForXMatch methods
        

        /// <summary>
        /// Waits for a "yes" or "no"
        /// </summary>
        /// <param name="yes_word">The word representing yes, by default "yes"</param>
        /// <param name="no_word">The word representing no, by default "no"</param>
        /// <param name="error_msg">The displayed error if there is bad input</param>
        /// <returns>True for yes, false for no</returns>
        public static bool WaitForBooleanChoice(string yes_word = "yes",
                                                string no_word = "no",
                                                string error_msg = "Expected yes or no",
                                                bool case_sensitive = false)
        {
            while (true)
            {
                string result = WaitForRegExMatch("(" + yes_word + "|" + no_word + ")", error_msg, case_sensitive);

                if (result == yes_word)
                {
                    return true;
                }
                else if (result == no_word)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Waits for input from the user based on range of allowed values
        /// Good for situations like getting a number between x and y
        /// </summary>
        /// <param name="lower_bound">The lowest character accepted (by value)</param>
        /// <param name="upper_bound">The highest character accepted (by value)</param>
        /// <param name="error_msg">The displayed error if there is bad input</param>
        /// <returns>The chosen input or null if there was a bounds mismatch </returns>
        public static char WaitForCharInRange(char lower_bound, char upper_bound, string error_msg = "Value out of bounds")
        {
            return (char)WaitForIntInRange((int)lower_bound,(int)upper_bound, true, error_msg);
        }

        /// <summary>
        /// Waits for the user to input an integer between bounds
        /// </summary>
        /// <param name="lower_bound">The lowest allowed number (inclusive)</param>
        /// <param name="upper_bound">The heighest allowed number (inclusive)</param>
        /// <param name="single_key">
        /// True if the caller wants ReadKey's behavior instead of Readline.
        /// Note: The 
        /// </param>
        /// <param name="error_msg">The displayed error if there is bad input</param>
        /// <returns>The inputed integer or null if there was a bounds mismatch</returns>
        public static int WaitForIntInRange(int lower_bound, int upper_bound, bool single_key, string error_msg = "Value out of bounds")
        {
            //If our range is impossible
            if (upper_bound < lower_bound)
            {
                //Return our error code
                throw new Exception("Upperbound is less than lower bound");
            }

            if (single_key == true)
            {
                char input = '\0';
                int output;
                //Get their input
                do
                {
                    if (Int32.TryParse(Console.ReadKey().KeyChar.ToString(), out output))
                    {
                        if (output < lower_bound || output > upper_bound)
                        {
                            Console.CursorLeft = 0;
                            Console.WriteLine(error_msg);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.CursorLeft = 0;
                        Console.WriteLine(error_msg);
                    }
                }
                while (true); 
                
                return output;
            }
            else
            {
                while(true)
                {
                    try
                    {
                        return Convert.ToInt32(WaitForRegExMatch(@"^-?[0-9]+$", error_msg));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Waits for a string matching a passed in regular expression
        /// </summary>
        /// <param name="regex_string">The string to match</param>
        /// <param name="error_msg">The displayed error if there is bad input</param>
        /// <param name="case_sensitive">If the regular expression is cases sensitive, default is false</param>
        /// <returns>The inputed string or null if invalid</returns>
        public static string WaitForRegExMatch(string regex_string, string error_msg, bool case_sensitive = false)
        {
            if (regex_string == null || error_msg == null)
            {
                return null;
            }

            while (regex_string != null)
            {
                string input = Console.ReadLine();
                if (Regex.IsMatch(input,
                                  regex_string,
                                  case_sensitive ? RegexOptions.IgnoreCase : RegexOptions.None))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine(error_msg);
                    continue;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// VK_Codes for P/Invoke calls
        /// </summary>
        public enum VK_Code : int
        {
            VK_BACKSPACE = 8,
            VK_TAB = 9,
            VK_CLEAR = 12,
            VK_ENTER = 13,
            VK_SHIFT = 16,
            VK_CONTROL = 17,
            VK_MENU = 18,
            VK_PAUSE = 19,
            VK_CAPITAL = 20,
            VK_ESCAPE = 27,
            VK_SPACEBAR = 32,
            VK_PAGEUP = 33,
            VK_PAGEDOWN = 34,
            VK_END = 35,
            VK_HOME = 36,
            VK_LEFT = 37,
            VK_UP = 38,
            VK_RIGHT = 39,
            VK_DOWN = 40,
            VK_SELECT = 41,
            VK_PRINT = 42,
            VK_EXECUTE = 43,
            VK_PRINTSCREEN = 44,
            VK_INSERT = 45,
            VK_DELETE = 46,
            VK_HELP = 47,
            VK_D0 = 48,
            VK_D1 = 49,
            VK_D2 = 50,
            VK_D3 = 51,
            VK_D4 = 52,
            VK_D5 = 53,
            VK_D6 = 54,
            VK_D7 = 55,
            VK_D8 = 56,
            VK_D9 = 57,
            VK_A = 65,
            VK_B = 66,
            VK_C = 67,
            VK_D = 68,
            VK_E = 69,
            VK_F = 70,
            VK_G = 71,
            VK_H = 72,
            VK_I = 73,
            VK_J = 74,
            VK_K = 75,
            VK_L = 76,
            VK_M = 77,
            VK_N = 78,
            VK_O = 79,
            VK_P = 80,
            VK_Q = 81,
            VK_R = 82,
            VK_S = 83,
            VK_T = 84,
            VK_U = 85,
            VK_V = 86,
            VK_W = 87,
            VK_X = 88,
            VK_Y = 89,
            VK_Z = 90,
            VK_LWIN = 91,
            VK_RWIN = 92,
            VK_APPLICATIONS = 93,
            VK_SLEEP = 95,
            VK_NUMPAD0 = 96,
            VK_NUMPAD1 = 97,
            VK_NUMPAD2 = 98,
            VK_NUMPAD3 = 99,
            VK_NUMPAD4 = 100,
            VK_NUMPAD5 = 101,
            VK_NUMPAD6 = 102,
            VK_NUMPAD7 = 103,
            VK_NUMPAD8 = 104,
            VK_NUMPAD9 = 105,
            VK_MULTIPLY = 106,
            VK_ADD = 107,
            VK_SEPARATOR = 108,
            VK_SUBTRACT = 109,
            VK_DECIMAL = 110,
            VK_DIVIDE = 111,
            VK_F1 = 112,
            VK_F2 = 113,
            VK_F3 = 114,
            VK_F4 = 115,
            VK_F5 = 116,
            VK_F6 = 117,
            VK_F7 = 118,
            VK_F8 = 119,
            VK_F9 = 120,
            VK_F10 = 121,
            VK_F11 = 122,
            VK_F12 = 123,
            VK_F13 = 124,
            VK_F14 = 125,
            VK_F15 = 126,
            VK_F16 = 127,
            VK_F17 = 128,
            VK_F18 = 129,
            VK_F19 = 130,
            VK_F20 = 131,
            VK_F21 = 132,
            VK_F22 = 133,
            VK_F23 = 134,
            VK_F24 = 135,
            VK_LSHIFT = 160,
            VK_RSHIFT = 161,
            VK_LCONTROL = 162,
            VK_RCONTROL = 163,
            //Alt
            VK_LMENU = 164,
            VK_RMENU = 165,
            VK_BROWSERBACK = 166,
            VK_BROWSERFORWARD = 167,
            VK_BROWSERREFRESH = 168,
            VK_BROWSERSTOP = 169,
            VK_BROWSERSEARCH = 170,
            VK_BROWSERFAVORITES = 171,
            VK_BROWSERHOME = 172,
            VK_VOLUMEMUTE = 173,
            VK_VOLUMEDOWN = 174,
            VK_VOLUMEUP = 175,
            VK_MEDIANEXT = 176,
            VK_MEDIAPREVIOUS = 177,
            VK_MEDIASTOP = 178,
            VK_MEDIAPLAY = 179,
            VK_LAUNCHMAIL = 180,
            VK_LAUNCHMEDIASELECT = 181,
            VK_LAUNCHAPP1 = 182,
            VK_LAUNCHAPP2 = 183,
            VK_OEM1 = 186,
            VK_OEMPLUS = 187,
            VK_OEMCOMMA = 188,
            VK_OEMMINUS = 189,
            VK_OEMPERIOD = 190,
            VK_OEM2 = 191,
            VK_OEM3 = 192,
            VK_OEM4 = 219,
            VK_OEM5 = 220,
            VK_OEM6 = 221,
            VK_OEM7 = 222,
            VK_OEM8 = 223,
            VK_OEM102 = 226,
            VK_PROCESS = 229,
            VK_PACKET = 231,
            VK_ATTENTION = 246,
            VK_CRSEL = 247,
            VK_EXSEL = 248,
            VK_ERASEENDOFFILE = 249,
            VK_PLAY = 250,
            VK_ZOOM = 251,
            VK_NONAME = 252,
            VK_PA1 = 253,
            VK_OEMCLEAR = 254,
        }
    }
}
