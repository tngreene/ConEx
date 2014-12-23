using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConEx
{
    public class ConEx_Input
    {
        private static System.Threading.Timer _inputTimer;
        //private static System.Threading.TimerCallback _callback;
        
        public static void Init()
        {
            //_callback = InputLoop;
            _inputTimer = new Timer(InputLoop, null, 0, 20);
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

        private static void GetKeys()
        {

        }
        
        private static void InputLoop(Object state)
        {
           //Console.Write("Here");
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

        public static string ReadLine()
        {
            string outline = "";
            return outline;
            /*do
            {
                bool enterPressed = false;
                char input = Console.ReadKey(true);

                //if(
            }
            //while(*/
        }
    }
}
