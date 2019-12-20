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

        /// <summary>
        /// Array that containing the graphic buttons
        /// </summary>
        public Button[,] GraphicalBoard { get; set; }

        public SeriesCollection PlotIterationCell { get; private set; }
        public SeriesCollection YearPyramid { get; private set; }

        private double precedentValueGraph;

        public MainWindow()
        {
            InitializeComponent();

            gm = new GameManager(this);

            UpdateGrid();
            InitPlot();



            YearPyramid = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Number of Cells : ",
                    Values = new ChartValues<double> { 10, 50, 39, 50 },
                    Fill = Brushes.Red
                }
            };
        }

        /// <summary>
        /// Init the plot parameters
        /// </summary>
        private void InitPlot()
        {
            PlotIterationCell = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Number of Cells : ",
                    Values = new ChartValues<double> {},
                    Stroke = Brushes.Red,
                    Fill = Brushes.Transparent
                }
            };

            DataContext = this;
            precedentValueGraph = 0;
        }

        /// <summary>
        /// Update the board cell from the precomputed graphical board.
        /// Clear the button in the graphical board, the column definition and the row definition.
        /// Create the new column and row definiton and add the button on the interface.
        /// The button come from the precomputed array of button.
        /// The board is resized with the actual size of the model board.
        /// </summary>
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

        /// <summary>
        /// Cell click event. Invert the celle state.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void CellClick(object sender, RoutedEventArgs e)
        {
            Button currentCell = sender as Button;
            int iCol = Grid.GetColumn(currentCell);
            int iRow = Grid.GetRow(currentCell);
            gm.Board[iCol, iRow].IsAlive = !gm.Board[iCol, iRow].IsAlive;
        }

        /// <summary>
        /// Disable interface component
        /// </summary>
        /// <param name="isEnabled">true for enable interface, false to disable</param>
        public void EnableInterface(bool isEnabled)
        {
            (FindName("IntegerUpDownWidth") as IntegerUpDown).IsEnabled = isEnabled;
            (FindName("IntegerUpDownHeight") as IntegerUpDown).IsEnabled = isEnabled;
            (FindName("ButtonRandom") as Button).IsEnabled = isEnabled;
            (FindName("ButtonClear") as Button).IsEnabled = isEnabled;
            (FindName("ButtonSave") as Button).IsEnabled = isEnabled;
            (FindName("ButtonRestore") as Button).IsEnabled = isEnabled;
        }

        /// <summary>
        /// Play click event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void ButtonPlayClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);
            gm.Play();           
        }

        /// <summary>
        /// Pause click event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void ButtonPauseClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(true);
            gm.Pause();
        }

        /// <summary>
        /// Strop click event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void ButtonStopClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(true);
            gm.ResetGame();
        }

        /// <summary>
        /// Clear the plot value
        /// </summary>
        public void ClearPlot()
        {
            PlotIterationCell[0].Values.Clear();
        }

        /// <summary>
        /// Allow to add a value to the plot
        /// </summary>
        /// <param name="value">double value to add on the plot</param>
        public void AddValueToGraph(double value)
        {
            if (!value.Equals(precedentValueGraph))
            {
                PlotIterationCell[0].Values.Add(value);
                precedentValueGraph = value;
            }
        }

        /// <summary>
        /// Save board click event.
        /// Open a SaveFileDialog for save the board in the file
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
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

        /// <summary>
        /// Open a OpenFileDialog for load the board from a file
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void ButtonRestoreClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".gol",
                Filter = "Board state file (.gol)|*.gol"

            };

            if(dlg.ShowDialog() == true)
            {
                gm.RestoreBoard(dlg.FileName);
            }
            
        }

        /// <summary>
        /// Edit time value when slider emit an ValueChanged
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(gm != null)
            {
                gm.IterationInterval = (int)e.NewValue;
            }
        }

        /// <summary>
        /// Clear button event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void ButtonClearClick(object sender, RoutedEventArgs e)
        {
            gm?.Board.Clear();
        }

        /// <summary>
        /// Random button event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void ButtonRandomClick(object sender, RoutedEventArgs e)
        {
            gm?.Board.Clear();
            gm?.Board.AleaInit();
        }

        /// <summary>
        /// IntegerUpDown for width value change event.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void IntegerUpDownWidthValueChanged(object sender, RoutedEventArgs e)
        {
            if (gm != null)
            {
                gm.UpdateBoard((int)((IntegerUpDown)sender).Value, gm.Board.NbCellY);
                gm.Board.Clear();
            }
        }

        /// <summary>
        /// IntegerUpDown for height value change event.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        public void IntegerUpDownHeightValueChanged(object sender, RoutedEventArgs e)
        {
            if (gm != null)
            {
                gm.UpdateBoard(gm.Board.NbCellX, (int)((IntegerUpDown)sender).Value);
                gm.Board.Clear();
            }
        }

        /// <summary>
        /// Closing event.
        /// It is necessary to kill the process when closing the application
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
