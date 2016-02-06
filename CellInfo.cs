using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;

namespace ConEx
{
    /*[StructLayout(LayoutKind.Explicit)]
    public struct CharUnion
    {
        [FieldOffset(0)]
        public char UnicodeChar;
        [FieldOffset(0)]
        public byte AsciiChar;
    }*/
    using WORD = UInt16;

    [StructLayout(LayoutKind.Explicit, CharSet=CharSet.Unicode)]
    public struct CharInfo
    {
        [FieldOffset(0)]
        public char UnicodeChar;
        [FieldOffset(0)]
        public char AsciiChar;

        [FieldOffset(2)]
        public short Attributes;
    }

    //A wrapper around all the P/Ivoke uglyness of CharInfo and such
    public class CellInfo
    {
        private CharInfo _cellCharInfo;
        public CharInfo CellCharInfo { get { return _cellCharInfo; } set { _cellCharInfo = value; } }

        public CellInfo(char character, ConsoleColor fore = ConsoleColor.White, ConsoleColor back = ConsoleColor.Black)
        {
            Color foreground_color = ConsoleColorToColorAttribute(fore,false);
            Color background_color = ConsoleColorToColorAttribute(back,true);

            _cellCharInfo.AsciiChar = character;
            _cellCharInfo.UnicodeChar = character;
            _cellCharInfo.Attributes = (short)((int)foreground_color | (int)background_color);
        }

        //Thanks Roslyn! http://referencesource.microsoft.com/#mscorlib/microsoft/win32/win32native.cs
        [Flags]
        public enum Color : short
        {
            Black = 0,
            ForegroundBlue = 0x1,
            ForegroundGreen = 0x2,
            ForegroundRed = 0x4,
            ForegroundYellow = 0x6,
            ForegroundIntensity = 0x8,
            BackgroundBlue = 0x10,
            BackgroundGreen = 0x20,
            BackgroundRed = 0x40,
            BackgroundYellow = 0x60,
            BackgroundIntensity = 0x80,

            ForegroundMask = 0xf,
            BackgroundMask = 0xf0,
            ColorMask = 0xff
        }

        
        [System.Security.SecurityCritical]  // auto-generated
        private static Color ConsoleColorToColorAttribute(ConsoleColor color, bool isBackground)
        {
            Color c = (Color)color;

            // Make these background colors instead of foreground
            if (isBackground)
                c = (Color)((int)c << 4);
            return c;
        }

        [System.Security.SecurityCritical]  // auto-generated
        private static ConsoleColor ColorAttributeToConsoleColor(Color c)
        {
            // Turn background colors into foreground colors.
            if ((c & Color.BackgroundMask) != 0)
                c = (Color)(((int)c) >> 4);

            return (ConsoleColor)c;
        }
    }
}
