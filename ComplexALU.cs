using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class ComplexALU : Component
    {
        public WireSet InputX { get; private set; }
        public WireSet InputY { get; private set; }
        public WireSet Output { get; private set; }

        public Wire ZeroX { get; private set; }
        public Wire ZeroY { get; private set; }
        public Wire NotX { get; private set; }
        public Wire NotY { get; private set; }
        public Wire F { get; private set; }
        public Wire NotOutput { get; private set; }

        public Wire Zero { get; private set; }
        public Wire Negative { get; private set; }

        public int Size { get; private set; }

        public ComplexALU(int iSize)
        {
            Size = iSize;
            InputX = new WireSet(Size);
            InputY = new WireSet(Size);
            ZeroX = new Wire();
            ZeroY = new Wire();
            NotX = new Wire();
            NotY = new Wire();
            F = new Wire();
            NotOutput = new Wire();
            Output = new WireSet(Size);
            Zero = new Wire();
            Negative = new Wire();

            ZeroX.ConnectOutput(this);
            ZeroY.ConnectOutput(this);
            NotX.ConnectOutput(this);
            NotY.ConnectOutput(this);
            F.ConnectOutput(this);
            NotOutput.ConnectOutput(this);

            InputX.ConnectOutput(this);
            InputY.ConnectOutput(this);
        }

        #region Component Members

        public void Compute()
        {
            //int iX = InputX.Get2sComplement(), iY = InputY.Get2sComplement();
            int iX = InputX.GetValue(), iY = InputY.GetValue();
            if (ZeroX.Value == 1)
                iX = 0;
            if (ZeroY.Value == 1)
                iY = 0;
            if (NotX.Value == 1)
                iX = ~iX;
            if (NotY.Value == 1)
                iY = ~iY;

            int iResult = 0;
            if (F.Value == 0)
                iResult = iX & iY;
            else
                iResult = iX + iY;

            if (NotOutput.Value == 1)
                iResult = ~iResult;

            Output.SetValue(iResult);
            //Output.Set2sComplement(iResult);
            if (iResult == 0)
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

        private void SetControlBits(int iZeroX, int iNotX, int iZeroY, int iNotY, int iF, int iNotOutput)
        {
            ZeroX.Value = iZeroX;
            ZeroY.Value = iZeroY;
            NotX.Value = iNotX;
            NotY.Value = iNotY;
            F.Value = iF;
            NotOutput.Value = iNotOutput;
        }

        private bool TestNumbers(int iX, int iY)
        {
            InputX.SetValue(iX);
            InputY.SetValue(iY);

            SetControlBits(1, 0, 1, 0, 1, 0);

            if (Output.GetValue() != 0)
                return false;

            SetControlBits(1, 1, 1, 1, 1, 1);

            if (Output.GetValue() != 1)
                return false;

            SetControlBits(1, 1, 1, 0, 1, 0);

            if (Output.GetValue() != -1)
                return false;

            SetControlBits(0, 0, 1, 1, 0, 0);

            if (Output.GetValue() != iX)
                return false;

            SetControlBits(1, 1, 0, 0, 0, 0);

            if (Output.GetValue() != iY)
                return false;

            SetControlBits(0, 0, 1, 1, 0, 1);

            if (iX > 0 && Output.GetValue() != ~iX)
                return false;

            SetControlBits(1, 1, 0, 0, 0, 1);

            if (iY > 0 && Output.GetValue() != ~iY)
                return false;

            SetControlBits(0, 1, 1, 1, 1, 1);

            if (Output.GetValue() != iX + 1)
                return false;

            SetControlBits(1, 1, 0, 1, 1, 1);

            if (Output.GetValue() != iY + 1)
                return false;

            SetControlBits(0, 0, 1, 1, 1, 0);

            if (Output.GetValue() != iX - 1)
                return false;

            SetControlBits(1, 1, 0, 0, 1, 0);

            if (Output.GetValue() != iY - 1)
                return false;

            SetControlBits(0, 0, 0, 0, 1, 0);

            if (Output.GetValue() != iX + iY)
                return false;

            SetControlBits(0, 1, 0, 0, 1, 1);

            if (Output.GetValue() != iX - iY)
                return false;

            SetControlBits(0, 0, 0, 1, 1, 1);

            if (Output.GetValue() != iY - iX)
                return false;

            SetControlBits(0, 0, 0, 0, 0, 0);

            if (Output.GetValue() != (iY & iX))
                return false;

            SetControlBits(0, 1, 0, 1, 0, 1);

            if (Output.GetValue() != (iY | iX))
                return false;

            return true;
        }

        public bool TestGate()
        {
            TestNumbers(0, 0);
            Random rnd = new Random(0);
            int iMax = (int)Math.Pow(2, Size - 2); // we can do up to 2^n-1 but then we'd might get x+y > 2^n-1
            for (int i = 0; i < 100; i++)
            {
                int iX = rnd.Next(iMax * -1, iMax);
                int iY = rnd.Next(iMax * -1, iMax);
                if (!TestNumbers(iX, iY))
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return InputX + ", " + InputY + ", C=[" + ZeroX.Value + NotX.Value + ZeroY.Value + NotY.Value + F.Value + NotOutput.Value + "], " + Output;
        }
    }
}

