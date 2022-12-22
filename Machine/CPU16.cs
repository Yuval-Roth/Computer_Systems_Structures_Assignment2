using SimpleComponents;
using System;

namespace Machine
{
    public class CPU16
    {
        public const int 
            J0 =  0,
            J1 =  1,
            J2 =  2,
            D0 =  3,
            D1 =  4,
            D2 =  5,
            C0 =  6,
            C1 =  7,
            C2 =  8,
            C3 =  9,
            C4 = 10,
            A  = 11,
            X0 = 12,
            X1 = 13,
            X2 = 14,
            Type = 15;

        public int Size { get; private set; }

        public WireSet Instruction { get; private set; }
        public WireSet MemoryInput { get; private set; }
        public Wire Reset { get; private set; }

        public WireSet MemoryOutput { get; private set; }
        public Wire MemoryWrite { get; private set; }
        public WireSet MemoryAddress { get; private set; }
        public WireSet InstructionAddress { get; private set; }

        private ALU m_gALU;
        private Counter m_rPC;
        private MultiBitRegister m_rA, m_rD;
        private BitwiseMux m_gAMux, m_gMAMux;


        public CPU16()
        {
            Size = 16;

            Instruction = new WireSet(Size);
            MemoryInput = new WireSet(Size);
            MemoryOutput = new WireSet(Size);
            MemoryAddress = new WireSet(Size);
            InstructionAddress = new WireSet(Size);
            MemoryWrite = new Wire();
            Reset = new Wire();

            m_gALU = new ALU(Size);
            m_rPC = new Counter(Size);
            m_rA = new MultiBitRegister(Size);
            m_rD = new MultiBitRegister(Size);

            m_gAMux = new BitwiseMux(Size);
            m_gMAMux = new BitwiseMux(Size);

            m_gAMux.ConnectInput1(Instruction);
            m_gAMux.ConnectInput2(m_gALU.Output);

            m_rA.ConnectInput(m_gAMux.Output);

            m_gMAMux.ConnectInput1(m_rA.Output);
            m_gMAMux.ConnectInput2(MemoryInput);
            m_gALU.InputY.ConnectInput(m_gMAMux.Output);

            m_gALU.InputX.ConnectInput(m_rD.Output);

            m_rD.ConnectInput(m_gALU.Output);

            MemoryOutput.ConnectInput(m_gALU.Output);
            MemoryAddress.ConnectInput(m_rA.Output);

            InstructionAddress.ConnectInput(m_rPC.Output);
            m_rPC.ConnectInput(m_rA.Output);
            m_rPC.ConnectReset(Reset);

            ConnectControls();
        }

        //Add gates for control implementation here

        BitwiseMux A_Input_Mux;
        BitwiseMux A_Or_M_Mux;
        WireSet ALU_Control;
        AndGate D_Load_AndGate;
        OrGate A_Load_OrGate;
        AndGate A_Load_AndGate;
        NotGate A_Load_NotGate;
        AndGate MemoryWrite_AndGate;

        private void ConnectControls()
        {
            //1. connect control of mux 1 (selects entrance to register A)
            A_Input_Mux = new BitwiseMux(Size);
            A_Input_Mux.ConnectInput1(m_gALU.Output);
            A_Input_Mux.ConnectInput2(Instruction);

            //2. connect control to mux 2 (selects A or M entrance to the ALU)
            A_Or_M_Mux = new BitwiseMux(Size);
            A_Or_M_Mux.ConnectInput1(MemoryOutput);
            A_Or_M_Mux.ConnectInput2(A_Input_Mux.Output);

            //3. consider all instruction bits only if C type instruction (MSB of instruction is 1)
            A_Input_Mux.Control.ConnectInput(Instruction[Type]);
            A_Or_M_Mux.Control.ConnectInput(Instruction[A]);

            //4. connect ALU control bits
            ALU_Control = new WireSet(5);
            ALU_Control[0].ConnectInput(Instruction[C0]);
            ALU_Control[1].ConnectInput(Instruction[C1]);
            ALU_Control[2].ConnectInput(Instruction[C2]);
            ALU_Control[3].ConnectInput(Instruction[C3]);
            ALU_Control[4].ConnectInput(Instruction[C4]);
            m_gALU.Control.ConnectInput(ALU_Control);

            //5. connect control to register D (very simple)
            D_Load_AndGate = new AndGate();
            D_Load_AndGate.ConnectInput1(Instruction[D1]);
            D_Load_AndGate.ConnectInput2(Instruction[Type]);
            m_rD.Load.ConnectInput(D_Load_AndGate.Output);

            //6. connect control to register A (a bit more complicated)
            A_Load_OrGate = new OrGate();
            A_Load_AndGate = new AndGate();
            A_Load_NotGate = new NotGate();
            A_Load_NotGate.ConnectInput(Instruction[Type]);
            A_Load_AndGate.ConnectInput1(Instruction[Type]);
            A_Load_AndGate.ConnectInput2(Instruction[D2]); 
            A_Load_OrGate.ConnectInput2(A_Load_NotGate.Output);
            A_Load_OrGate.ConnectInput1(A_Load_AndGate.Output);
            m_rA.Load.ConnectInput(A_Load_OrGate.Output);

            //7. connect control to MemoryWrite
            MemoryWrite_AndGate = new AndGate();
            MemoryWrite_AndGate.ConnectInput1(Instruction[D0]);
            MemoryWrite_AndGate.ConnectInput2(Instruction[Type]);
            MemoryWrite.ConnectInput(MemoryWrite_AndGate.Output);

            //8. create inputs for jump mux

            //9. connect jump mux (this is the most complicated part)

            //10. connect PC load control
        }


        public override string ToString()
        {
            return "A=" + m_rA + ", D=" + m_rD + ", PC=" + m_rPC + ",Ins=" + Instruction;
        }

        private string GetInstructionString()
        {
            if (Instruction[Type].Value == 0)
                return "@" + Instruction.GetValue();
            return Instruction[Type].Value + "XXX " +
               "a" + Instruction[A] + " " +
               "c" + Instruction[C4] + Instruction[C3] + Instruction[C2] + Instruction[C1] + Instruction[C0] + " " +
               "d" + Instruction[D2] + Instruction[D1] + Instruction[D0] + " " +
               "j" + Instruction[J2] + Instruction[J1] + Instruction[J0];
        }

        public void PrintState()
        {
            Console.WriteLine("CPU state:");
            Console.WriteLine("PC=" + m_rPC + "=" + m_rPC.Output.GetValue());
            Console.WriteLine("A=" + m_rA + "=" + m_rA.Output.GetValue());
            Console.WriteLine("D=" + m_rD + "=" + m_rD.Output.GetValue());
            Console.WriteLine("Ins=" + GetInstructionString());
            Console.WriteLine("ALU=" + m_gALU);
            Console.WriteLine("inM=" + MemoryInput);
            Console.WriteLine("outM=" + MemoryOutput);
            Console.WriteLine("addM=" + MemoryAddress);
        }
    }
}
