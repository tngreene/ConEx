using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace ConEx
{
    //Contains code relating to the Console Window as in, a window within the Windows Operating System
    public class ConEx_Window
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// Gets the window handle for the console.
        /// </summary>
        /// <returns>A hwnd or IntPtr.Zero if there is no console window.</returns>
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        public static bool IsActive()
        {
            IntPtr foregroundWnd = GetForegroundWindow();
            if (GetConsoleWindow() == foregroundWnd)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
