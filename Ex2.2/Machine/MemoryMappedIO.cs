using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleComponents
{
    public abstract class MemoryMappedIO : SequentialGate
    {
        protected int[] m_aiMemoryMap;
        protected int m_iOffset;
    }
}
