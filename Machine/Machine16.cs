using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    public class Machine16
    {
        public Memory Data { get; private set; }
        public Memory Code { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public Screen Screen{get; private set;}
        public CPU16 CPU { get; private set; }

        public Machine16(bool bUseKeyboard, bool bUseScreen)
        {
            Clock.Clear();
            Data = new Memory(16, 16, "Data");
            Code = new Memory(16, 16, "Code");
            CPU = new CPU16();

            if (bUseKeyboard)
                Keyboard = new Keyboard(Data, 0x6000);
            if (bUseScreen)
            {
                Screen = new Screen(Data, 0x4000, 40, 80);
            }

            Code.Address.ConnectInput(CPU.InstructionAddress);
            CPU.Instruction.ConnectInput(Code.Output);

            Data.Input.ConnectInput(CPU.MemoryOutput);
            CPU.MemoryInput.ConnectInput(Data.Output);
            Data.Load.ConnectInput(CPU.MemoryWrite);
            Data.Address.ConnectInput(CPU.MemoryAddress);
        }

        public void Reset()
        {
            CPU.Reset.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            CPU.Reset.Value = 0; 
        }
    }
}
