using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class Memory: SequentialGate, Component
    {
        public string Name { get; private set; }
        public int AddressSize { get; private set; }
        public int WordSize { get; private set; }

        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public WireSet Address { get; private set; }
        public Wire Load { get; private set; }

        public int Size { get; private set; }

        private int[] m_aiRegisters;

        public int this[int iAddress]
        {
            get
            {
                return m_aiRegisters[iAddress];
            }
            set
            {
                m_aiRegisters[iAddress] = value;
            }
        }

        public Memory(int iAddressSize, int iWordSize, string sName)
        {
            AddressSize = iAddressSize;
            WordSize = iWordSize;

            Input = new WireSet(WordSize);
            Output = new WireSet(WordSize);
            Address = new WireSet(AddressSize);
            Load = new Wire();

            Size = (int)Math.Pow(2,Address.Size);
            m_aiRegisters = new int[Size];

            Address.ConnectOutput(this);
            Name = sName;
        }

        public void LoadFromFile(string sFileName)
        {
            StreamReader sr = new StreamReader(sFileName);
            int iAddress = 0;
            while (!sr.EndOfStream)
            {
                string sLine = sr.ReadLine().Trim();
                if (sLine.Length != WordSize)
                    throw new FormatException("Every line must contain a single instruction with " + WordSize + " bits (line " + iAddress + ").");
                int iValue = 0;
                for (int i = 0; i < WordSize; i++)
                {
                    if (sLine[i] == '1')
                        iValue = iValue * 2 + 1;
                    else if (sLine[i] == '0')
                        iValue = iValue * 2 + 0;
                    else
                        throw new FormatException("Can only have 0 or 1 for bits (line " + iAddress + ").");
                }
                m_aiRegisters[iAddress] = iValue;

                iAddress++;
            }

        }

        

        public override void OnClockUp()
        {
            Compute();
        }

        public override void OnClockDown()
        {
            if (Load.Value == 1)
            {
                int iAddress = Address.GetValue();
                m_aiRegisters[iAddress] = Input.GetValue();
            }
        }
        
        public void RegisterMemoryMappedIO(MemoryMappedIO io, int iOffset, int cRegisters, out int[] aiRegisters)
        {
            //TODO - we can register the io here to avoid duplicate settings
            aiRegisters = m_aiRegisters;
        }
        
        public override bool TestGate()
        {
            throw new NotImplementedException();
        }

        #region Component Members

        public void Compute()
        {
            int iAddress = Address.GetValue();
            if(iAddress >= m_aiRegisters.Length)
            {
                throw new Exception("Address " + iAddress + " out of range for memory " + Name);
            }
            Output.SetValue(m_aiRegisters[iAddress]);
        }

        #endregion
    }
}
