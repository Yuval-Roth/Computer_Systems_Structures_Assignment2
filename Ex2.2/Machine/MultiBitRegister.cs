using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class MultiBitRegister : SequentialGate
    {
        private int m_iValue;
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public Wire Load { get; private set; }
        public int Size { get; private set; }



        public MultiBitRegister(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            Load = new Wire();
        }

        public void ConnectInput(WireSet ws)
        {
            Input.ConnectInput(ws);
        }

        public override string ToString()
        {
            return Output.ToString();
        }

        public override void OnClockUp()
        {
            Output.SetValue(m_iValue);
        }

        public override void OnClockDown()
        {
            if (Load.Value == 1)
                m_iValue = Input.GetValue();
        }

        public override bool TestGate()
        {
            throw new NotImplementedException();
        }
    }
}
