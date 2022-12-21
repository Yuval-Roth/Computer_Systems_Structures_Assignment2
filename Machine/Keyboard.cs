using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleComponents;


namespace Machine
{
    public class Keyboard : MemoryMappedIO
    {

        public int PressedKeyCode { get; private set; }
        public int AltPressed { get; private set; }
        public int CtrlPressed { get; private set; }
        public int ShiftPressed { get; private set; }


        public Keyboard(Memory data, int iOffset)
        {
            
            PressedKeyCode = -1;

            m_iOffset = iOffset;
            data.RegisterMemoryMappedIO(this, m_iOffset, 1, out m_aiMemoryMap);
        }

        //may require two clock ticks before info is delivered
        public override void OnClockUp()
        {
            
        }

        public override void OnClockDown()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo cki = Console.ReadKey();
                PressedKeyCode = ((int)cki.Key) % 128;

                //special keys
                if (cki.Key == ConsoleKey.Enter)
                    PressedKeyCode = 128;
                if (cki.Key == ConsoleKey.Backspace)
                    PressedKeyCode = 129;
                if (cki.Key == ConsoleKey.LeftArrow)
                    PressedKeyCode = 130;
                if (cki.Key == ConsoleKey.UpArrow)
                    PressedKeyCode = 131;
                if (cki.Key == ConsoleKey.RightArrow)
                    PressedKeyCode = 132;
                if (cki.Key == ConsoleKey.DownArrow)
                    PressedKeyCode = 133;
                if (cki.Key == ConsoleKey.Home)
                    PressedKeyCode = 134;
                if (cki.Key == ConsoleKey.End)
                    PressedKeyCode = 135;
                if (cki.Key == ConsoleKey.PageUp)
                    PressedKeyCode = 136;
                if (cki.Key == ConsoleKey.PageDown)
                    PressedKeyCode = 137;
                if (cki.Key == ConsoleKey.Insert)
                    PressedKeyCode = 138;
                if (cki.Key == ConsoleKey.Delete)
                    PressedKeyCode = 139;
                if (cki.Key == ConsoleKey.Escape)
                    PressedKeyCode = 140;

                if (cki.Key == ConsoleKey.F1)
                    PressedKeyCode = 141;
                if (cki.Key == ConsoleKey.F2)
                    PressedKeyCode = 142;
                if (cki.Key == ConsoleKey.F3)
                    PressedKeyCode = 143;
                if (cki.Key == ConsoleKey.F4)
                    PressedKeyCode = 144;
                if (cki.Key == ConsoleKey.F5)
                    PressedKeyCode = 145;
                if (cki.Key == ConsoleKey.F6)
                    PressedKeyCode = 146;
                if (cki.Key == ConsoleKey.F7)
                    PressedKeyCode = 147;
                if (cki.Key == ConsoleKey.F8)
                    PressedKeyCode = 148;
                if (cki.Key == ConsoleKey.F9)
                    PressedKeyCode = 149;
                if (cki.Key == ConsoleKey.F10)
                    PressedKeyCode = 150;
                if (cki.Key == ConsoleKey.F11)
                    PressedKeyCode = 151;
                if (cki.Key == ConsoleKey.F12)
                    PressedKeyCode = 152;


                AltPressed = (int)(cki.Modifiers & ConsoleModifiers.Alt);
                CtrlPressed = (int)(cki.Modifiers & ConsoleModifiers.Control);
                ShiftPressed = (int)(cki.Modifiers & ConsoleModifiers.Shift);
                
                int iValue = PressedKeyCode;
                
                if (AltPressed != 0)
                    iValue += 0x100;
                if (CtrlPressed != 0)
                    iValue += 0x200;
                if (ShiftPressed != 0)
                    iValue += 0x400;
                m_aiMemoryMap[m_iOffset] = iValue;
            }
            else
            {
                PressedKeyCode = -1;
            }
            
        }

        public override bool TestGate()
        {
            throw new NotImplementedException();
        }
    }
}
