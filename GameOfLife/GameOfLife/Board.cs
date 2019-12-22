using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GameOfLife
{

    class Board
    {
        Cell[,] board;

        /// <summary>
        /// Current number of cell on x
        /// </summary>
        public int NbCellX { get; set; }

        /// <summary>
        /// Current number of cell on y
        /// </summary>
        public int NbCellY { get; set; }

        /// <summary>
        /// Indicated if the game is finish
        /// </summary>
        public bool isEnd { get; private set; }

        /// <summary>
        /// maximum number of cell on x (for precompute cell array)
        /// </summary>
        private readonly int MAX_BOARD_SIZE_X;

        /// <summary>
        /// maximum number of cell on y (for precompute cell array)
        /// </summary>
        private readonly int MAX_BOARD_SIZE_Y;

        /// <summary>
        /// Get the Number of alive cell on the current board
        /// </summary>
        public double NbAliveCells
        {
            get
            {
                double sum = 0;

                for (int i = 0; i < NbCellX; i++)
                {
                    for (int j = 0; j < NbCellY; j++)
                    {
                        if(board[i, j].IsAlive)
                        {
                            sum++;
                        }
                    }
                }

                return sum;
            }
        }

        Random aleaRand = new Random();

        /// <summary>
        /// Board constructor   
        /// </summary>
        /// <param name="nbCellX">number of current cell on x</param>
        /// <param name="nbCellY">number of current cell on y</param>
        /// <param name="maxCellX">maximum number of cell on x (for precompute cell array)</param>
        /// <param name="maxCellY">maximum number of cell on y (for precompute cell array)</param>
        public Board(int nbCellX, int nbCellY, int maxCellX, int maxCellY)
        {
            NbCellX = nbCellX;
            NbCellY = nbCellY;
            MAX_BOARD_SIZE_X = maxCellX;
            MAX_BOARD_SIZE_Y = maxCellY;
            board = new Cell[MAX_BOARD_SIZE_X, MAX_BOARD_SIZE_Y];
            InitBoard();
        }

        public int MaxAge()
        {
            int max = 0;
            for(int i = 0; i < NbCellX; i++)
            {
                for(int j = 0; j < NbCellY; j++)
                {
                    if (board[i, j].Age > max)
                    {
                        max = board[i, j].Age;
                    }
                }
            }

            return max;
        }

        public int[] ValuesHisto()
        {
            int[] values = new int[MaxAge() + 1];

            for (int i = 0; i < NbCellX; i++)
            {
                for (int j = 0; j < NbCellY; j++)
                {
                    values[board[i, j].Age]++;
                }
            }

            return values;
        }

        /// <summary>
        /// Indexer for the internal board array
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>board cell on (x,y) coordinate</returns>
        public Cell this[int x, int y]
        {
            get { return board[x, y]; }
            set
            {
                if (x >= 0 && x <= NbCellX && y >= 0 && y <= NbCellY)
                {
                    board[x, y] = value;
                }
            }
        }

        /// <summary>
        /// Init the board with the maximum allowed size.
        /// This operation is performed in order to precompute the array to increase performance.
        /// </summary>
        private void InitBoard()
        {
            for (int i = 0; i < MAX_BOARD_SIZE_X; i++)
            {
                for (int j = 0; j < MAX_BOARD_SIZE_Y; j++)
                {
                    board[i, j] = new Cell();
                }
            }
        }

        /// <summary>
        /// Export the cell board in 2D array.
        /// IsAlive = true = 1
        /// IsAlvie = false = 0
        /// </summary>
        /// <returns>2D array of int which represent the board</returns>
        public int[,] to2dArray()
        {
            int[,] intBoard = new int[NbCellX, NbCellY];
            for(int i = 0; i < NbCellX; i++)
            {
                for(int j = 0; j < NbCellY; j++)
                {
                    intBoard[i, j] = Convert.ToInt32(board[i, j].IsAlive);
                }
            }
            return intBoard;
        }

        /// <summary>
        /// Load the board from a 2D array of int
        /// IsAlive = true = 1
        /// IsAlvie = false = 0
        /// </summary>
        /// <param name="intBoard">2D array of int which represent the board</param>
        public void from2dArray(int[,] intBoard)
        {
            NbCellX = intBoard.GetLength(0);
            NbCellY = intBoard.GetLength(1);

            for (int i = 0; i < NbCellX; i++)
            {
                for(int j = 0; j < NbCellY; j++)
                {
                    board[i, j].IsAlive = Convert.ToBoolean(intBoard[i, j]);
                }
            }
        }

        /// <summary>
        /// Clear the current board cell
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < NbCellX; i++)
            {
                for (int j = 0; j < NbCellY; j++)
                {
                    board[i, j].IsAlive = false;
                    board[i, j].Age = 0;
                }
            }
        }

        /// <summary>
        /// Init the board with cell positioned randomly.
        /// Iterate over each column on the x axis and init the column with a number of cell from 0 to maximum of cell on the y axis.
        /// Choose the index randomly on the column.
        /// </summary>
        public void AleaInit()
        {
            int rand;
            Clear();
            for (int i = 0; i < NbCellX; i++)
            {
                for (int j = 0; j < aleaRand.Next(0, NbCellY); j++)
                {
                    board[i, aleaRand.Next(0, NbCellY)].IsAlive = true;
                }
            }
        }

        /// <summary>
        /// Compute the number of neighboor for a (x,y) cell
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>The number of neighboor of the given cell</returns>
        public int ComputeCellXYNeighbours(int x, int y)
        {
            int xToTest;
            int yToTest;
            int nbNeighbours = 0;

            //iterate over the neighboor
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    xToTest = x + i;
                    yToTest = y + j;
                    
                    //check if the neighboor is in the board
                    if ((xToTest >= 0 && yToTest >= 0) && (xToTest < NbCellX && yToTest < NbCellY))
                    {
                        if(board[xToTest, yToTest].IsAlive)
                        {
                            nbNeighbours++;
                        }
                    }
                }
            }
            //remove 1 is the given cell is alive
            if (board[x, y].IsAlive)
                nbNeighbours--;
            return nbNeighbours;
        }

        /// <summary>
        /// Compute the number of neighboor for each cell in the current board.
        /// This operation is necessary to have a syncrone simulation.
        /// </summary>
        /// <returns>2D int array with the value of neighboor for each cell</returns>
        private int[,] ComputeBoardNeighbours()
        {
            int[,] boardNeighboor = new int[NbCellX, NbCellY];
            for (int i = 0; i < NbCellX; i++)
            {
                for (int j = 0; j < NbCellY; j++)
                {
                    boardNeighboor[i, j] = ComputeCellXYNeighbours(i, j);
                }
            }
            return boardNeighboor;
        }

        /// <summary>
        /// Compute the next board state.
        /// The simulation rules are implemented here.
        /// If no changes are made the simulation is finish 
        /// </summary>
        public void NextIteration()
        {
            isEnd = true;
            int[,] boardNeighbours = ComputeBoardNeighbours();//get the neighboor array

            for (int i = 0; i < NbCellX; i++)
            {
                for (int j = 0; j < NbCellY; j++)
                {
                    int nbNeighbours = boardNeighbours[i, j];
                    if (board[i, j].IsAlive)
                    {
                        board[i, j].Age++;

                        if (nbNeighbours < 2)
                        {
                            board[i, j].IsAlive = false;
                            isEnd = false;
                        }

                        if (nbNeighbours > 3)
                        {
                            board[i, j].IsAlive = false;
                            isEnd = false;
                        }
                    }
                    else
                    {
                        if (nbNeighbours == 3)
                        {
                            board[i, j].IsAlive = true;
                            isEnd = false;
                        }
                    }
                }
            }
        }
    }
}
