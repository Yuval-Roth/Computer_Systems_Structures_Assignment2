using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class BitwiseDemux : Gate, Component
    {
        public int Size { get; private set; }
        public WireSet Output1 { get; private set; }
        public WireSet Output2 { get; private set; }
        public WireSet Input { get; private set; }
        public Wire Control { get; private set; }

        public BitwiseDemux(int iSize)
        {
            Size = iSize;
            Control = new Wire();
            Input = new WireSet(Size);
            Output1 = new WireSet(Size);
            Output2 = new WireSet(Size);
            Input.ConnectOutput(this);
        }


        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public override bool TestGate()
        {
            throw new NotImplementedException();
        }

        #region Component Members

        public void Compute()
        {
            if (Control.Value == 0)
            {
                Output1.SetValue(Input.GetValue());
                Output2.SetValue(0);
            }
            else
            {
                Output2.SetValue(Input.GetValue());
                Output1.SetValue(0);
            }
        }

        #endregion
    }
}
