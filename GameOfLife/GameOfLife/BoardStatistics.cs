using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class BoardStatistics : INotifyPropertyChanged
    {
        private Board board;
        public event PropertyChangedEventHandler PropertyChanged;

        private int[] histo;

        /// <summary>
        /// Number of iteration for the current board
        /// </summary>
        public int NumberIteration { get; private set; }

        /// <summary>
        /// Number of alive cell in the board
        /// </summary>
        public double NbAliveCells
        {
            get
            {
                double sum = 0;

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

        public int[] ValuesHisto()
        {
            int[] histo = new int[MaxAge];

            for (int i = 0; i < board.NbCellX; i++)
            {
                for (int j = 0; j < board.NbCellY; j++)
                {
                    histo[board[i, j].Age]++;
                }
            }

            return histo;
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxPopulation
        {
            get
            {
                int max = int.MinValue;
                for(int i = 0; i < histo.Length; i++)
                {
                    if(histo[i]>max)
                    {
                        max = histo[i];
                    }
                }
                return max;
            }
        }

        public int MinPopulation
        {
            get
            {
                int min = int.MaxValue;
                for (int i = 0; i < histo.Length; i++)
                {
                    if (histo[i] < min)
                    {
                        min = histo[i];
                    }
                }
                return min;
            }
        }

        /// <summary>
        /// BoardStatistics Iteration
        /// </summary>
        /// <param name="board">Board object</param>
        public BoardStatistics(Board board)
        {
            this.board = board;
            NumberIteration = 0;
        }

        /// <summary>
        /// Increment iteration number
        /// </summary>
        public void IncIteration()
        {
            NumberIteration++;
        }

        public void ResetStatistics()
        {
            NumberIteration = 0;

        }

        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
