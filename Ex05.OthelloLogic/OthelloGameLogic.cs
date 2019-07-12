using System;

namespace Ex05.OthelloLogic
{
    public delegate void DiskUpdateEventHandler(int i_Row, int i_Column, eCurrentDiskMode i_DiskMode);

    public delegate void GameOverUpdateEventHandler(bool i_IsGameOver);

    public class OthelloGameLogic
    {
        private const int k_MinimumBoardSize = 6;
        private const int k_MaximumBoardSize = 12;
        private readonly int r_BoardLength;
        private OthelloGameDisk[,] m_TheOthelloBoardGame;
        private bool m_IsGameOver = false;
        private bool m_IsMinMaxSimulation = false;
        private bool m_IsBlackTurn = false;
        private OthelloPlayer m_PlayerOne;
        private OthelloPlayer m_PlayerTwo;
        private int m_BlackPlayerTotalScore;
        private int m_WhitePlayerTotalScore;

        public event DiskUpdateEventHandler DiskUpdate;

        public event GameOverUpdateEventHandler GameOverUpdate;

        public int BlackPlayerTotalScore
        {
            get
            {
                return m_BlackPlayerTotalScore;
            }
        }

        public int WhitePlayerTotalScore
        {
            get
            {
                return m_WhitePlayerTotalScore;
            }
        }

        public int BoardLength
        {
            get
            {
                return r_BoardLength;
            }
        }

        public OthelloPlayer PlayerOne
        {
            get
            {
                return m_PlayerOne;
            }

            set
            {
                m_PlayerOne = value;
            }
        }

        public OthelloPlayer PlayerTwo
        {
            get
            {
                return m_PlayerTwo;
            }

            set
            {
                m_PlayerTwo = value;
            }
        }

        public bool IsBlackTurn
        {
            get
            {
                return m_IsBlackTurn;
            }

            set
            {
                m_IsBlackTurn = value;
            }
        }

        public bool GameOver
        {
            get
            {
                return m_IsGameOver;
            }

            set
            {
                m_IsGameOver = value;
                if (GameOverUpdate != null)
                {
                    GameOverUpdate(value);
                }
            }
        }

        private bool MinMaxSimulation
        {
            get
            {
                return m_IsMinMaxSimulation;
            }

            set
            {
                m_IsMinMaxSimulation = value;
            }
        }

        public OthelloGameDisk this[int i_Row, int i_Column]
        {
            get
            {
                if (!isDiskInBoardRange(i_Row, i_Column))
                {
                    throw new Exception("Disk row: " + i_Row + " column " + i_Column + " not in board range");
                }

                return m_TheOthelloBoardGame[i_Row - 1, i_Column - 1];
            }

            private set
            {
                if (!isDiskInBoardRange(i_Row, i_Column))
                {
                    throw new Exception("Disk row: " + i_Row + " column " + i_Column + " not in board range");
                }

                m_TheOthelloBoardGame[i_Row - 1, i_Column - 1].DiskColor = value.DiskColor;
                if (DiskUpdate != null && !m_IsMinMaxSimulation)
                {
                    DiskUpdate(i_Row, i_Column, value.DiskColor);
                }
            }
        }

        private bool isDiskInBoardRange(int i_Row, int i_Column)
        {
            return BoardLength >= i_Row && BoardLength >= i_Column && i_Row > 0 && i_Column > 0;
        }

        public OthelloGameLogic(int i_BoardSize, string i_PlayerOneName, bool i_IsPlayerOneIsComputer, string i_IsPlayerTwoName, bool i_IsPlayerTwoIsComputer)
        {
            r_BoardLength = i_BoardSize;
            PlayerOne = new OthelloPlayer(i_PlayerOneName, i_IsPlayerOneIsComputer);
            PlayerTwo = new OthelloPlayer(i_IsPlayerTwoName, i_IsPlayerTwoIsComputer);
            m_TheOthelloBoardGame = new OthelloGameDisk[r_BoardLength, r_BoardLength];

            initBoardStartPosition();
        }

