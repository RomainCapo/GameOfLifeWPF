using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class GameManager
    {
        private int width;
        private int height;
        
        public GameManager(int height, int width)
        {
            this.height = height;
            this.width = width;
        }
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
    }
}
