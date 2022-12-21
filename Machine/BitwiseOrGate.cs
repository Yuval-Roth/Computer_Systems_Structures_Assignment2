using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class BitwiseOrGate : BitwiseBooleanGate
    {
        public BitwiseOrGate(int iSize)
            : base(iSize)
        {
            
        }


        public override string ToString()
        {
            return "Or " + Input1 + ", " + Input2 + " -> " + Output;
        }

        public override bool TestGate()
        {
            throw new NotImplementedException();
        }

        public override void Compute()
        {
            int iOrValue = 0, i1 = Input1.Value, i2 = Input2.Value;
            int iOffset = 1;
            for (int i = 0; i < Size; i++)
            {
                if (i1 % 2 == 1 || i2 % 2 == 1)
                    iOrValue += iOffset;
                iOffset *= 2;
                i1 /= 2;
                i2 /= 2;
            }
            Output.SetValue(iOrValue);
        }

    }
}
