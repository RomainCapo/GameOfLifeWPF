using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameOfLife
{
    class GameManager
    {
        public Board Board { get; }
        public bool IsGameRunning { get; set; }
        public GameManager(int x, int y)
        {
            Board = new Board(x, y);
            Board.AleaInit();
            IsGameRunning = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            while (IsGameRunning)
            {
                Board.NextIteration();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

}
