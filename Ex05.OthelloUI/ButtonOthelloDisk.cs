using System.Drawing;
using System.Windows.Forms;
using Ex05.OthelloLogic;

namespace Ex05.OthelloUI
{
    public class ButtonOthelloDisk : Button
    {
        private readonly int r_Row;
        private readonly int r_Column;
        private eCurrentDiskMode m_DiskMode;

        public eCurrentDiskMode DiskMode
        {
            get
            {
                return m_DiskMode;
            }

            set
            {
                m_DiskMode = value;
            }
        }

        public int Row
        {
            get
            {
                return r_Row;
            }
        }

        public int Column
        {
            get
            {
                return r_Column;
            }
        }

        public ButtonOthelloDisk(int i_Row, int i_Column, eCurrentDiskMode i_DiskMode)
        {
            r_Row = i_Row;
            r_Column = i_Column;
            m_DiskMode = i_DiskMode;
            updateStatus(i_DiskMode);
        }

        private void updateStatus(eCurrentDiskMode i_DiskMode)
        {
            const string k_DiskSign = "O";
            if (i_DiskMode == eCurrentDiskMode.NotExist)
            {
                BackColor = Color.Empty;
                Text = string.Empty;
                Enabled = false;
            }
            else if (i_DiskMode == eCurrentDiskMode.Black)
            {
                BackColor = Color.Black;
                Text = k_DiskSign;
                ForeColor = Color.White;
                Enabled = true;
            }
            else if (i_DiskMode == eCurrentDiskMode.White)
            {
                BackColor = Color.White;
                Text = k_DiskSign;
                ForeColor = Color.Black;
                Enabled = true;
            }
            else if (i_DiskMode == eCurrentDiskMode.Green)
            {
                BackColor = Color.LimeGreen;
                Text = string.Empty;
                ForeColor = Color.Black;
                Enabled = true;
            }
        }

        public void DiskUpdate(int i_Row, int i_Column, eCurrentDiskMode i_DiskMode)
        {
            if (r_Row == i_Row && r_Column == i_Column)
            {
                updateStatus(i_DiskMode);
            }
        }
    }
}
