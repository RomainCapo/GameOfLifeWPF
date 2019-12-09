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
        public GameManager(int x, int y)
        {
            Board = new Board(x, y);
            Board.AleaInit();
            IsGameRunning = false;
        }

        public void UpdateBoard(int nbCellX, int nbCellY)
        {
            Board = new Board(nbCellX, nbCellY);
        }

        private void ThreadMethod()
        {
            while(true)
            {
                Board.NextIteration();
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void Play()
        {
            ThreadStart ts = new ThreadStart(ThreadMethod);
            Thread thread = new Thread(ts);
            thread.Start();
        }

        public void Pause()
        {

        }
    }

}
