using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameManager gm;

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            gm = new GameManager(20, 10);
            GenerateGrid();
        }

        private void UpdateGrid(int nbCellX, int nbCellY)
        {
            gm.UpdateBoard(nbCellX, nbCellY);
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Grid g = this.FindName("BoardGrid") as Grid;
            g.Children.Clear();
            g.RowDefinitions.Clear();
            g.ColumnDefinitions.Clear();

            Board b = gm.Board;

            for (int i = 0; i < b.NbCellX; i++)
            {
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int j = 0; j < b.NbCellY; j++)
            {
                BoardGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < b.NbCellX; i++)
            {
                for (int j = 0; j < b.NbCellY; j++)
                {
                    Button cell = new Button();

                    Binding bindingCellColor = new Binding("CellColor");
                    bindingCellColor.Source = b[i, j];
                    cell.SetBinding(Button.BackgroundProperty, bindingCellColor);

                    cell.Click += new RoutedEventHandler(Cell_Click);

                    BoardGrid.Children.Add(cell);
                    Grid.SetColumn(cell, i);
                    Grid.SetRow(cell, j);
                }
            }
        }

        public void ButtonPlayClick(object sender, RoutedEventArgs e)
        {

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4, 4, 5, 6, 3, 5, 7, 8, 23, 45, 2, 3, 4, 12, 45 }
                }
            };

            YFormatter = value => value.ToString("C");
            DataContext = this;

            gm.IsGameRunning = true;
            gm.Play();           
        }
        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            Button currentCell = sender as Button;
            gm.Board[Grid.GetColumn(currentCell), Grid.GetRow(currentCell)].IsAlive = !gm.Board[Grid.GetColumn(currentCell), Grid.GetRow(currentCell)].IsAlive;
        }

        public void ButtonPauseClick(object sender, RoutedEventArgs e)
        {
            gm.Pause();
        }

        public void ButtonStopClick(object sender, RoutedEventArgs e)
        {
            gm.Stop();
        }

        /// <summary>
        /// Edit time value when slider emit an ValueChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(gm != null)
            {
                gm.Time = (int)e.NewValue;
            }
        }

        /// <summary>
        /// Edit grid and give the possibility to user to edit the board manually
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CustomizedRadioButton(object sender, RoutedEventArgs e)
        {
            gm?.Clear();
        }

        /// <summary>
        /// Edit the board by reload a random grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RandomRadioButton(object sender, RoutedEventArgs e)
        {
            gm?.AleaInit();
        }

        public void SliderWidthValueChanged(object sender, DragCompletedEventArgs e)
        {
            if (gm != null)
            {
                this.UpdateGrid((int)((Slider)sender).Value, gm.Board.NbCellY);
            }

        }

        public void SliderHeightValueChanged(object sender, DragCompletedEventArgs e)
        {
            if (gm != null)
            {
                this.UpdateGrid(gm.Board.NbCellX, (int)((Slider)sender).Value);
            }
        }
    }
}
