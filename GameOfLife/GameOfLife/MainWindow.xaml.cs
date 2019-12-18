using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameManager gm;

        public Button[,] GraphicalBoard { get; set; }

        public SeriesCollection SeriesCollection { get; private set; }
        public Func<double, string> YFormatter { get; private set; }
        private double precedentValueGraph;

        public MainWindow()
        {
            InitializeComponent();

            gm = new GameManager(this);
            UpdateGrid();

            InitPlot();
        }

        private void InitPlot()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Number of Cells : ",
                    Values = new ChartValues<double> {},
                    Stroke = Brushes.Red,
                    Fill = Brushes.Transparent,
                    Opacity = 0.5
                }
            };

            DataContext = this;
            precedentValueGraph = 0;
        }

        public void UpdateGrid()
        {
            Grid boardGrid = this.FindName("BoardGrid") as Grid;

            boardGrid.Children.Clear();
            boardGrid.RowDefinitions.Clear();
            boardGrid.ColumnDefinitions.Clear();

            Board b = gm.Board;

            for (int i = 0; i < b.NbCellX; i++)
            {
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int j = 0; j < b.NbCellY; j++)
            {
                boardGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < b.NbCellX; i++)
            {
                for (int j = 0; j < gm.Board.NbCellY; j++)
                {
                    Button cell = GraphicalBoard[i, j];

                    boardGrid.Children.Add(cell);
                    Grid.SetColumn(cell, i);
                    Grid.SetRow(cell, j);
                }
            }
        }

        public void CellClick(object sender, RoutedEventArgs e)
        {
            Button currentCell = sender as Button;
            int iCol = Grid.GetColumn(currentCell);
            int iRow = Grid.GetRow(currentCell);
            gm.Board[iCol, iRow].IsAlive = !gm.Board[iCol, iRow].IsAlive;
        }

        private void EnableInterface(bool isEnabled)
        {
            (this.FindName("IntegerUpDownWidth") as IntegerUpDown).IsEnabled = isEnabled;
            (this.FindName("IntegerUpDownHeight") as IntegerUpDown).IsEnabled = isEnabled;
            (this.FindName("ButtonRandom") as Button).IsEnabled = isEnabled;
            (this.FindName("ButtonClear") as Button).IsEnabled = isEnabled;
            (this.FindName("ButtonSave") as Button).IsEnabled = isEnabled;
            (this.FindName("ButtonRestore") as Button).IsEnabled = isEnabled;
        }

        public void ButtonPlayClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);
            gm.Play();           
        }
        

        public void ButtonPauseClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(true);
            gm.Pause();
        }

        public void ButtonStopClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(true);
            gm.ResetGame();
        }

        public void ClearPlot()
        {
            SeriesCollection[0].Values.Clear();
        }

        public void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = "BoardState",
                DefaultExt = ".gol",
                Filter ="Board state file (.gol)|*.gol"
            };
            
            if(dlg.ShowDialog() == true)
            {
                gm.SaveBoard(dlg.FileName);
            }
        }

        public void ButtonRestoreClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".gol",
                Filter = "Board state file (.gol)|*.gol"

            };

            if(dlg.ShowDialog() == true)
            {
                gm.RestoreBoard(dlg.FileName, BoardGrid);
            }
            
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
                gm.IterationInterval = (int)e.NewValue;
            }
        }

        public void ButtonClearClick(object sender, RoutedEventArgs e)
        {
            gm?.Board.Clear();
        }


        public void ButtonRandomClick(object sender, RoutedEventArgs e)
        {
            gm?.Board.Clear();
            gm?.Board.AleaInit();
        }

        public void IntegerUpDownWidthValueChanged(object sender, RoutedEventArgs e)
        {
            if (gm != null)
            {
                gm.UpdateBoard((int)((IntegerUpDown)sender).Value, gm.Board.NbCellY);
                gm.Board.AleaInit();
            }
        }

        public void IntegerUpDownHeightValueChanged(object sender, RoutedEventArgs e)
        {
            if (gm != null)
            {
                gm.UpdateBoard(gm.Board.NbCellX, (int)((IntegerUpDown)sender).Value);
                gm.Board.AleaInit();
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        public void AddValueToGraph(double value)
        {
            if (!value.Equals(precedentValueGraph))
            {
                SeriesCollection[0].Values.Add(value);
                precedentValueGraph = value;
            }
        }
    }
}
