using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    class Cell : INotifyPropertyChanged
    {
        public SolidColorBrush CellColor { get; private set; }
        public int Age { get; set; }

        private bool isAlive;
        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;
                if (isAlive)
                {
                    CellColor = Brushes.White;
                    Age = 1;
                }
                else
                {
                    CellColor = Brushes.Black;
                    Age = 0;
                }   
                OnPropertyChanged("CellColor");//Update the cell color when cell state is updated
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Cell()
        {
            isAlive = false; //initially the cell is dead
            Age = 0;
        }

        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
