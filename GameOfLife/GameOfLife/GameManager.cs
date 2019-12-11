using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GameOfLife
{
    class GameManager
    {
        public Board Board { get; set; }
        public bool IsGameRunning { get; set; }
        public int Time { get; set; }

        private Thread thread;
        private bool isPaused = false;

        ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        ManualResetEvent _pauseEvent = new ManualResetEvent(true);

        public GameManager(int x, int y)
        {
            Board = new Board(x, y);
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

        public void Stop()
        {
            _shutdownEvent.Set();
            _pauseEvent.Set();
            thread.Join();
        }
        public void AleaInit()
        {
            Board.AleaInit();
        }
        public void Clear()
        {
            Board.Clear();
        }
    }

}
