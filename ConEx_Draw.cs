using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;


/* What is this?
 * The Console Drawing API hopes to accomplish helping developers who wish to use the console
 * as more than just a read out for text. A project like displaying a game or making a more complicated interface.
 * Using color coded text. Plus, their console must application must run fast! SetCursor is so slow and using Console.Write
 * exclusively without management means that the whole system shuts down when a new line is accidentally inserted wrong.
 * 
 * Color management is aweful too, forgeting not to reset the colors means that you could end up with black text on a black background! 
 * Goodness forbid you want to resize the window!
 * 
 * This project aims to take care of all of that for you. This manager does all the drawing for you, 
 * all the color arangment, all the spacing and everything else needed.
 * 
 * You specify attributes and where you want to insert text and for what color, and thats it.
 * 
 * Before you change any Console Setting in your application, check here if it should be set for you!
 * 
 */
namespace ConEx
{
    public class ConEx_Draw
    {
        
        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;

      
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(
            int nStdHandle
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet=CharSet.Unicode)]
        static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        struct CurrentColor
        {
            ConsoleColor fore;
            ConsoleColor back;
        }

        public struct WindowDimensions
        {
            public int width;
            public int height;

            WindowDimensions(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
            
            public bool Contains(int row, int column)
            {
                if( row < 0 || column < 0 )
                {
                    return false;
                }
                if (row >= height || column >= width)
                {
                    return false;
                }
                return true;
            }
        }

        private static CurrentColor _currentColor;
        private static WindowDimensions _dimensions;
        public static WindowDimensions Dimensions { get { return _dimensions; } set { _dimensions = value; } }
        
        private static CharInfo[][] buffer;
        


        public static void Init(int width, int height)
        {
            _dimensions.width = width;
            _dimensions.height = height;
            Console.WindowWidth = width;
            Console.BufferWidth = width;

            Console.WindowHeight = height;
            Console.BufferHeight = height;
            
            buffer = new CharInfo[Console.BufferHeight][];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new CharInfo[Console.BufferWidth];
            }
        }

        /// <summary>
        /// Inserts a character into the drawing buffer and (potetially draws)
        /// </summary>
        /// <param name="c">The character to insert</param>
        /// <param name="row">The row to insert into</param>
        /// <param name="column">The column to insert into</param>
        /// <param name="foreground">The foreground color, default white</param>
        /// <param name="background">The background color, default black</param>
        public static void InsertCharacter(char c, int row, int column, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            if (row >= buffer.Length || column >= buffer[0].Length)
            {
                return;
            }
            else
            {
                buffer[row][column] = new CellInfo(c, foreground, background).CellCharInfo;
            }
        }

        /// <summary>
        /// Inserts a string into the drawing buffer, wrapping it around if it needs and can and (potetially draws)
        /// </summary>
        /// <param name="s">The string to insert</param>
        /// <param name="row">The row to insert into</param>
        /// <param name="column">The column to insert into</param>
        /// <param name="wrapAround">Whether or not to wrap around the screen buffer TODO</param>
        /// <param name="foreground">The foreground color, default white</param>
        /// <param name="background">The background color, default black</param>
        /// <param name="delayDrawing">Useful if you are inserting a large number of characters at once and want to avoid the flicker</param>
        public static void InsertString(string s, int row, int column, bool wrapAround, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            for (int i = 0; i < s.Length; i++)
            {
                ConEx_Draw.InsertCharacter(s[i], row, column + i, foreground, background);
            }
        }

        /// <summary>
        /// Fills an area of the screen buffer with a particular character with a particular color, delays drawing
        /// </summary>
        /// <param name="c">The characer to fill with</param>
        /// <param name="row">Start row of the area to fill</param>
        /// <param name="column">Start column of the area to fill</param>
        /// <param name="width">The width of the area to fill</param>
        /// <param name="height">The height of the area to fill</param>
        /// <param name="foreground">The foreground color, default white</param>
        /// <param name="background">The background color, default black</param>
        public static void FillArea(char c, int row, int column, int width, int height, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            for (int i = column; i < width; i++)
            {
                for (int j = row; j < height; j++)
                {
                    InsertCharacter(c, j, i, foreground, background);
                }
            }
        }

        /// <summary>
        /// Fills the screen buffer with a particular character with a particular color, delays drawing
        /// </summary>
        /// <param name="c">The characer to fill with</param>
        /// <param name="foreground">The foreground color, default white</param>
        /// <param name="background">The background color, default black</param>
        public static void FillScreen(char c, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            FillArea(c, 0, 0, _dimensions.width, _dimensions.height, foreground, background);
        }

        public static void SetAttributes(int row, int column, ConsoleColor foreground, ConsoleColor background)
        {
            if (Dimensions.Contains(row, column) == true)
            {
                char c = buffer[row][column].AsciiChar;

                buffer[row][column] = new CellInfo(c, foreground, background).CellCharInfo;
            }
        }

        public static void DrawScreen()
        {
            IntPtr h = GetStdHandle(STD_OUTPUT_HANDLE);

            if (h != IntPtr.Zero)
            {
                CharInfo[] singleBuf = new CharInfo[Console.BufferHeight * Console.BufferWidth];

                int counter = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    for (int j = 0; j < buffer[i].Length; j++)
                    {
                        singleBuf[counter] = buffer[i][j];
                        counter++;
                    }
                }
                SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = (short)ConEx_Draw.Dimensions.width, Bottom = (short)ConEx_Draw.Dimensions.height};

                bool b = WriteConsoleOutput(h, singleBuf,
                          new Coord() { X = (short)Dimensions.width, Y = (short)Dimensions.height },
                          new Coord() { X = 0, Y = 0 },
                          ref rect);
            }
        }
        //-----------
    }
}

/*Hanlded
 * CursorLeft
 * CursorSize
CursorTop
 * CursorVisible
 * LargestWindowWidth
LargestWindowHeight
 * ForegroundColor
 * 
 * WindowHeight
WindowLeft
WindowTop
WindowWidth
  */ 

    /*Should be handled?
 /*Not handled
    CapsLock
     * 
NumberLock
     * 
    */

    /*
Error

In
InputEncoding
IsErrorRedirecte
IsInputRedirecte
IsOutputRedirect
KeyAvailable

Out
OutputEncoding
Title
TreatControlCAsInput*/
    //}
