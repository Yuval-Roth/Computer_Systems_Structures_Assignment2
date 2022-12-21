using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleComponents
{
    public class ALU : Component
    {
        public int Size { get; private set; }
        //inputs
        public WireSet InputX { get; private set; }
        public WireSet InputY { get; private set; }
        public WireSet Control { get; private set; }

        //outputs
        public WireSet Output { get; private set; }
        public Wire Zero { get; private set; }
        public Wire Negative { get; private set; }

        public ALU(int iSize)
        {
            Size = iSize;
            InputX = new WireSet(Size);
            InputY = new WireSet(Size);
            Control = new WireSet(5);
            Output = new WireSet(Size);
            Zero = new Wire();
            Negative = new Wire();

            InputX.ConnectOutput(this);
            InputY.ConnectOutput(this);
            Control.ConnectOutput(this);
        }

        #region Component Members

        public void Compute()
        {
            //int iX = InputX.GetValue(), iY = InputY.GetValue();
            int iX = InputX.GetValue(), iY = InputY.GetValue();
            //constants
            if (Control.GetValue() == 0)
                Output.SetValue(0);
            if (Control.GetValue() == 1)
                Output.SetValue(1);

            //Single input ops
            if (Control.GetValue() == 2)
                Output.SetValue(iX);
            if (Control.GetValue() == 3)
                Output.SetValue(iY);
            if (Control.GetValue() == 4)
                Output.SetValue(~iX);
            if (Control.GetValue() == 5)
                Output.SetValue(~iY);
            if (Control.GetValue() == 6)
                Output.SetValue(-iX);
            if (Control.GetValue() == 7)
                Output.SetValue(-iY);

            //Add ops
            if (Control.GetValue() == 8)
                Output.SetValue(iX + 1);

            if (Control.GetValue() == 9)
                Output.SetValue(iY + 1);

            if (Control.GetValue() == 10)
                Output.SetValue(iX - 1);

            if (Control.GetValue() == 11)
                Output.SetValue(iY - 1);

            if (Control.GetValue() == 12)
                Output.SetValue(iX + iY);

            if (Control.GetValue() == 13)
                Output.SetValue(iX - iY);

            if (Control.GetValue() == 14)
                Output.SetValue(iY - iX);


            //and or
            if (Control.GetValue() == 15)
                Output.SetValue((iX & iY));
            if (Control.GetValue() == 16)
            {
                if (iX == 0 || iY == 0)
                    Output.SetValue(0);
                else
                    Output.SetValue(1);
            }
            if (Control.GetValue() == 17)
                Output.SetValue((iX | iY));
            if (Control.GetValue() == 18)
            {
                if (iX == 0 && iY == 0)
                    Output.SetValue(0);
                else
                    Output.SetValue(1);
            }

            if (Output.GetValue() == 0)
                Zero.Value = 1;
            else
                Zero.Value = 0;

            if (Output.GetValue() < 0)
                Negative.Value = 1;
            else
                Negative.Value = 0;

            //Console.WriteLine("ALU state: " + ToString());
        }


        #endregion

        private static void TestNumbers(ALU alu, int iX, int iY, HashSet<string> hErrors)
        {
            alu.InputX.SetValue(iX);
            alu.InputY.SetValue(iY);

            alu.Control.SetValue(0);
            if (alu.Output.GetValue() != 0)
                hErrors.Add("0" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(1);
            if (alu.Output.GetValue() != 1)
                hErrors.Add("1" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(2);
            if (alu.Output.GetValue() != iX)
                hErrors.Add("x" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(3);
            if (alu.Output.GetValue() != iY)
                hErrors.Add("y" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(4);
            if (alu.Output.GetValue() != ~iX)
                hErrors.Add("!x" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(5);
            if (alu.Output.GetValue() != ~iY)
                hErrors.Add("!y" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(6);
            if (alu.Output.GetValue() != -iX)
                hErrors.Add("-x" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(7);
            if (alu.Output.GetValue() != -iY)
                hErrors.Add("-y" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(8);
            if (alu.Output.GetValue() != iX + 1)
                hErrors.Add("x+1" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(9);
            if (alu.Output.GetValue() != iY + 1)
                hErrors.Add("y+1" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(10);
            if (alu.Output.GetValue() != iX - 1)
                hErrors.Add("x-1" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(11);
            if (alu.Output.GetValue() != iY - 1)
                hErrors.Add("y-1" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(12);
            if (alu.Output.GetValue() != iX + iY)
                hErrors.Add("x+y" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(13);
            if (alu.Output.GetValue() != iX - iY)
                hErrors.Add("x-y" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(14);
            if (alu.Output.GetValue() != iY - iX)
                hErrors.Add("y-x" + " x = " + iX + " y = " + iY);



            iX = Math.Abs(iX);
            iY = Math.Abs(iY);
            alu.InputX.SetValue(iX);
            alu.InputY.SetValue(iY);

            alu.Control.SetValue(15);
            if (alu.Output.GetValue() != (iX & iY))
                hErrors.Add("x&y " + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(16);
            if (alu.Output.GetValue() == 0 && (iY != 0 && iX != 0))
                hErrors.Add("x*y (logical and)" + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(17);
            if (alu.Output.GetValue() != (iX | iY))
                hErrors.Add("x|y " + " x = " + iX + " y = " + iY);

            alu.Control.SetValue(18);
            if (alu.Output.GetValue() == 0 && (iY != 0 || iX != 0))
                hErrors.Add("x v y (logical or)" + " x = " + iX + " y = " + iY);


        }


        public bool TestGate()
        {
            HashSet<string> hsErrors = new HashSet<string>();
            TestNumbers(this, 0, 0, hsErrors);
            Random rnd = new Random(0);
            int iMax = (int)Math.Pow(2, Size - 2); // we can do up to 2^n-1 but then we'd might get x+y > 2^n-1
            for (int i = 0; i < 100; i++)
            {
                int iX = rnd.Next(iMax * -1, iMax);
                int iY = rnd.Next(iMax * -1, iMax);
                TestNumbers(this, iX, iY, hsErrors);
                if (hsErrors.Count > 0)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "X=" + InputX + ", Y=" + InputY + ", C=" + Control + ", Output=" + Output;
        }
    }
}

