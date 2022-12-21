using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public abstract class BitwiseBooleanGate : Gate, Component
    {
        public WireSet Input1 { get; protected set; }
        public WireSet Input2 { get; protected set; }
        public WireSet Output { get; protected set; }
        public int Size { get; private set; }

        public BitwiseBooleanGate(int iSize)
        {
            Size = iSize;
            Input1 = new WireSet(iSize);
            Input2 = new WireSet(iSize);
            Output = new WireSet(iSize);
            Input1.ConnectOutput(this);
            Input2.ConnectOutput(this);
        }

        public void ConnectInput1(WireSet wInput)
        {
            Input1.ConnectInput(wInput);
        }
        public void ConnectInput2(WireSet wInput)
        {
            Input2.ConnectInput(wInput);
        }


        #region Component Members

        public abstract void Compute();

        #endregion
    }
}
