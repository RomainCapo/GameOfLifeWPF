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
            Board = new Board(INITIAL_NB_CELL_X, INITIAL_NB_CELL_Y);
            Board.AleaInit();

            IsGameRunning = false;
            Time = 100;
        }
        public void UpdateBoard(int nbCellX, int nbCellY)
        {
            Board = new Board(nbCellX, nbCellY);
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
