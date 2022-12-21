using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class OrGate : BooleanGate
    {

        public OrGate()
        {
        }


        public override string ToString()
        {
            return "Or " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }

        public override bool TestGate()
        {
            Wire w1 = new Wire(), w2 = new Wire();
            ConnectInput1(w1);
            ConnectInput2(w2);
            w1.Value = 0;
            w2.Value = 0;
            
            if (Output.Value != 0)
                return false;
            w1.Value = 0;
            w2.Value = 1;
            
            if (Output.Value != 1)
                return false;
            w1.Value = 1;
            w2.Value = 0;
            
            if (Output.Value != 1)
                return false;
            w1.Value = 1;
            w2.Value = 1;
            
            if (Output.Value != 1)
                return false;
            return true;
        }

        public override void Compute()
        {
            if (Input1.Value == 1 || Input2.Value == 1)
                Output.Value = 1;
            else
                Output.Value = 0;
        }
    }

}
