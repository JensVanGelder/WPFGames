// Credit: mooict.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFGames
{
    public partial class SpaceInvaders : Window
    {
        bool goLeft, goRight;
        List<Rectangle> itemsToRemove = new List<Rectangle>();
        int enemyImages = 0;
        int bulletTimer = 0;
        int bulletTimerLimit = 90;
        int totalEnemies = 0;
        int enemySpeed = 6;
        bool gameOver = false;
        DispatcherTimer gameTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();

        public SpaceInvaders()
        {
            InitializeComponent();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();


            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/player.png"));
            player.Fill = playerSkin;

            myCanvas.Focus();

            MakeEnemies(25);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            enemiesLeft.Content = "Enemies Left:" + totalEnemies;

            if (goLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (goRight == true && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }

            bulletTimer -= 3;
            if (bulletTimer < 0)
            {
                EnemyBulletMaker(Canvas.GetLeft(player) + 20, 10);

                bulletTimer = bulletTimerLimit;
            }

            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(enemyHit))
                            {
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);

                                totalEnemies -= 1;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

                    if (Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }

                    Rect enemyHitbox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitbox))
                    {
                        ShowGameOver("You were killed by the invaders!");
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);

                    if (Canvas.GetTop(x) > 480)
                    {
                        itemsToRemove.Add(x);
                    }

                    Rect enemyBulletHitbox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitbox))
                    {
                        ShowGameOver("You were killed by the invaders!");
                    }
                }
            }

            foreach (Rectangle rectangle in itemsToRemove)
            {
                myCanvas.Children.Remove(rectangle);
            }

            if (totalEnemies < 10)
            {
                enemySpeed = 12;
            }

            if (totalEnemies < 1)
            {
                ShowGameOver("You win!");
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red,
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);

                myCanvas.Children.Add(newBullet);
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                SpaceInvaders newSpace = new SpaceInvaders();
                Application.Current.MainWindow = newSpace;
                newSpace.Show();
                this.Close();
            }
        }
        private void EnemyBulletMaker(double x, double y)
        {
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 5,
            };
            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);

            myCanvas.Children.Add(enemyBullet);

        }
        private void MakeEnemies(int limit)
        {
            int left = 0;

            totalEnemies = limit;

            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();

                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin,
                };

                Canvas.SetTop(newEnemy, 30);
                Canvas.SetLeft(newEnemy, left);
                myCanvas.Children.Add(newEnemy);
                left -= 60;

                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                switch (enemyImages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader1.gif"));
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader2.gif"));
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader3.gif"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader4.gif"));
                        break;
                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader5.gif"));
                        break;
                    case 6:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader6.gif"));
                        break;
                    case 7:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader7.gif"));
                        break;
                    case 8:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/WpfGames;component/images/invader8.gif"));
                        break;
                }
            }
        }
        private void ShowGameOver(string msg)
        {
            gameOver = true;
            gameTimer.Stop();
            enemiesLeft.Content += " " + msg + " Press Enter to play again";
        }
    }
}

