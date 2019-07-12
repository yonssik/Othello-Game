using System;
using System.Windows.Forms;
using System.Drawing;

namespace Ex05.OthelloUI
{
    public class FormOthelloSettings : Form
    {
        private const int k_MaxBoardSize = 12;
        private const int k_MinBoardSize = 6;
        private const bool v_PlayingAgainstHuman = true;
        private Button m_ButtonBoardSize;
        private Button m_ButtonPlayerVsComputer;
        private Button m_ButtonPlayerVsPlayer;
        private bool m_IsPlayingAgainstHuman = true;
        private int m_BoardSize;

        public int BoardSize
        {
            get
            {
                return m_BoardSize;
            }
        }

        public bool IsPlayingAgainstHuman
        {
            get
            {
                return m_IsPlayingAgainstHuman;
            }
        }

        public FormOthelloSettings()
        {
            const int k_OthelloSettingsWidth = 450;
            const int k_OthelloSettingsHeight = 300;

            m_BoardSize = k_MinBoardSize;
            Text = "Othello - Game Settings";
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            Width = k_OthelloSettingsWidth;
            Height = k_OthelloSettingsHeight;
            initializeButtons();
            Controls.Add(m_ButtonBoardSize);
            Controls.Add(m_ButtonPlayerVsComputer);
            Controls.Add(m_ButtonPlayerVsPlayer);
        }

        private void initializeButtons()
        {
            const string k_ButtonTextFont = "Arial";
            const int k_ButtonTextSize = 14;
            const int k_ButtonHeight = 100;
            const int k_ButtonWidht = 200;
            const int k_ButtonSpaceLeft = 10;
            const int k_ButtonSpaceTop = 150;
            const int k_ButtonSpaceBetween = 10;
            const int k_ButtonSpaceTopStart = 20;

            m_ButtonBoardSize = new Button();
            m_ButtonPlayerVsComputer = new Button();
            m_ButtonPlayerVsPlayer = new Button();

            m_ButtonBoardSize.Text = "Board Size: 6x6 (click to increase)";
            m_ButtonPlayerVsComputer.Text = "Play against the computer";
            m_ButtonPlayerVsPlayer.Text = "Play against your friend";

            m_ButtonBoardSize.Font = new Font(k_ButtonTextFont, k_ButtonTextSize);
            m_ButtonBoardSize.Width = (2 * k_ButtonWidht) + k_ButtonSpaceLeft;
            m_ButtonBoardSize.Height = k_ButtonHeight;
            m_ButtonBoardSize.Top = k_ButtonSpaceTopStart;
            m_ButtonBoardSize.Left = k_ButtonSpaceLeft;

            m_ButtonPlayerVsComputer.Font = new Font(k_ButtonTextFont, k_ButtonTextSize);
            m_ButtonPlayerVsComputer.Width = k_ButtonWidht;
            m_ButtonPlayerVsComputer.Height = k_ButtonHeight;
            m_ButtonPlayerVsComputer.Top = k_ButtonSpaceTop;
            m_ButtonPlayerVsComputer.Left = k_ButtonSpaceLeft;

            m_ButtonPlayerVsPlayer.Font = new Font(k_ButtonTextFont, k_ButtonTextSize);
            m_ButtonPlayerVsPlayer.Width = k_ButtonWidht;
            m_ButtonPlayerVsPlayer.Height = k_ButtonHeight;
            m_ButtonPlayerVsPlayer.Top = k_ButtonSpaceTop;
            m_ButtonPlayerVsPlayer.Left = k_ButtonSpaceLeft + k_ButtonSpaceBetween + k_ButtonWidht;

            m_ButtonBoardSize.Click += m_ButtonBoardSize_Click;
            m_ButtonPlayerVsComputer.Click += m_ButtonPlayerVsComputer_Click;
            m_ButtonPlayerVsPlayer.Click += m_ButtonPlayerVsPlayer_Click;
        }

        private void m_ButtonPlayerVsPlayer_Click(object i_Sender, EventArgs i_EventArgs)
        {
            m_IsPlayingAgainstHuman = v_PlayingAgainstHuman;
            DialogResult = DialogResult.OK;
            Visible = false;
        }

        private void m_ButtonPlayerVsComputer_Click(object i_Sender, EventArgs i_EventArgs)
        {
            m_IsPlayingAgainstHuman = !v_PlayingAgainstHuman;
            DialogResult = DialogResult.OK;
            Visible = false;
        }

        private void m_ButtonBoardSize_Click(object i_Sender, EventArgs i_EventArgs)
        {
            const int k_IncreaseOfBoardSize = 2;

            if (m_BoardSize == k_MaxBoardSize)
            {
                m_BoardSize = k_MinBoardSize;
            }
            else
            {
                m_BoardSize += k_IncreaseOfBoardSize;
            }

            m_ButtonBoardSize.Text = string.Format("Board Size: {0}x{1} (click to increase)", m_BoardSize, m_BoardSize);
        }
    }
}
