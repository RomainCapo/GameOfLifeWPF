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
    [Serializable]
    class Cell : INotifyPropertyChanged
    {
        public SolidColorBrush CellColor { get; private set; }

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
                }
                else
                {
                    CellColor = Brushes.Black;
                }   
                OnPropertyChanged("CellColor");
            }
        }

        public Cell()
        {
            isAlive = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


    }
}
