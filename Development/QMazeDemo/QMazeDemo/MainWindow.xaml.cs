using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DeepQNetworkWrapper;

namespace QMazeDemo
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int _n = 9;
        private const int _m = 9;
        //private uint[,] _copyMazeMap = new uint[12, 12]
        //{
        //    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        //    { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        //    { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        //    { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        //    { 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 1 },
        //    { 1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
        //    { 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1 },
        //    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        //    { 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
        //    { 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1 },
        //    { 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 2, 1 },
        //    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        //};
        private uint[,] _copyMazeMap = new uint[_n, _m]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 1, 0, 0, 0, 0, 0, 1},
            { 1, 0, 0, 0, 1, 1, 0, 1, 1},
            { 1, 1, 1, 1, 0, 0, 0, 1, 1},
            { 1, 0, 0, 0, 0, 1, 1, 0, 1},
            { 1, 0, 1, 1, 1, 0, 0, 0, 1},
            { 1, 0, 1, 0, 0, 0, 0, 0, 1},
            { 1, 0, 0, 0, 1, 0, 0, 2, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };


        private uint[,] _mazeMap = new uint[_n, _m];

        private double _minReward = -0.5 * _n * _m;
        private double _totalReward;

        private bool _startedTraining = false;

        private DeepQWrapper _brain;

        private Rectangle _player = new Rectangle();

        private Rectangle[,] _children = new Rectangle[_n, _m];

        public MainWindow()
        {
            InitializeComponent();

            NeuralNetworkWrapper nn = new NeuralNetworkWrapper(0.5);
            nn.AddLayer(_n * _m, _n * _m, "RELU");
            nn.AddLayer(_n * _m, _n * _m, "RELU");
            nn.AddLayer(4, _n * _m, "RELU");
            _brain = new DeepQWrapper(nn);
            _brain.SetGamma(0.95);
            _brain.SetExploration(0.1);
            _brain.SetReplayBatchSize(32);
            _brain.SetUpdateTargetWeightsAfterSteps(1);
            _brain.SetExperienceMemorySize(8 * _n * _m);
            _brain.SetTrainEpochs(8);

            DrawMap();

            _player.Width = 49;
            _player.Height = 49;
            _player.Fill = new SolidColorBrush(Colors.Green);
            Canvas.SetTop(_player, 0);
            Canvas.SetLeft(_player, 0);
            canvas.Children.Add(_player);

            _player.SetValue(Canvas.TopProperty, 50.0);
            _player.SetValue(Canvas.LeftProperty, 50.0);
        }

        private void DrawMap()
        {
            for (var i = 0; i < _n; ++i)
            {
                for (var j = 0; j < _m; ++j)
                {
                    var rectangle = new Rectangle();
                    rectangle.Width = 49;
                    rectangle.Height = 49;

                    switch (_copyMazeMap[i, j])
                    {
                        case 0:
                            {
                                rectangle.Fill = new SolidColorBrush(Colors.LightGray);
                                break;
                            }
                        case 1:
                            {
                                rectangle.Fill = new SolidColorBrush(Colors.Red);
                                break;
                            }
                        case 2:
                            {
                                rectangle.Fill = new SolidColorBrush(Colors.Blue);
                                break;
                            }
                    }

                    Canvas.SetTop(rectangle, 50 * i);
                    Canvas.SetLeft(rectangle, 50 * j);
                    _children[i, j] = rectangle;
                    canvas.Children.Add(rectangle);
                }
            }
        }

        ~MainWindow()
        {
        }

        private void CopyMazeMap()
        {
            for (var i = 0; i < _n; ++i)
            {
                for (var j = 0; j < _m; ++j)
                {
                    _mazeMap[i, j] = _copyMazeMap[i, j];
                }
            }
        }

        private void Train_Click(object sender, RoutedEventArgs e)
        {
            if (_startedTraining)
                return;

            var trainThread = new Thread(() =>
            {
                var totalGames = 1000000;
                var numberOfGamesWon = 0;
                var numberOfGamesLost = 0;
                var state = new double[4];
                var gameWon = false;
                double reward = 0;
                int action = 0;
                var numberOfGames = 0;
                var epsilon = 1.0;
                while (numberOfGames < totalGames)
                {
                    //epsilon = epsilon > 0.1 ? epsilon * 0.9995 : epsilon;
                    //_brain.SetMinimumExploration(epsilon);

                    //Thread.Sleep(500);
                    CopyMazeMap();

                    // Reset position
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        DrawMap();
                        _player.SetValue(Canvas.TopProperty, 50.0);
                        _player.SetValue(Canvas.LeftProperty, 50.0);
                    });

                    numberOfGames++;
                    _totalReward = 0;
                    reward = 0;

                    while (!gameWon)
                    {
                        // Get State
                        var playerPos = GetMapPlayerPosition();

                        //state[0] = _mazeMap[playerPos.i - 1, playerPos.j];
                        //state[1] = _mazeMap[playerPos.i, playerPos.j + 1];
                        //state[2] = _mazeMap[playerPos.i + 1, playerPos.j];
                        //state[3] = _mazeMap[playerPos.i, playerPos.j - 1];

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _children[playerPos.i, playerPos.j].Fill = new SolidColorBrush(Colors.Yellow);
                        });

                        // flatten state
                        double[] fMap = new double[_n * _m];
                        int k = 0;
                        for (var i = 0; i < _n; ++i)
                        {
                            for (var j = 0; j < _m; ++j)
                            {
                                fMap[k++] = _mazeMap[i, j];
                            }
                        }

                        // Get action
                        action = _brain.GetAction(fMap);

                        // Take Action
                        switch (action)
                        {
                            case 0:
                                {
                                    reward = MoveUp();
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        actionLabel.Content = "UP";
                                    });
                                    break;
                                }
                            case 1:
                                {
                                    reward = MoveRight();
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        actionLabel.Content = "RIGHT";
                                    });
                                    break;
                                }
                            case 2:
                                {
                                    reward = MoveDown();
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        actionLabel.Content = "DOWN";
                                    });
                                    break;
                                }
                            case 3:
                                {
                                    reward = MoveLeft();
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        actionLabel.Content = "LEFT";
                                    });
                                    break;
                                }
                        }

                        _brain.Remember();
                        _brain.ReplayExperience();

                        if (reward == 1)
                        {
                            gameWon = true;
                            numberOfGamesWon++;
                            double winRate = numberOfGamesWon * 100 / totalGames;
                            if (winRate > 0.9)
                            {
                                _brain.SetExploration(0.05);
                            }

                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                gamesWonLabel.Content = numberOfGamesWon;
                            });
                            break;
                        }
                        else
                        {
                            _totalReward += reward;
                        }

                        if (_totalReward < _minReward)
                        {
                            numberOfGamesLost++;
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                gamesLostLabel.Content = numberOfGamesLost;
                            });
                            break;
                        }

                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        rewardLabel.Content = _totalReward + " Exploration: " + epsilon * 100;
                    });
                }
            });

            trainThread.IsBackground = true;
            trainThread.Start();
            _startedTraining = true;
        }

        private (int i, int j) GetMapPlayerPosition()

        {
            int i = -1, j = -1;
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                var topPos = Canvas.GetTop(_player);
                var leftPos = Canvas.GetLeft(_player);
                i = (int)(topPos / 50);
                j = (int)(leftPos / 50);
            });

            return (i, j);
        }

        private (double reward, bool canMove) GetReward(int i, int j)
        {
            switch (_mazeMap[i, j])
            {
                case 3:
                    {
                        return (-0.25, true);
                    }
                case 1:
                    {
                        return (-0.75, false);
                    }
                case 2:
                    {
                        return (1, true);
                    }
                default:
                    {
                        return (0, true);
                    }
            }
        }

        private double MoveUp()
        {
            var pos = GetMapPlayerPosition();

            var reward = GetReward(pos.i - 1, pos.j);
            _totalReward += reward.reward;

            if (reward.canMove)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    _player.SetValue(Canvas.TopProperty, Canvas.GetTop(_player) - 50.0);
                });
                _mazeMap[pos.i - 1, pos.j] = 5;
                _mazeMap[pos.i, pos.j] = 3;
            }

            return reward.reward;
        }

        private double MoveDown()
        {
            var pos = GetMapPlayerPosition();

            var reward = GetReward(pos.i + 1, pos.j);
            _totalReward += reward.reward;

            if (reward.canMove)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    _player.SetValue(Canvas.TopProperty, Canvas.GetTop(_player) + 50.0);
                });
                _mazeMap[pos.i + 1, pos.j] = 5;
                _mazeMap[pos.i, pos.j] = 3;
            }

            return reward.reward;
        }

        private double MoveLeft()
        {
            var pos = GetMapPlayerPosition();

            var reward = GetReward(pos.i, pos.j - 1);
            _totalReward += reward.reward;

            if (reward.canMove)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    _player.SetValue(Canvas.LeftProperty, Canvas.GetLeft(_player) - 50.0);
                });
                _mazeMap[pos.i, pos.j - 1] = 5;
                _mazeMap[pos.i, pos.j] = 3;
            }

            return reward.reward;
        }

        private double MoveRight()
        {
            var pos = GetMapPlayerPosition();

            var reward = GetReward(pos.i, pos.j + 1);
            _totalReward += reward.reward;

            if (reward.canMove)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    _player.SetValue(Canvas.LeftProperty, Canvas.GetLeft(_player) + 50.0);
                });
                _mazeMap[pos.i, pos.j + 1] = 5;
                _mazeMap[pos.i, pos.j] = 3;
            }

            return reward.reward;
        }
    }
}
