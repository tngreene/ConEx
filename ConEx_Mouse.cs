using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace ConEx
{
    public class ConEx_Mouse
    {
        /// <summary>
        /// Gets the window handle for the console.
        /// </summary>
        /// <returns>A hwnd or IntPtr.Zero if there is no console window.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        const int C_BorderWidth = 10;
        const int C_HeaderHeight = 35;
        const int CELL_WIDTH = 10;
        const int CELL_HEIGHT = 20;

        public static Point GetMousePosition()
        {
            Point p = new Point();
            
            int x = System.Windows.Forms.Cursor.Position.X;
            int y = System.Windows.Forms.Cursor.Position.Y;
            
            
            IntPtr hWnd = GetConsoleWindow();
            ShowScrollBar(hWnd, 1, true);
            RECT r;

            GetWindowRect(hWnd, out r);
            //Console.WriteLine("X:{0},Y:{1},W:{2},H:{3}",r.Left,r.Top,r.Right-r.Left,r.Top-r.Bottom);

            int row = 0;
            int column = 0;

            //TEST IF Cursor is inside of black part
            if (x >= r.Left + C_BorderWidth && x <= r.Right - C_BorderWidth)
            {
                //Console.WriteLine("Inside X");
                if (y >= r.Top + C_HeaderHeight && y <= r.Bottom - C_BorderWidth)
                {
                    //Figure out where you are inside the console
                    int localX = x - (r.Left + C_BorderWidth);
                    int localY = y - (r.Top + C_HeaderHeight);
                    //Console.Write(localX + ", " + localY +"| ");

                    int localXCopy = localX;
                    int localYCopy = localY;

                    while (true)
                    {
                        localYCopy -= CELL_HEIGHT;
                        if (localYCopy > 0)
                        {
                            row++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    while (true)
                    {
                        localXCopy -= CELL_WIDTH;
                        if (localXCopy > 0)
                        {
                            column++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            p.X = column;
            p.Y = row;
            return p;
        }
        //bool  IsMouseButtonDown(MouseButton button)

    }
}
