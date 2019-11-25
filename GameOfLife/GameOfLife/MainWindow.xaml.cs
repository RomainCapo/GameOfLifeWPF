using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameManager gm;

        public MainWindow()
        {
            InitializeComponent();
            gm = new GameManager(20, 10);
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            Board b = gm.Board;
            for(int i = 0; i <b.NbCellX; i++)
            {
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for(int j = 0; j < b.NbCellY; j++)
            {
                BoardGrid.RowDefinitions.Add(new RowDefinition());
            }

            for(int i = 0; i < b.NbCellX; i++)
            {
                for(int j = 0; j < b.NbCellY; j++)
                {
                    BoardGrid.Children.Add(b[i,j]);
                    Grid.SetColumn(b[i,j], i);
                    Grid.SetRow(b[i,j], j);
                }
            }
        }

        public void ButtonPlayClick(object sender, RoutedEventArgs e)
        {
            /*gm.IsGameRunning = true;
            gm.Play();*/
            MessageBox.Show("Play!");
        }

        public void ButtonPauseClick(object sender, RoutedEventArgs e)
        {
            //gm.IsGameRunning = false;
            MessageBox.Show("Pause!");
        }

        public void ButtonStopClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Stop!");
        }

        /// <summary>
        /// Edit time value when slider emit an ValueChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MessageBox.Show("Value changed!");
        }

        /// <summary>
        /// Edit grid and give the possibility to user to edit the board manually
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CustomizedRadioButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Customize board!");
        }

        /// <summary>
        /// Edit the board by reload a random grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RandomRadioButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Random board!");
        }
    }
}
