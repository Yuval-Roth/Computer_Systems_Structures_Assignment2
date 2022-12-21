using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class ScreenUI : Form
    {
        public int FontSize { get; set; }
        private int m_cColumns, m_cRows;
        private char[,] m_aScreen;
        private bool m_bChanged;

        public ScreenUI(int cRows, int cColumns)
        {
            m_cColumns = cColumns;
            m_cRows = cRows;
            m_aScreen = new char[m_cRows, m_cColumns];
            m_bChanged = false;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            FontSize = 10;
        }

        private void ScreenUI_Load(object sender, EventArgs e)
        {
            Width = m_cColumns * FontSize + 18;
            Height = m_cRows * (FontSize + 4) + 40;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            for (int iRow = 0; iRow < m_cRows; iRow++)
                for (int iColumn = 0; iColumn < m_cColumns; iColumn++)
                    DrawChar(iRow, iColumn, g);

        }


        public void SetChar(int iRow, int iColumn, char c)
        {
            if (m_aScreen[iRow, iColumn] != c)
            {
                m_aScreen[iRow, iColumn] = c;
                DrawChar(iRow, iColumn, CreateGraphics());
                m_bChanged = true;
            }
        }
        private void DrawChar(int iRow, int iColumn, Graphics g)
        {
            RectangleF rectf = new RectangleF(10 * iColumn, 14 * iRow, 10, 14);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.FillRectangle(Brushes.Green, FontSize * iColumn, (int)(FontSize + 4) * iRow, FontSize, (int)(FontSize + 4));
            g.DrawString(m_aScreen[iRow, iColumn] + "", new Font("New Courier", FontSize), Brushes.Black, rectf);

            g.Flush();
        }
        public void SetChar(int iRow, int iColumn, int c)
        {
            if (m_aScreen[iRow, iColumn] != c)
            {
                try
                {

                    m_aScreen[iRow, iColumn] = (char)c;
                    DrawChar(iRow, iColumn, CreateGraphics());
                    m_bChanged = true;
                }
                catch (Exception e)
                {
                }
            }
        }
        
    }
}
