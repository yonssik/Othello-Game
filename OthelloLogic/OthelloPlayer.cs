namespace Ex05.OthelloLogic
{
    public class OthelloPlayer
    {
        private static int s_NumberOfPlayers = 0;
        private string m_PlayerName;
        private bool m_IsPlayerComputer;

        public bool IsPlayerComputer
        {
            get
            {
                return m_IsPlayerComputer;
            }

            set
            {
                m_IsPlayerComputer = value;
            }
        }

        public string PlayerName
        {
            get
            {
                return m_PlayerName;
            }

            set
            {
                if (value == string.Empty && s_NumberOfPlayers == 1)
                {
                    value = "Player 1";
                }
                else if(value == string.Empty && s_NumberOfPlayers == 2)
                {
                    value = "Player 2";
                }

                m_PlayerName = value;
            }
        }

        public OthelloPlayer(string i_PlayerName, bool i_IsPlayerComputer)
        {
            s_NumberOfPlayers++;
            PlayerName = i_PlayerName;
            IsPlayerComputer = i_IsPlayerComputer;
        }
    }
}
