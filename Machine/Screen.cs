using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
//using System.Drawing.Drawing2D;
using SimpleComponents;

namespace Machine
{
    public class Screen : MemoryMappedIO
    {
        private ScreenUI m_fScreen;
        private int m_cRows, m_cColumns;
        private const int PIXEL_SIZE = 2;
        private const int CHAR_SIZE = 12;
        private int m_cWordsPerColumn;
        private int m_iWordSize;
        private Thread m_tScreenThread;

        public Screen(Memory data, int iOffset, int cRows, int cColumns)
        {
            m_iOffset = iOffset;
            data.RegisterMemoryMappedIO(this, iOffset, cRows * cColumns / 16, out m_aiMemoryMap);
            m_cRows = cRows;
            m_cColumns = cColumns;
            m_iWordSize = data.WordSize;
            m_cWordsPerColumn = cColumns / m_iWordSize;

            m_fScreen = new ScreenUI(cRows, cColumns);
            Application.EnableVisualStyles();


            m_tScreenThread = new Thread(Start);
            m_tScreenThread.Start();
        }


        private void Start()
        {
            Application.Run(m_fScreen);
        }

        public void Close()
        {
            Application.Exit();
        }
        public override void OnClockUp()
        {
            
            for (int iRow = 0; iRow < m_cRows; iRow++)
            {
                for (int iColumn = 0; iColumn < m_cColumns; iColumn++)
                {
                    m_fScreen.SetChar(iRow, iColumn, m_aiMemoryMap[m_iOffset + iRow * m_cColumns + iColumn]);
                }
            }
        }

        public override void OnClockDown()
        {
        }

        public void SetColoredLines(bool bEven)
        {
            for (int iRow = 0; iRow < m_cRows; iRow++)
            {
                for (int iWord = 0; iWord < m_cColumns / m_iWordSize; iWord++)
                {
                    if ((iRow % 2 == 0 && bEven) || (iRow % 2 == 1 && !bEven))
                        m_aiMemoryMap[m_iOffset + iRow * m_cWordsPerColumn + iWord] = 0xFFFF;
                    else
                        m_aiMemoryMap[m_iOffset + iRow * m_cWordsPerColumn + iWord] = 0;
                }
            }
        }

        public override bool TestGate()
        {
            throw new NotImplementedException();
        }
    }
}