        private void initBoardStartPosition()
        {
            int midOfBoard = BoardLength / 2;
            m_TheOthelloBoardGame[midOfBoard, midOfBoard].DiskColor = eCurrentDiskMode.White;
            m_TheOthelloBoardGame[midOfBoard - 1, midOfBoard].DiskColor = eCurrentDiskMode.Black;
            m_TheOthelloBoardGame[midOfBoard, midOfBoard - 1].DiskColor = eCurrentDiskMode.Black;
            m_TheOthelloBoardGame[midOfBoard - 1, midOfBoard - 1].DiskColor = eCurrentDiskMode.White;
        }

        public OthelloGameLogic(OthelloGameLogic i_GameToCopy)
        {
            r_BoardLength = i_GameToCopy.r_BoardLength;
            m_IsGameOver = i_GameToCopy.m_IsGameOver;
            m_IsBlackTurn = i_GameToCopy.m_IsBlackTurn;
            m_PlayerOne = i_GameToCopy.m_PlayerOne;
            m_PlayerTwo = i_GameToCopy.m_PlayerTwo;
            m_TheOthelloBoardGame = new OthelloGameDisk[r_BoardLength, r_BoardLength];

            for (int row = 1; row <= r_BoardLength; row++)
            {
                for (int column = 1; column <= r_BoardLength; column++)
                {
                    this[row, column] = i_GameToCopy[row, column];
                }
            }
        }

        private void computerMakeMoveDecsion()
        {
            const int k_AiDepth = 2;
            int nextRow, nextColumn;

            aiMinMaxAlgorithm(k_AiDepth, out nextRow, out nextColumn, this);
            makeMove(nextRow, nextColumn, eCurrentDiskMode.Black);
        }

        private int aiMinMaxAlgorithm(int i_Depth, out int o_RowOfBestMove, out int o_ColumnOfBestMove, OthelloGameLogic i_TheGameForNextMove)
        {
            o_RowOfBestMove = -1;
            o_ColumnOfBestMove = -1;
            int rowOfBestMove = -1;
            int columnOfBestMove = -1;
            int returnMaxScore = int.MinValue;
            int blackScore;
            int whiteScore;
            int currentScoreFromRecursiveMove;
            OthelloGameLogic theNextLevelBoard;

            if (!i_TheGameForNextMove.GameOver)
            {
                for (int row = 1; row <= BoardLength; row++)
                {
                    for (int column = 1; column <= BoardLength; column++)
                    {
                        bool isLegalPosition = i_TheGameForNextMove.isLegalPositionForNewDisk(row, column, i_TheGameForNextMove.IsBlackTurn ? eCurrentDiskMode.Black : eCurrentDiskMode.White);
                        if (isLegalPosition)
                        {
                            theNextLevelBoard = new OthelloGameLogic(i_TheGameForNextMove);
                            theNextLevelBoard.MinMaxSimulation = true;

                            if (theNextLevelBoard.IsBlackTurn)
                            {
                                theNextLevelBoard.BlackMakeMove(row, column);
                            }
                            else
                            {
                                theNextLevelBoard.WhiteMakeMove(row, column);
                            }

                            theNextLevelBoard.CalculateScore(out blackScore, out whiteScore);
                            currentScoreFromRecursiveMove = blackScore - whiteScore;

                            if (i_Depth == 0 || theNextLevelBoard.GameOver)
                            {
                                o_RowOfBestMove = row;
                                o_ColumnOfBestMove = column;
                            }
                            else
                            {
                                currentScoreFromRecursiveMove = aiMinMaxAlgorithm(i_Depth - 1, out rowOfBestMove, out columnOfBestMove, theNextLevelBoard);
                            }

                            if (currentScoreFromRecursiveMove > returnMaxScore)
                            {
                                returnMaxScore = currentScoreFromRecursiveMove;
                                o_RowOfBestMove = row;
                                o_ColumnOfBestMove = column;
                            }
                        }
                    }
                }
            }

            return returnMaxScore;
        }

