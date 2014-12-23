using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;

namespace ConEx
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            
            //IntPtr hWnd = GetConsoleWindow();
            //ShowScrollBar(hWnd, 1, true);
            ConEx_Draw.Init(80, 25);
            ConEx_Input.Init();

            Console.WriteLine("asdf");
            Console.WriteLine("asdf");
            Console.WriteLine("asdf");
            Console.WriteLine("asdf");
            Console.WriteLine("asdf");
            Console.WriteLine("asdf");
            Console.WriteLine("asdf");
            Console.Clear();

            //string input = Console.ReadLine();

            ConEx_Draw.InsertCharacter((char)9500,0,0);
                ConEx_Draw.InsertCharacter((char)9508,0,1);
                    ConEx_Draw.InsertCharacter((char)9516,0,2);
                    ConEx_Draw.InsertCharacter((char)9524, 0, 3);
                    ConEx_Draw.DrawScreen();
            ConEx_Draw.InsertCharacter('a', 9, 9);
            
            ConEx_Draw.InsertCharacter('A', 0, 15);
            ConEx_Draw.InsertCharacter('n', 4, 1);

            ConEx_Draw.InsertCharacter('k', 3, 7,ConsoleColor.Gray,ConsoleColor.DarkGray);
            ConEx_Draw.InsertCharacter('#', 10, 40,ConsoleColor.Green,ConsoleColor.Black);
            ConEx_Draw.InsertCharacter((char)150, 14, 59,ConsoleColor.Cyan,ConsoleColor.DarkYellow);
            ConEx_Draw.InsertCharacter((char)200, 24, 79,ConsoleColor.Red);
            ConEx_Draw.DrawScreen();

            
            while (true)
            {
                Point p = ConEx_Mouse.GetMousePosition();
                if (Control.MouseButtons.HasFlag(MouseButtons.Left) == true)
                {
                    
                    //Move cursor there
                    Console.SetCursorPosition(p.X,p.Y);
                    Console.Write("0");
                }

                if (Control.MouseButtons.HasFlag(MouseButtons.Right) == true)
                {
                    ConsoleKeyInfo k = Console.ReadKey(true);
                    if (k.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    else
                    {
                        Console.Clear();
                    }
                }
            }
        }
    }
}