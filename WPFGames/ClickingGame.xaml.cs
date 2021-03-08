using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFGames
{
    /// <summary>
    /// Interaction logic for ClickingGame.xaml
    /// </summary>
    public partial class ClickingGame : Window
    {
        private DispatcherTimer gameTimer = new DispatcherTimer();

        private List<Ellipse> removeThis = new List<Ellipse>(); 
        private int spawnRate = 60; 

        private int currentRate;
        private int lastScore = 0;
        private int health = 350; 
        private int posX; 
        private int posY; 
        private int score = 0; 

        private double growthRate = 0.6;

        private Random rand = new Random(); 

        private MediaPlayer playClickSound = new MediaPlayer();
        private MediaPlayer playerPopSound = new MediaPlayer();

        private Uri ClickedSound;
        private Uri PoppedSound;

        private Brush brush;

        public ClickingGame()
        {
            InitializeComponent();

            gameTimer.Tick += GameLoop; 
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            currentRate = spawnRate; 


            ClickedSound = new Uri("pack://siteoforigin:,,,/sound/clickedpop.mp3");
            PoppedSound = new Uri("pack://siteoforigin:,,,/sound/pop.mp3");
        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;
            txtLastScore.Content = "Last Score: " + lastScore;
            currentRate -= 2;

            if (currentRate < 1)
            {
                currentRate = spawnRate;

                posX = rand.Next(15, 700);
                posY = rand.Next(50, 350);

                brush = new SolidColorBrush(Color.FromRgb((byte)rand.Next(1, 255), (byte)rand.Next(1, 255), (byte)rand.Next(1, 255)));

                Ellipse circle = new Ellipse
                {
                    Tag = "circle",
                    Height = 10,
                    Width = 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = brush
                };
                Canvas.SetLeft(circle, posX);
                Canvas.SetTop(circle, posY);
                MyCanvas.Children.Add(circle);
            }

            foreach (var x in MyCanvas.Children.OfType<Ellipse>())
            {
                x.Height += growthRate; 
                x.Width += growthRate; 
                x.RenderTransformOrigin = new Point(0.5, 0.5); 

                if (x.Width > 70)
                {
                    removeThis.Add(x);
                    health -= 15; 
                    playerPopSound.Open(PoppedSound);
                    playerPopSound.Play(); 
                }
            } 

            if (health > 1)
            {
                healthBar.Width = health;
            }
            else
            {
                GameOverFunction();
            }
            foreach (Ellipse i in removeThis)
            {
                MyCanvas.Children.Remove(i); 
            }
            if (score > 5)
            {
                spawnRate = 25;
            }

            if (score > 20)
            {
                spawnRate = 15;
                growthRate = 1.5;
            }
        }

        private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse)
            {
                Ellipse circle = (Ellipse)e.OriginalSource;
                MyCanvas.Children.Remove(circle);
                score++;

                playClickSound.Open(ClickedSound);
                playClickSound.Play();
            }
        }

        private void GameOverFunction()
        {
            gameTimer.Stop();

            MessageBox.Show("Game Over" + Environment.NewLine + "You Scored: " + score + Environment.NewLine + "Click Ok to play again!", "Moo Says: ");

            foreach (var y in MyCanvas.Children.OfType<Ellipse>())
            {
                removeThis.Add(y);
            }
            foreach (Ellipse i in removeThis)
            {
                MyCanvas.Children.Remove(i);
            }

            growthRate = .6;
            spawnRate = 60;
            lastScore = score;
            score = 0;
            currentRate = 5;
            health = 350;
            removeThis.Clear();
            gameTimer.Start();
        }
    }
}