        private bool makeMove(int i_RowLocationToAddDiskOnBoard, int i_ColumnLocationToAddDiskOnBoard, eCurrentDiskMode i_DiskToMove)
        {
            bool isSuccessfullMove = false;
            bool isRightTurn = (IsBlackTurn && i_DiskToMove == eCurrentDiskMode.Black) || (!IsBlackTurn && i_DiskToMove == eCurrentDiskMode.White);
            bool isRightTurnAndNotGameOver = !GameOver && isRightTurn && addNewDiskToBoard(i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard, i_DiskToMove);
            if (isRightTurnAndNotGameOver)
            {
                isSuccessfullMove = !isSuccessfullMove;
                checkGameStatusAfterEndOfTurn();
                while (!GameOver && IsBlackTurn && m_PlayerTwo.IsPlayerComputer && !MinMaxSimulation)
                {
                    computerMakeMoveDecsion();
                }

                if (!m_IsMinMaxSimulation)
                {
                    if (IsBlackTurn && !m_PlayerTwo.IsPlayerComputer)
                    {
                        colorPossibleNextMove(eCurrentDiskMode.Black);
                    }
                    else if(!IsBlackTurn && !m_PlayerOne.IsPlayerComputer)
                    {
                        colorPossibleNextMove(eCurrentDiskMode.White);
                    }
                }
            }

            return isSuccessfullMove;
        }

        private void colorPossibleNextMove(eCurrentDiskMode i_CurrentDisk)
        {
            for (int i = 1; i <= BoardLength; i++)
            {
                for (int j = 1; j <= BoardLength; j++)
                {
                    if (this[i, j].DiskColor == eCurrentDiskMode.NotExist)
                    {
                        if (DiskUpdate != null)
                        {
                            DiskUpdate(i, j, eCurrentDiskMode.NotExist);
                        }
                    }

                    if (isLegalPositionForNewDisk(i, j, i_CurrentDisk))
                    {
                        if (DiskUpdate != null)
                        {
                            DiskUpdate(i, j, eCurrentDiskMode.Green);
                        }
                    }
                }
            }
        }

        public void StartNextRound()
        {
            m_IsBlackTurn = false;
            m_IsGameOver = false;
            int middleOfBoard = BoardLength / 2;
            for (int i = 1; i <= BoardLength; i++)
            {
                for (int j = 1; j <= BoardLength; j++)
                {
                    this[i, j] = new OthelloGameDisk(eCurrentDiskMode.NotExist);
                }
            }

            this[middleOfBoard, middleOfBoard] = new OthelloGameDisk(eCurrentDiskMode.White);
            this[middleOfBoard + 1, middleOfBoard] = new OthelloGameDisk(eCurrentDiskMode.Black);
            this[middleOfBoard + 1, middleOfBoard + 1] = new OthelloGameDisk(eCurrentDiskMode.White);
            this[middleOfBoard, middleOfBoard + 1] = new OthelloGameDisk(eCurrentDiskMode.Black);
        }

        public void AskForRefreshPossibleNextMove()
        {
            if (!m_IsMinMaxSimulation)
            {
                if (IsBlackTurn && !m_PlayerTwo.IsPlayerComputer)
                {
                    colorPossibleNextMove(eCurrentDiskMode.Black);
                }
                else
                {
                    colorPossibleNextMove(eCurrentDiskMode.White);
                }
            }
        }

        public bool BlackMakeMove(int i_RowLocationToAddDiskOnBoard, int i_ColumnLocationToAddDiskOnBoard)
        {
            return makeMove(i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard, eCurrentDiskMode.Black);
        }

        public bool WhiteMakeMove(int i_RowLocationToAddDiskOnBoard, int i_ColumnLocationToAddDiskOnBoard)
        {
            return makeMove(i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard, eCurrentDiskMode.White);
        }

        private void checkGameStatusAfterEndOfTurn()
        {
            IsBlackTurn = !IsBlackTurn;
            eCurrentDiskMode currentColor = IsBlackTurn ? eCurrentDiskMode.Black : eCurrentDiskMode.White;
            bool isPossibleForNextPlayerToPlay = false;

            isPossibleForNextPlayerToPlay = checkIsItPossiableForNextPlayerToPlay(isPossibleForNextPlayerToPlay, currentColor);
            
            if (!isPossibleForNextPlayerToPlay)
            {
                IsBlackTurn = !IsBlackTurn;
                currentColor = IsBlackTurn ? eCurrentDiskMode.Black : eCurrentDiskMode.White;

                isPossibleForNextPlayerToPlay = checkIsItPossiableForNextPlayerToPlay(isPossibleForNextPlayerToPlay, currentColor);
                
                if (!isPossibleForNextPlayerToPlay)
                {
                    updateCalculateScore();
                    GameOver = !GameOver;
                }
            }
        }

