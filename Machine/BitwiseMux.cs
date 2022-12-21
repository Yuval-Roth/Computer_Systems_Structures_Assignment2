using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class BitwiseMux : BitwiseBooleanGate
    {
        public Wire Control { get; private set; }


        public BitwiseMux(int iSize):base(iSize)
        {
            Control = new Wire();
            Control.ConnectOutput(this);
            Input1.ConnectOutput(this);
            Input2.ConnectOutput(this);
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }



        public override string ToString()
        {
            return "Mux " + Input1 + "," + Input2 + ",C" + Control.Value + " -> " + Output;
        }



        public override bool TestGate()
        {
            
            return true;
        }

 
        public override void Compute()
        {
            if (Control != null && Control.Value == 0)
                Output.SetValue(Input1.GetValue());
            else
                Output.SetValue(Input2.GetValue());

        }

    }
}
