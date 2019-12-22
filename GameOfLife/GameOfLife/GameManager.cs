using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GameOfLife
{
    class GameManager
    {
        private const int INITIAL_NB_CELL_X = 20;
        private const int INITIAL_NB_CELL_Y = 10;

        /// <summary>
        /// maximum number of cell on x (for precompute cell array)
        /// </summary>
        private const int MAX_BOARD_SIZE_X = 100;

        /// <summary>
        /// maximum number of cell on y (for precompute cell array)
        /// </summary>
        private const int MAX_BOARD_SIZE_Y = 100;

        public Board Board { get; set; }
        private MainWindow mw;

        private Thread animationThread;

        private bool isPaused = false;
        public int IterationInterval { get; set; }

        ManualResetEvent pauseEvent = new ManualResetEvent(true);

        /// <summary>
        /// GameManager constructor
        /// </summary>
        /// <param name="mw">MainWindow object</param>
        public GameManager(MainWindow mw)
        {
            this.mw = mw;
            InitBoard();
            Board.AleaInit();

            IterationInterval = 100;
        }

        /// <summary>
        /// Init the model Board and the graphical board with the maximum value for x and y.
        /// This operation is performed in order to precompute the array to increase performance.
        /// </summary>
        private void InitBoard()
        {
            Board = new Board(INITIAL_NB_CELL_X, INITIAL_NB_CELL_Y, MAX_BOARD_SIZE_X, MAX_BOARD_SIZE_Y);
            mw.GraphicalBoard = new Button[MAX_BOARD_SIZE_X, MAX_BOARD_SIZE_Y];

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    //Bind the CellColor property of the celle object with the graphical button backgroung property
                    Button cell = new Button();

                    Binding bindingCellColor = new Binding("CellColor");
                    bindingCellColor.Source = Board[i, j];
                    cell.SetBinding(Button.BackgroundProperty, bindingCellColor);

                    cell.Click += new RoutedEventHandler(mw.CellClick);//Bind the clic event to each button

                    mw.GraphicalBoard[i, j] = cell;
                }
            }
        }

        /// <summary>
        /// Update the board size
        /// </summary>
        /// <param name="nbCellX">new x size</param>
        /// <param name="nbCellY">new y size</param>
        public void UpdateBoard(int nbCellX, int nbCellY)
        {
            Board.NbCellX = nbCellX;
            Board.NbCellY= nbCellY;
            mw.UpdateGrid();
        }

        /// <summary>
        /// Reset game function.
        /// Pause the game, clear the plot and the board.
        /// </summary>
        public void ResetGame()
        {
            Pause();
            mw.ClearPlot();
            Board.Clear();
            Board.BoardStatistics.ResetStatistics();
            mw.txtStats.Text = "";
        }

        /// <summary>
        /// Simulation thread. 
        /// Compute the next board iteration. 
        /// Stop the thread if the pause event is run or if the simulation is ended.
        /// </summary>
        private void ThreadMethod()
        {
            while(true)
            {
                pauseEvent.WaitOne(Timeout.Infinite);
                
                mw.AddValueToGraph(Board.BoardStatistics.NbAliveCells);
                mw.AddValueToHisto(Board.BoardStatistics.ValuesHisto());

                Board.NextIteration();
                mw.Dispatcher.Invoke(() =>
                {
                    mw.txtStats.Text = Board.BoardStatistics.DisplayStatistics();
                });
                

                if (Board.IsEnd)
                {
                    MessageBox.Show("Game is ended");
                    mw.Dispatcher.Invoke(() =>
                    {
                        ResetGame();
                        mw.EnableInterface(true);
                    });
                }
                Thread.Sleep(IterationInterval);
            }
        }

        /// <summary>
        /// Run the simulation 
        /// </summary>
        public void Play()
        {
            if(isPaused == true)
            {
                pauseEvent.Set();
                isPaused = false;
            }
            else
            {
                animationThread = new Thread(ThreadMethod);
                animationThread.Start();
            }
        }

        /// <summary>
        /// Pause the simulation
        /// </summary>
        public void Pause()
        {
            pauseEvent.Reset();
            isPaused = true;
        }

        /// <summary>
        /// Save the board state. The board state is represented by a 2D array of int.
        /// A binary stream is used to serialize the array.
        /// </summary>
        /// <param name="filename">filname of the file</param>
        public void SaveBoard(string filename)
        {
            using (Stream stream = File.Open(filename, FileMode.Create)) 
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, Board.to2dArray());
            }
        }

        /// <summary>
        /// Restore the board state from a 2D int array. If the file can't be open open a exception is raise.
        /// </summary>
        /// <param name="filename">filename of the file</param>
        public void RestoreBoard(string filename)
        {

            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                try
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    Board.from2dArray((int[,])binaryFormatter.Deserialize(stream));
                }
                catch
                {
                    MessageBox.Show("The file cannot be read !");
                }
            }
            mw.UpdateGrid();//Update the grid after load the board
        }
    }
}