        private bool checkIsItPossiableForNextPlayerToPlay(bool i_IsPossibleForNextPlayerToPlay, eCurrentDiskMode i_CurrentColor)
        {
            for (int i = 1; i <= BoardLength && !i_IsPossibleForNextPlayerToPlay; i++)
            {
                for (int j = 1; j <= BoardLength && !i_IsPossibleForNextPlayerToPlay; j++)
                {
                    i_IsPossibleForNextPlayerToPlay = isLegalPositionForNewDisk(i, j, i_CurrentColor);
                }
            }

            return i_IsPossibleForNextPlayerToPlay;
        }

        private bool addNewDiskToBoard(int i_RowLocationToAddDiskOnBoard, int i_ColumnLocationToAddDiskOnBoard, eCurrentDiskMode i_DiskColorToAdd)
        {
            const bool v_IsTheLineFlipable = true;
            bool isCurrentDiskDirectionNeedFliplop = !v_IsTheLineFlipable;
            bool isPossibleToAddDisk = isLegalPositionForNewDisk(i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard, i_DiskColorToAdd);

            if (isPossibleToAddDisk)
            {
                this[i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard] = new OthelloGameDisk(i_DiskColorToAdd);

                for (int numberOfRowRunningOnFor = -1; numberOfRowRunningOnFor <= 1; numberOfRowRunningOnFor++)
                {
                    for (int numberOfColumnRunningOnFor = -1; numberOfColumnRunningOnFor <= 1; numberOfColumnRunningOnFor++)
                    {
                        if (numberOfRowRunningOnFor != 0 || numberOfColumnRunningOnFor != 0)
                        {
                            // FirstLine check if is in board range, 2nd line check if the disk is opposite color
                            isCurrentDiskDirectionNeedFliplop = isDiskInBoardRange(i_RowLocationToAddDiskOnBoard + numberOfRowRunningOnFor, i_ColumnLocationToAddDiskOnBoard + numberOfColumnRunningOnFor);
                            isCurrentDiskDirectionNeedFliplop = isCurrentDiskDirectionNeedFliplop ? (this[i_RowLocationToAddDiskOnBoard + numberOfRowRunningOnFor, i_ColumnLocationToAddDiskOnBoard + numberOfColumnRunningOnFor].GetOppositeDisk() == i_DiskColorToAdd) : isCurrentDiskDirectionNeedFliplop;

                            // Check if we can close line
                            isCurrentDiskDirectionNeedFliplop = isCurrentDiskDirectionNeedFliplop ? checkIfCanCloseLine(i_DiskColorToAdd, i_RowLocationToAddDiskOnBoard + numberOfRowRunningOnFor, i_ColumnLocationToAddDiskOnBoard + numberOfColumnRunningOnFor, numberOfRowRunningOnFor, numberOfColumnRunningOnFor) : isCurrentDiskDirectionNeedFliplop;
                            if (isCurrentDiskDirectionNeedFliplop)
                            {
                                closeLine(i_DiskColorToAdd, i_RowLocationToAddDiskOnBoard + numberOfRowRunningOnFor, i_ColumnLocationToAddDiskOnBoard + numberOfColumnRunningOnFor, numberOfRowRunningOnFor, numberOfColumnRunningOnFor);
                                isCurrentDiskDirectionNeedFliplop = !v_IsTheLineFlipable;
                            }
                        }
                    }
                }
            }

            return isPossibleToAddDisk;
        }

