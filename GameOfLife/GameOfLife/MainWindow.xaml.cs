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

namespace GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// Options :
    /// Réglage interactif de la vitesse ainsi que des dimensions de la simulation
    /// Option de pause/Play et Réinitialisation de la simulation
    /// Etat de départ aléatoire ou au choix de l'utilisateur
    /// Chargement/Sauvegarde d'un état de la simulation
    /// Affichage de statistiques de la simulation(population actuelle, itération, population min/max, age de la plus ancienne cellule, pyramide des ages des cellules, ...)


    /// </summary>
    public partial class MainWindow : Window
    {
        GameManager gm;

        public MainWindow()
        {
            InitializeComponent();
            gm = new GameManager(10, 10);
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
                    Button cell = new Button();
                    Binding binding = new Binding("CellColor");
                    binding.Source = b[i, j];
                    cell.SetBinding(Button.BackgroundProperty, binding);

                    BoardGrid.Children.Add(cell);
                    Grid.SetColumn(cell, i);
                    Grid.SetRow(cell, j);
                }
            }
        }

        public void ButtonPlayClick(object sender, RoutedEventArgs e)
        {
            gm.IsGameRunning = true;
            gm.Play();
        }

        public void ButtonPauseClick(object sender, RoutedEventArgs e)
        {
            gm.Pause();
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

        public void SliderWidthValueChanged(object sender, DragCompletedEventArgs e)
        {
            if(gm != null)
            {
                this.UpdateGrid((int)((Slider)sender).Value, gm.Board.NbCellY);
            }
            
        }

        public void SliderHeightValueChanged(object sender, DragCompletedEventArgs e)
        {
            if(gm != null)
            {
                this.UpdateGrid(gm.Board.NbCellX, (int)((Slider)sender).Value);
            }
        }
    }
}
