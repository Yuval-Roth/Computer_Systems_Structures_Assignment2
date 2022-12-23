using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class Wire : Component
    {
        public bool InputConected { get; private set; }
        private List<Component> Outputs;
        private int m_iValue;
        private WireSet m_wsContainer;

        public int Value
        {
            get
            {
                return m_iValue;
            }
            set
            {
                if (value != 0 && value != 1)
                    throw new InvalidOperationException("Valid values are only 0 and 1");

                if (value == m_iValue)
                    return;

                m_iValue = value;
                foreach (Component c in Outputs)
                {
                    if (c is Wire)
                        ((Wire)c).Value = value;
                    else
                        c.Compute();
                }
                if (m_wsContainer != null)
                    m_wsContainer.Compute();
            }
        }

        public Wire()
        {
            Outputs = new List<Component>();
            Value = 0;
        }

        public Wire(WireSet wsContainer):this()
        {
            m_wsContainer = wsContainer;
        }

        public void ConnectInput(Wire wIn)
        {
            if (InputConected)
                throw new InvalidOperationException("Cannot connect a wire to more than one inputs");
            InputConected = true;
            wIn.Outputs.Add(this);
            Value = wIn.Value;
        }
        public override string ToString()
        {
            return m_iValue + "";
        }

        
        public void ConnectOutput(Component cOut)
        {
            Outputs.Add(cOut);
            cOut.Compute();
        }
         


        #region Component Members

        public void Compute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
