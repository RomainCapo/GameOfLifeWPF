using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

        public MainWindow()
        {
            InitializeComponent();
            gm = new GameManager(this);
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
            (this.FindName("ButtonSave") as Button).IsEnabled = isEnabled;
            (this.FindName("ButtonRestore") as Button).IsEnabled = isEnabled;
        }

        public void ButtonPlayClick(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);

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
    }
}
