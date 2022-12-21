using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class BitwiseAndGate : BitwiseBooleanGate
    {
        public BitwiseAndGate(int iSize)
            : base(iSize)
        {
            
        }


        public override string ToString()
        {
            return "And " + Input1 + ", " + Input2 + " -> " + Output;
        }

        public override bool TestGate()
        {
            throw new NotImplementedException();
        }


        public override void Compute()
        {
            int iAndValue = 0, i1 = Input1.Value, i2 = Input2.Value;
            int iOffset = 1;
            for (int i = 0; i < Size; i++)
            {
                if (i1 % 2 == 1 && i2 % 2 == 1)
                    iAndValue += iOffset;
                iOffset *= 2;
                i1 /= 2;
                i2 /= 2;
            }
            Output.SetValue(iAndValue);
        }
    }
}
