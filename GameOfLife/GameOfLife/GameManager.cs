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

        public Board Board { get; set; }
        public bool IsGameRunning { get; set; }
        public int Time { get; set; }

        private Thread thread;
        private bool isPaused = false;

        private MainWindow mw;

        ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        ManualResetEvent _pauseEvent = new ManualResetEvent(true);

        public GameManager(MainWindow mw)
        {
            this.mw = mw;
            //Board = new Board(INITIAL_NB_CELL_X, INITIAL_NB_CELL_Y);
            preGenerateGrid();
            Board.AleaInit();

            IsGameRunning = false;
            Time = 100;
        }

        private void preGenerateGrid()
        {
            Board = new Board(INITIAL_NB_CELL_X, INITIAL_NB_CELL_Y);
            mw.GraphicalBoard = new Button[100, 100];

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Button cell = new Button();

                    Binding bindingCellColor = new Binding("CellColor");
                    bindingCellColor.Source = Board[i, j];
                    cell.SetBinding(Button.BackgroundProperty, bindingCellColor);

                    cell.Click += new RoutedEventHandler(CellClick);

                    mw.GraphicalBoard[i, j] = cell;
                }
            }
        }

        public void CellClick(object sender, RoutedEventArgs e)
        {
            Button currentCell = sender as Button;
            int iCol = Grid.GetColumn(currentCell);
            int iRow = Grid.GetRow(currentCell);
            Board[iCol, iRow].IsAlive = !Board[iCol, iRow].IsAlive;
        }

        public void UpdateBoard(int nbCellX, int nbCellY)
        {
            Board.NbCellX = nbCellX;
            Board.NbCellY= nbCellY;
            mw.GenerateGrid();
        }

        private void ThreadMethod()
        {
            while(true)
            {
                _pauseEvent.WaitOne(Timeout.Infinite);

                if (_shutdownEvent.WaitOne(0))
                {
                    break;
                }

                mw.AddValueToGraph(Board.NbAliveCells);

                Board.NextIteration();
                System.Threading.Thread.Sleep(Time);
            }
        }

        public void Play()
        {
            if(isPaused == true)
            {
                _pauseEvent.Set();
                isPaused = false;
            }
            else
            {
                thread = new Thread(ThreadMethod);
                thread.Start();
            }
        }

        public void Pause()
        {
            _pauseEvent.Reset();
            isPaused = true;
        }

        public void SaveBoard(string filename)
        {
            using (Stream stream = File.Open(filename, FileMode.Create)) 
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, Board.to2dArray());
            }
        }

        public void RestoreBoard(string filename, Grid boardGrid)
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
            mw.GenerateGrid();
        }
    }
}
