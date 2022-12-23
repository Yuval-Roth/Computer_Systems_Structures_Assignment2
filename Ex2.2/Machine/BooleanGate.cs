using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public abstract class BooleanGate : Gate, Component
    {
        public Wire Input1 { get; protected set; }
        public Wire Input2 { get; protected set; }
        public Wire Output { get; protected set; }

        public BooleanGate()
        {
            Input1 = new Wire();
            Input2 = new Wire();
            Output = new Wire();
            Input1.ConnectOutput(this);
            Input2.ConnectOutput(this);
        }

        public void ConnectInput1(Wire wInput)
        {
            Input1.ConnectInput(wInput);
        }
        public void ConnectInput2(Wire wInput)
        {
            Input2.ConnectInput(wInput);
        }


        #region Component Members

        public abstract void Compute();

        #endregion
    }
}
