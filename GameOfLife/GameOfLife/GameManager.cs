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

        ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        ManualResetEvent pauseEvent = new ManualResetEvent(true);

        public GameManager(MainWindow mw)
        {
            this.mw = mw;
            Board = new Board(INITIAL_NB_CELL_X, INITIAL_NB_CELL_Y);
            Board.AleaInit();

            IsGameRunning = false;
            Time = 100;
        }
        public void UpdateBoard(int nbCellX, int nbCellY)
        {
            Board = new Board(nbCellX, nbCellY);
            mw.GenerateGrid();
        }

        public void ResetGame()
        {
            Pause();
            mw.ClearPlot();
            Board.Clear();
        }

        private void ThreadMethod()
        {
            while(true)
            {
                pauseEvent.WaitOne(Timeout.Infinite);

                if (shutdownEvent.WaitOne(0))
                    break;
                
                mw.AddValueToGraph(Board.NbAliveCells);

                Board.NextIteration();

                if (Board.isEnd)
                {
                    MessageBox.Show("Game is ended");
                    ResetGame();
                }
                Thread.Sleep(Time);
            }
        }

        public void Play()
        {
            if(isPaused == true)
            {
                pauseEvent.Set();
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
            pauseEvent.Reset();
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
