using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class WireSet : Component
    {
        private Wire[] m_aWires;
        public int Size { get; private set; }
        public Boolean InputConected { get; private set; }
        private int m_iValue;
        public int Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        private List<Component> Outputs;


        public Wire this[int i]
        {
            get
            {
                if (m_aWires == null)
                {
                    m_aWires = new Wire[Size];
                    for (int j = 0; j < m_aWires.Length; j++)
                    {
                        m_aWires[j] = new Wire(this);
                    }
                }
                return m_aWires[i];
            }
        }

        public WireSet(int iSize)
        {
            Size = iSize;
            InputConected = false;
            m_iValue = 0;
            Outputs = new List<Component>();
        }

        public override string ToString()
        {
            string s = "[";
            if (m_aWires != null)
            {
                for (int i = m_aWires.Length - 1; i >= 0; i--)
                    s += m_aWires[i].Value;
            }
            else
                s += m_iValue;
            s += "]";
            return s;
        }

        public void SetValue(int iValue)
        {
            m_iValue = iValue;
            if (iValue < 0)
            {
                iValue = (int)(Math.Pow(2, Size) + iValue);
            }
            if (m_aWires != null)
            {
                for (int iBit = 0; iBit < Size; iBit++)
                {
                    m_aWires[iBit].Value = iValue % 2;
                    iValue = iValue / 2;
                }
            }
            Compute();
        }

        public int GetValue()
        {
            if (m_aWires != null)
            {
                int iValue = 0;
                for (int iBit = Size - 1; iBit >= 0; iBit--)
                {
                    iValue *= 2;
                    iValue += m_aWires[iBit].Value;
                }
                if (iValue != m_iValue)
                {
                    m_iValue = iValue;
                }
            }
            return m_iValue;
        }

        public void ConnectInput(WireSet wIn)
        {
            if (InputConected)
                throw new InvalidOperationException("Cannot connect a wire to more than one inputs");
            if(wIn.Size != Size)
                throw new InvalidOperationException("Cannot connect two wiresets of different sizes.");

            wIn.ConnectOutput(this);

            InputConected = true;
            
        }

        public void ConnectOutput(Component c)
        {
            Outputs.Add(c);
            
            c.Compute();
        }


        #region Component Members

        public void Compute()
        {
            GetValue();
            foreach (Component c in Outputs)
            {
                if (c is WireSet)
                    ((WireSet)c).SetValue(m_iValue);
                else
                    c.Compute();
            }
        }

        #endregion
    }
}
