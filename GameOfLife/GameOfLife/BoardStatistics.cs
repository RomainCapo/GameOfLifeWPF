

namespace GameOfLife
{
    class BoardStatistics
    {
        private Board board;

        /// <summary>
        /// Number of iteration for the current board
        /// </summary>
        public int NumberIteration { get; private set; }

        /// <summary>
        /// Number of alive cell in the board
        /// </summary>
        public int NbAliveCells
        {
            get
            {
                int sum = 0;

                for (int i = 0; i < board.NbCellX; i++)
                {
                    for (int j = 0; j < board.NbCellY; j++)
                    {
                        if (board[i, j].IsAlive)
                        {
                            sum++;
                        }
                    }
                }

                return sum;
            }
        }

        /// <summary>
        /// Maximum age of board cell
        /// </summary>
        public int MaxAge
        {
            get
            {
                int max = 0;
                for (int i = 0; i < board.NbCellX; i++)
                {
                    for (int j = 0; j < board.NbCellY; j++)
                    {
                        if (board[i, j].Age > max)
                        {
                            max = board[i, j].Age;
                        }
                    }
                }

                return max;
            }
        }

        /// <summary>
        /// Compute the histogramm for the current board
        /// </summary>
        /// <returns>histogramm in int array</returns>
        public int[] ValuesHisto()
        {
            int[] histo = new int[MaxAge+1];

            for (int i = 0; i < board.NbCellX; i++)
            {
                for (int j = 0; j < board.NbCellY; j++)
                {
                    histo[board[i, j].Age]++;
                }
            }

            return histo;
        }

        
        private int maxPopulation;
        /// <summary>
        /// Maximum number of cell in all itearation
        /// </summary>
        public int MaxPopulation
        {
            get
            {
                int nbAliveCells = NbAliveCells;
                if (nbAliveCells > maxPopulation)
                {
                    maxPopulation = nbAliveCells;
                }
                return maxPopulation;
            }
            private set
            {
                minPopulation = value;
            }
        }

        private int minPopulation;
        /// <summary>
        /// Minimum number of cell in all itearation
        /// </summary>
        public int MinPopulation
        {
            get
            {
                int nbAliveCells = NbAliveCells;
                if (nbAliveCells < minPopulation)
                {
                    minPopulation = nbAliveCells;
                }
                return minPopulation;
            }
            private set
            {
                minPopulation = value;
            }
        }

        /// <summary>
        /// BoardStatistics
        /// </summary>
        /// <param name="board">Board object</param>
        public BoardStatistics(Board board)
        {
            this.board = board;
            ResetStatistics();
        }

        /// <summary>
        /// Increment iteration number
        /// </summary>
        public void IncIteration()
        {
            NumberIteration++;
        }

        /// <summary>
        /// Reset all statistics
        /// </summary>
        public void ResetStatistics()
        {
            NumberIteration = 0;
            minPopulation = int.MaxValue;
            maxPopulation = int.MinValue;
        }

        /// <summary>
        /// Conacatains all stats and display it to string format
        /// </summary>
        /// <returns>statistics in string format</returns>
        public string DisplayStatistics()
        {
            string output = "";
            output += "Oldest cell : " + MaxAge;
            output += ", Actual pop : " + NbAliveCells;
            output += ", Min pop : " + MinPopulation;
            output += ", Max pop : " + MaxPopulation;
            output += ", Number iterations : " + NumberIteration;
           return output;
        }
    }
}
