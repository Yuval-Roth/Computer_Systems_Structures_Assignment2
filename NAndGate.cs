using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class NAndGate : BooleanGate
    {
        public static int NAND_GATES = 0;
        public static int NAND_COMPUTE = 0;

        public NAndGate()
        {
            NAND_GATES++;
            Input1.ConnectOutput(this);
            Input2.ConnectOutput(this);
        }

        public override string ToString()
        {
            return "NAnd " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }


        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;
            
            if (Output.Value != 1)
                return false;
            Input1.Value = 0;
            Input2.Value = 1;
            
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 0;
            
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 1;
            
            if (Output.Value != 0)
                return false;
            return true;
        }

        public override void Compute()
        {
            NAND_COMPUTE++;
            if (Input1.Value == 1 && Input2.Value == 1)
                Output.Value = 0;
            else
                Output.Value = 1;
        }

    }
}
