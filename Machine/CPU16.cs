using SimpleComponents;
using System;
using System.Runtime.InteropServices;

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

        private AndGate Mux2_Control;
        private WireSet ALU_Control;
        private AndGate D_Load_AndGate;
        private OrGate A_Load_OrGate;
        private AndGate A_Load_AndGate;
        private NotGate A_Load_NotGate;
        private AndGate MemoryWrite_AndGate;
        private NotGate JGT;
        private OrGate JGT_Or;
        private Wire JEQ;
        //private OrGate JLT_Or;
        //private NotGate JLT;
        private Wire JLT;
        private OrGate JGE;
        private OrGate JLE;
        private NotGate JNE;
        private Wire JMP;

        #region MultiwayMux class

        /// <summary>
        /// © Yuval Roth
        /// </summary>
        class MultiwayMux
        {
            private MuxGate[] gates;
            private readonly int ControlBits;
            private Wire[] Inputs;
            private Wire[] Controls;

            public Wire Output { get; private set; }


            /// <param name="ControlBits">The amount of control bits</param>
            public MultiwayMux(int ControlBits)
            {
                this.ControlBits = ControlBits;
                gates = new MuxGate[(int)Math.Pow(2, ControlBits) - 1];
                Inputs = new Wire[(int)Math.Pow(2, ControlBits)];
                for(int i = 0; i< Inputs.Length; i++)
                {
                    Inputs[i] = new Wire();
                }
                Controls = new Wire[ControlBits];
                for (int i = 0; i < Controls.Length; i++) 
                {
                    Controls[i] = new Wire();
                }
                Output = new Wire();
                BuildHeap(0);
                ConnectWires();
                ConnectControls();
            }

            /// <summary>
            /// first wire is at index 0 <br/>
            /// last wire is at index 2^(ControlBits) - 1
            /// </summary>
            /// <param name="i">Index</param>
            /// <param name="Input">Input bit at Index</param>
            public void ConnectInput(int i, Wire Input)
            {
                Inputs[i].ConnectInput(Input);
            }

            /// <summary>
            /// LSB is at index 0 <br/>
            /// MSB is at index ControlBits - 1 
            /// </summary>
            /// <param name="i">Index</param>
            /// <param name="Control">Control bit at Index</param>
            public void ConnectControl(int i, Wire Control)
            {
                Controls[i].ConnectInput(Control);
            }

            private void BuildHeap(int current)
            {
                if (current == 0) gates[current] = new MuxGate();

                //Left son
                if (current * 2 + 1 < gates.Length)
                {
                    gates[current * 2 + 1] = new MuxGate();
                    gates[current].ConnectInput1(gates[current * 2 + 1].Output);
                    BuildHeap(current * 2 + 1);
                }
                //Right son
                if (current * 2 + 2 < gates.Length)
                {
                    gates[current * 2 + 2] = new MuxGate();
                    gates[current].ConnectInput2(gates[current * 2 + 2].Output);
                    BuildHeap(current * 2 + 2);
                }
            }

            private void ConnectWires()
            {
                MuxGate[] Lastlevel = GetLevel(ControlBits - 1);
                for (int i = 0, j = 0; i < Lastlevel.Length; i++, j += 2)
                {
                    Lastlevel[i].ConnectInput1(Inputs[j]);
                    Lastlevel[i].ConnectInput2(Inputs[j + 1]);
                }
                MuxGate[] root = GetLevel(0);
                Output.ConnectInput(root[0].Output);
            }

            private void ConnectControls()
            {
                for (int i = 0; i < ControlBits; i++)
                {
                    MuxGate[] level = GetLevel(ControlBits - 1 - i);
                    foreach (MuxGate gate in level)
                    {
                        gate.ConnectControl(Controls[i]);
                    }
                }
            }

            private MuxGate[] GetLevel(int level)
            {
                MuxGate[] output = new MuxGate[(int)Math.Pow(2, level)];

                int start = 0;
                for (int i = 0; i < level; i++)
                {
                    start = start * 2 + 1;
                }
                int end = start * 2;
                for (int i = start, j = 0; i <= end; i++, j++)
                {
                    output[j] = gates[i];
                }
                return output;
            }
        }
        #endregion
        
        private MultiwayMux Jump_Mux;
        private AndGate PC_Load_And;

        private void ConnectControls()
        {
            //1. connect control of mux 1 (selects entrance to register A)
            m_gAMux.ConnectControl(Instruction[Type]);

            //2. connect control to mux 2 (selects A or M entrance to the ALU)
            Mux2_Control = new AndGate();
            Mux2_Control.ConnectInput1(Instruction[Type]);
            Mux2_Control.ConnectInput2(Instruction[A]);
            m_gMAMux.ConnectControl(Mux2_Control.Output);

            //3. consider all instruction bits only if C type instruction (MSB of instruction is 1)


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
            JEQ = new Wire();
            JEQ.ConnectInput(m_gALU.Zero);
            JLT = new Wire();
            JLT.ConnectInput(m_gALU.Negative);
            JGT = new NotGate();
            JGT_Or = new OrGate();
            JGT_Or.ConnectInput1(JLT);
            JGT_Or.ConnectInput2(JEQ);
            JGT.ConnectInput(JGT_Or.Output);
            JGE = new OrGate();
            JGE.ConnectInput1(JEQ);
            JGE.ConnectInput2(JGT.Output);
            JLE = new OrGate();
            JLE.ConnectInput1(JEQ);
            JLE.ConnectInput2(JLT);
            JNE = new NotGate();
            JNE.ConnectInput(JEQ);
            JMP = new Wire();
            JMP.Value = 1;

            //9. connect jump mux (this is the most complicated part)
            Jump_Mux = new MultiwayMux(3);
            Jump_Mux.ConnectInput(1, JGT.Output);
            Jump_Mux.ConnectInput(2, JEQ);
            Jump_Mux.ConnectInput(3, JGE.Output);
            Jump_Mux.ConnectInput(4, JLT);
            Jump_Mux.ConnectInput(5, JNE.Output);
            Jump_Mux.ConnectInput(6, JLE.Output);
            Jump_Mux.ConnectInput(7, JMP);
            Jump_Mux.ConnectControl(0,Instruction[J0]);
            Jump_Mux.ConnectControl(1, Instruction[J1]);
            Jump_Mux.ConnectControl(2, Instruction[J2]);

            //10. connect PC load control
            PC_Load_And = new AndGate();
            PC_Load_And.ConnectInput1(Jump_Mux.Output);
            PC_Load_And.ConnectInput2(Instruction[Type]);
            m_rPC.Load.ConnectInput(PC_Load_And.Output);
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
