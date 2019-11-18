using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    class Cell:Button
    {
        private bool isAlive;
        public bool IsAlive { 
            get
            {
                return isAlive;
            }
            set
            {
                if (value)
                {
                    isAlive = value;
                    this.Background = Brushes.White;
                }
                else
                {
                    isAlive = value;
                    this.Background = Brushes.Black;
                }
            }
        }

        public Cell():base()
        {
            IsAlive = false;
        }
    }
}
