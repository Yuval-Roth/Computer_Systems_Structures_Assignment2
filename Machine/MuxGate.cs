using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class MuxGate : BooleanGate
    {
        public Wire Control { get; private set; }


        public MuxGate()
        {
            Control = new Wire();
            Control.ConnectOutput(this);
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }


        public override string ToString()
        {
            return "Mux " + Input1.Value + "," + Input2.Value + ",C" + Control.Value + " -> " + Output.Value;
        }



        public override bool TestGate()
        {
            
            return true;
        }

        public override void Compute()
        {
            if (Control == null || Control.Value == 0)
                Output.Value = Input1.Value;
            else
                Output.Value = Input2.Value;
        }
    }
}
