using System;
using System.Drawing;
using System.Windows.Forms;
using Ex05.OthelloLogic;

namespace Ex05.OthelloUI
{
    public class FormOthelloGame : Form
    {
        private FormOthelloSettings m_FormOthelloSettings;
        private OthelloGameLogic m_OthelloGameLogic;
        private ButtonOthelloDisk[,] m_ButtonDiskMatrix;
        private int m_ButtonMatrixSize;
        private bool m_IsSettingsWasClosed = false;

        public FormOthelloGame()
        {
            const bool v_ComputerPlayer = true;
            m_FormOthelloSettings = new FormOthelloSettings();
            m_FormOthelloSettings.FormClosing += m_FormOthelloSettings_FormClosing;
            m_FormOthelloSettings.ShowDialog();
            
            Text = "Othello - White's Turn";
            StartPosition = FormStartPosition.CenterParent;
            AutoSize = true;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            m_ButtonMatrixSize = m_FormOthelloSettings.BoardSize;

            m_OthelloGameLogic = new OthelloGameLogic(m_ButtonMatrixSize, "Player 1", !v_ComputerPlayer, "Player 2", !m_FormOthelloSettings.IsPlayingAgainstHuman);
            m_OthelloGameLogic.GameOverUpdate += GameOverUpdate;
            m_ButtonDiskMatrix = new ButtonOthelloDisk[m_ButtonMatrixSize, m_ButtonMatrixSize];
            createButtons();
        }

        public void BeginGame()
        {
            if (!m_IsSettingsWasClosed)
            {
                ShowDialog();
            }
        }

        private void m_FormOthelloSettings_FormClosing(object i_Sender, FormClosingEventArgs i_EventArgs)
        {
            if (i_EventArgs.CloseReason == CloseReason.UserClosing)
            {
                m_IsSettingsWasClosed = true;
            }
        }

        private void createButtons()
        {
            const int k_LeftStartSpace = 10;
            const int k_DiskSize = 50;
            const int k_BetweenDiskSpace = 5;
            int top = 10;
            int left = 0;
            
            for (int i = 1; i <= m_ButtonMatrixSize; i++)
            {
                left = k_LeftStartSpace;
                for (int j = 1; j <= m_ButtonMatrixSize; j++)
                {
                    m_ButtonDiskMatrix[i - 1, j - 1] = new ButtonOthelloDisk(i, j, m_OthelloGameLogic[i, j].DiskColor);
                    m_ButtonDiskMatrix[i - 1, j - 1].Width = k_DiskSize;
                    m_ButtonDiskMatrix[i - 1, j - 1].Height = k_DiskSize;
                    m_ButtonDiskMatrix[i - 1, j - 1].Location = new Point(top, left);
                    m_ButtonDiskMatrix[i - 1, j - 1].Click += othelloDisk_Click;
                    left += k_DiskSize + k_BetweenDiskSpace;
                    Controls.Add(m_ButtonDiskMatrix[i - 1, j - 1]);
                    m_OthelloGameLogic.DiskUpdate += m_ButtonDiskMatrix[i - 1, j - 1].DiskUpdate;
                }

                top += k_DiskSize + k_BetweenDiskSpace;
                m_OthelloGameLogic.AskForRefreshPossibleNextMove();
            }
        }

        private void othelloDisk_Click(object i_Sender, EventArgs i_EventArgs)
        {
            ButtonOthelloDisk currentButtonOthelloDisk = i_Sender as ButtonOthelloDisk;
            bool isPlayingVsPlayer = m_FormOthelloSettings.IsPlayingAgainstHuman;

            if (currentButtonOthelloDisk != null && m_OthelloGameLogic.IsBlackTurn)
            {
                m_OthelloGameLogic.BlackMakeMove(currentButtonOthelloDisk.Row, currentButtonOthelloDisk.Column);
                if (isPlayingVsPlayer)
                {
                    Text = "Othello - White's Turn";
                }
            }
            else if(currentButtonOthelloDisk != null && !m_OthelloGameLogic.IsBlackTurn)
            {
                m_OthelloGameLogic.WhiteMakeMove(currentButtonOthelloDisk.Row, currentButtonOthelloDisk.Column);
                if (isPlayingVsPlayer)
                {
                    Text = "Othello - Black's Turn";
                }
            }
        }

        public void GameOverUpdate(bool i_IsGameOver)
        {
            string winnerMessage = string.Empty;
            int blackScore = 0;
            int whiteScore = 0;

            if (i_IsGameOver)
            {
                m_OthelloGameLogic.CalculateScore(out blackScore, out whiteScore);
                if (blackScore > whiteScore)
                {
                    winnerMessage = string.Format(@"Black Won!! ({0}/{1}) ({2}/{3})", blackScore, whiteScore, m_OthelloGameLogic.BlackPlayerTotalScore, m_OthelloGameLogic.WhitePlayerTotalScore);
                }
                else if (whiteScore > blackScore)
                {
                    winnerMessage = string.Format(@"White Won!! ({0}/{1}) ({2}/{3})", blackScore, whiteScore, m_OthelloGameLogic.BlackPlayerTotalScore, m_OthelloGameLogic.WhitePlayerTotalScore);
                }
                else
                {
                    winnerMessage = string.Format(@"There is a Tie!! ({0}/{1})", whiteScore, blackScore);
                }

                DialogResult dialogResult = MessageBox.Show(
                    winnerMessage + 
                    @"
Would you like another round?", 
                    "Othello", 
                    MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    m_OthelloGameLogic.StartNextRound();
                }
                else if (dialogResult == DialogResult.No)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}
