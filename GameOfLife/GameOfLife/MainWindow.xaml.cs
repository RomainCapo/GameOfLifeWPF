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
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace GameOfLife
{
    /// <summary>
    /// Réglage interactif de la vitesse ainsi que des dimensions de la simulation
    /// Option de pause/Play et Réinitialisation de la simulation
    /// Etat de départ aléatoire ou au choix de l'utilisateur
    /// Affichage d'un graphe avec la population (nombre de celllules) selon l'itération
    /// Chargement/Sauvegarde d'un état de la simulation
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameManager gm;

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private double PrecedentValueGraph { get; set; }
        private int NumberOfIteration { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            gm = new GameManager(20, 10, this);

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
            PrecedentValueGraph = 0;
            NumberOfIteration = 0;

            gm.GenerateGrid(this.FindName("BoardGrid") as Grid);
        }

        private void UpdateGrid(int nbCellX, int nbCellY)
        {
            gm.UpdateBoard(nbCellX, nbCellY);
            gm.GenerateGrid(this.FindName("BoardGrid") as Grid);
        }

        

        private void EnableInterface(bool isEnabled)
        {
            (this.FindName("IntegerUpDownWidth") as IntegerUpDown).IsEnabled = isEnabled;
            (this.FindName("IntegerUpDownHeight") as IntegerUpDown).IsEnabled = isEnabled;
            (this.FindName("ButtonRandom") as Button).IsEnabled = isEnabled;
            (this.FindName("ButtonClear") as Button).IsEnabled = isEnabled;
        }

        public void ButtonPlayClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);

            gm.IsGameRunning = true;
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
            gm.Pause();
            gm.Board.Clear();

            SeriesCollection[0].Values.Clear();
            NumberOfIteration = 0;
        }

        public void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            /*SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = "BoardState",
                DefaultExt = ".gol",
                Filter ="Board state file (.gol)|*.gol"
            };
            
            if(dlg.ShowDialog() == true)
            {
                //gm.SaveBoard(dlg.FileName);
            }*/

            gm.SaveBoard();
        }

        public void ButtonRestoreClick(object sender, RoutedEventArgs e)
        {
            gm.RestoreBoard(BoardGrid);
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
                this.UpdateGrid((int)((IntegerUpDown)sender).Value, gm.Board.NbCellY);
                gm.Board.AleaInit();
            }
        }

        public void IntegerUpDownHeightValueChanged(object sender, RoutedEventArgs e)
        {
            if (gm != null)
            {
                this.UpdateGrid(gm.Board.NbCellX, (int)((IntegerUpDown)sender).Value);
                gm.Board.AleaInit();
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void AddValueToGraph(double value)
        {
            NumberOfIteration++;

            if (!value.Equals(PrecedentValueGraph))
            {
                SeriesCollection[0].Values.Add(value);
                PrecedentValueGraph = value;
            }

            /*if(NumberOfIteration > 10)
            {
                SeriesCollection[0].Values.RemoveAt(0);
            }*/
        }
    }
}