        private bool isLegalPositionForNewDisk(int i_RowLocationToAddDiskOnBoard, int i_ColumnLocationToAddDiskOnBoard, eCurrentDiskMode i_DiskColorToAdd)
        {
            const int k_CurrentCell = 0;
            bool isSurroundingEnviromentMakeItLegal = false;
            bool isLegalRandgeAndNotOccupied = i_DiskColorToAdd != eCurrentDiskMode.NotExist && isDiskInBoardRange(i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard);
            isLegalRandgeAndNotOccupied = isLegalRandgeAndNotOccupied ? (this[i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard].DiskColor == eCurrentDiskMode.NotExist) : isLegalRandgeAndNotOccupied;

            if (isLegalRandgeAndNotOccupied)
            {
                for (int i = -1; i <= 1 && !isSurroundingEnviromentMakeItLegal; i++)
                {
                    for (int j = -1; j <= 1 && !isSurroundingEnviromentMakeItLegal; j++)
                    {
                        if (i != k_CurrentCell || j != k_CurrentCell)
                        {
                            // First line check if is in BoardRange, Second line if the Disk Opposite color so we can close line
                            isSurroundingEnviromentMakeItLegal = isDiskInBoardRange(i_RowLocationToAddDiskOnBoard + i, i_ColumnLocationToAddDiskOnBoard + j);
                            isSurroundingEnviromentMakeItLegal = isSurroundingEnviromentMakeItLegal ? (this[i_RowLocationToAddDiskOnBoard + i, i_ColumnLocationToAddDiskOnBoard + j].GetOppositeDisk() == i_DiskColorToAdd) : isSurroundingEnviromentMakeItLegal;

                            // Check if we can Close line
                            isSurroundingEnviromentMakeItLegal = isSurroundingEnviromentMakeItLegal ? checkIfCanCloseLine(i_DiskColorToAdd, i_RowLocationToAddDiskOnBoard + i, i_ColumnLocationToAddDiskOnBoard + j, i, j) : isSurroundingEnviromentMakeItLegal;
                        }
                    }
                }
            }

            return isSurroundingEnviromentMakeItLegal;
        }

        private bool checkIfCanCloseLine(eCurrentDiskMode i_ColorToClose, int i_RowLocationToAddDiskOnBoard, int i_ColumnLocationToAddDiskOnBoard, int i_RowDirection, int i_ColumnDirection)
        {
            bool canWeCloseLine = false;
            i_RowLocationToAddDiskOnBoard += i_RowDirection;
            i_ColumnLocationToAddDiskOnBoard += i_ColumnDirection;

            while (isDiskInBoardRange(i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard) && !canWeCloseLine && this[i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard].DiskColor != eCurrentDiskMode.NotExist)
            {
                canWeCloseLine = this[i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard].DiskColor == i_ColorToClose;
                i_RowLocationToAddDiskOnBoard += i_RowDirection;
                i_ColumnLocationToAddDiskOnBoard += i_ColumnDirection;
            }

            return canWeCloseLine;
        }

        private void closeLine(eCurrentDiskMode i_ColorToClose, int i_RowLocationToAddDiskOnBoard, int i_ColumnLocationToAddDiskOnBoard, int i_RowDirection, int i_ColumnDirection)
        {
            OthelloGameDisk newDiskToAdd = new OthelloGameDisk();
            newDiskToAdd.DiskColor = i_ColorToClose;
            bool canWeCloseLine = false;

            while (isDiskInBoardRange(i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard) && !canWeCloseLine && this[i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard].DiskColor != eCurrentDiskMode.NotExist)
            {
                canWeCloseLine = this[i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard].DiskColor == i_ColorToClose;
                this[i_RowLocationToAddDiskOnBoard, i_ColumnLocationToAddDiskOnBoard] = newDiskToAdd;
                i_RowLocationToAddDiskOnBoard += i_RowDirection;
                i_ColumnLocationToAddDiskOnBoard += i_ColumnDirection;
            }
        }

        public void CalculateScore(out int o_BlackPlayerScore, out int o_WhitePlayerScore)
        {
            o_BlackPlayerScore = o_WhitePlayerScore = 0;
            const int k_DefaultScorePerCell = 1;

            for (int forRowNumber = 1; forRowNumber <= BoardLength; forRowNumber++)
            {
                for (int forColumnNumber = 1; forColumnNumber <= BoardLength; forColumnNumber++)
                {
                    if (this[forRowNumber, forColumnNumber].DiskColor == eCurrentDiskMode.Black)
                    {
                        o_BlackPlayerScore += MinMaxSimulation ? GetRegionScoreForAiMinMax(forRowNumber, forColumnNumber) : k_DefaultScorePerCell;
                    }
                    else if (this[forRowNumber, forColumnNumber].DiskColor == eCurrentDiskMode.White)
                    {
                        o_WhitePlayerScore += MinMaxSimulation ? GetRegionScoreForAiMinMax(forRowNumber, forColumnNumber) : k_DefaultScorePerCell;
                    }
                }
            }
        }

        public int GetRegionScoreForAiMinMax(int i_RowGetScoreForCell, int i_ColumnGetScoreForCell)
        {
            const int k_InitialPosition = 1;
            const int k_Region4Position = 2;
            const int k_ScoreForRegion3 = 5;
            const int k_ScoreForRegion4 = -5;
            const int k_ScoreForRegion5 = 100;
            int calculateScore = 0;

            if (isScoreForRegion5(i_RowGetScoreForCell, i_ColumnGetScoreForCell, k_InitialPosition))
            {
                calculateScore = k_ScoreForRegion5;
            }
            else if (isScoreForRegion4(i_RowGetScoreForCell, i_ColumnGetScoreForCell, k_InitialPosition, k_Region4Position))
            {
                calculateScore = k_ScoreForRegion4;
            }
            else if (isScoreForRegion3(i_RowGetScoreForCell, i_ColumnGetScoreForCell, k_InitialPosition))
            {
                calculateScore = k_ScoreForRegion3;
            }

            return calculateScore;
        }

        private bool isScoreForRegion3(int i_RowGetScoreForCell, int i_ColumnGetScoreForCell, int k_InitialPosition)
        {
            return i_ColumnGetScoreForCell == k_InitialPosition ||
                     i_ColumnGetScoreForCell == BoardLength ||
                     i_RowGetScoreForCell == k_InitialPosition ||
                     i_RowGetScoreForCell == BoardLength;
        }

        private bool isScoreForRegion4(int i_RowGetScoreForCell, int i_ColumnGetScoreForCell, int i_InitialPosition, int i_Region4Position)
        {
            return (i_ColumnGetScoreForCell == i_Region4Position && i_RowGetScoreForCell == i_InitialPosition) ||
                   (i_ColumnGetScoreForCell == i_InitialPosition && i_RowGetScoreForCell == i_Region4Position) ||
                    (i_ColumnGetScoreForCell == i_Region4Position && i_RowGetScoreForCell == i_Region4Position) ||
                    (i_ColumnGetScoreForCell == BoardLength - i_InitialPosition && i_RowGetScoreForCell == i_InitialPosition) ||
                    (i_ColumnGetScoreForCell == BoardLength && i_RowGetScoreForCell == i_Region4Position) ||
                    (i_ColumnGetScoreForCell == BoardLength - i_InitialPosition && i_RowGetScoreForCell == i_Region4Position) ||
                    (i_ColumnGetScoreForCell == i_InitialPosition && i_RowGetScoreForCell == BoardLength - i_InitialPosition) ||
                    (i_ColumnGetScoreForCell == i_Region4Position && i_RowGetScoreForCell == BoardLength) ||
                    (i_ColumnGetScoreForCell == i_Region4Position && i_RowGetScoreForCell == BoardLength - i_InitialPosition) ||
                    (i_ColumnGetScoreForCell == BoardLength && i_RowGetScoreForCell == BoardLength - i_InitialPosition) ||
                    (i_ColumnGetScoreForCell == BoardLength - i_InitialPosition && i_RowGetScoreForCell == BoardLength) ||
                    (i_ColumnGetScoreForCell == BoardLength - i_InitialPosition && i_RowGetScoreForCell == BoardLength - i_InitialPosition);
        }

        private bool isScoreForRegion5(int i_RowGetScoreForCell, int i_ColumnGetScoreForCell, int i_InitialPosition)
        {
            return (i_RowGetScoreForCell == i_InitialPosition && i_ColumnGetScoreForCell == i_InitialPosition) ||
                (i_RowGetScoreForCell == BoardLength && i_ColumnGetScoreForCell == i_InitialPosition) ||
                (i_RowGetScoreForCell == i_InitialPosition && i_ColumnGetScoreForCell == BoardLength) ||
                (i_RowGetScoreForCell == BoardLength && i_ColumnGetScoreForCell == BoardLength);
        }

        private void updateCalculateScore()
        {
            int blackPlayerScore;
            int whitePlayerScore;
            CalculateScore(out blackPlayerScore, out whitePlayerScore);

            if (blackPlayerScore > whitePlayerScore)
            {
                m_BlackPlayerTotalScore++;
            }
            else if(whitePlayerScore > blackPlayerScore)
            {
                m_WhitePlayerTotalScore++;
            }
            else
            {
                m_WhitePlayerTotalScore++;
                m_BlackPlayerTotalScore++;
            }    
        }
    }
}