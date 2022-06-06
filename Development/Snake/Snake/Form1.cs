using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DeepQNetworkWrapper;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>(); // creating an list array for the snake
        private Circle food = new Circle(); // creating a single Circle class called food
        private DeepQWrapper _brain;
        private double[] _state;
        private double[] _previousState;
        private double _reward;
        private double _previousReward;
        private bool _previousGameOver;
        private Directions _previousDirection;
        private long xx = 0;
        private double exploration = 1;

        private ConcurrentQueue<(double[] i, int a, double[] f, double r, bool o)> queue = new ConcurrentQueue<(double[] i, int a, double[] f, double r, bool o)>();

        public Form1()
        {
            InitializeComponent();

            new Settings(); // linking the Settings Class to this Form

            _state = new double[13];

            // setup AI
            var network = new NeuralNetworkWrapper(0.001);
            network.AddLayer(256, (uint)_state.Length, "RELU");
            network.AddLayer(4, 256, "LINEAR");
            _brain = new DeepQWrapper(network);
            _brain.SetGamma(0.9);
            _brain.SetExploration(exploration);
            _brain.SetReplayBatchSize(8);
            _brain.SetTrainEpochs(1);
            _brain.SetUpdateTargetWeightsAfterSteps(1);
            _brain.SetUpdateMainWeightsAfterSteps(1);
            _brain.SetExperienceMemorySize(1000);

            new Thread(() =>
            {
                while(true)
                {
                    if(queue.TryDequeue(out var exp))
                    {
                        _brain.Remember(exp.i, exp.a, exp.f, exp.r, exp.o);
                        _brain.ReplayExperience();
                        Thread.Sleep(50);
                    }
                }
            }) { IsBackground = true }.Start();

            gameTimer.Interval = 1000 / Settings.Speed; // Changing the game time to settings speed
            gameTimer.Tick += updateSreen; // linking a updateScreen function to the timer
            gameTimer.Start(); // starting the timer

            startGame(); // running the start game function

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // the key down event will trigger the change state from the Input class
            Input.changeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // the key up event will trigger the change state from the Input class
            Input.changeState(e.KeyCode, false);
        }

        private void updateGraphics(object sender, PaintEventArgs e)
        {
            // this is where we will see the snake and its parts moving

            Graphics canvas = e.Graphics; // create a new graphics class called canvas

            if (Settings.GameOver == false)
            {
                // if the game is not over then we do the following

                Brush snakeColour; // create a new brush called snake colour

                // run a loop to check the snake parts
                for (int i = 0; i < Snake.Count; i++)
                {
                    if (i == 0)
                    {
                        // colour the head of the snake black
                        snakeColour = Brushes.Black;
                    }
                    else
                    {
                        // the rest of the body can be green
                        snakeColour = Brushes.Green;
                    }
                    //draw snake body and head
                    canvas.FillEllipse(snakeColour,
                                        new Rectangle(
                                            Snake[i].X * Settings.Width,
                                            Snake[i].Y * Settings.Height,
                                            Settings.Width, Settings.Height
                                            ));

                    // draw food
                    canvas.FillEllipse(Brushes.Red,
                                        new Rectangle(
                                            food.X * Settings.Width,
                                            food.Y * Settings.Height,
                                            Settings.Width, Settings.Height
                                            ));
                }
            }
            else
            {
                // this part will run when the game is over
                // it will show the game over text and make the label 3 visible on the screen

                string gameOver = "Game Over \n" + "Final Score is " + Settings.Score + "\n Press enter to Restart \n";
                label3.Text = gameOver;
                label3.Visible = true;
            }
        }

        private void startGame()
        {
            // this is the start game function

            label3.Visible = false; // set label 3 to invisible
            new Settings(); // create a new instance of settings
            Snake.Clear(); // clear all snake parts
            int maxXpos = pbCanvas.Size.Width / Settings.Width;
            // create a maximum X position int with half the size of the play area
            int maxYpos = pbCanvas.Size.Height / Settings.Height;
            // create a maximum Y position int with half the size of the play area
            Random rnd = new Random(); // create a new random class
            generateFood(); // run the generate food function
            Circle head = null;
            do
            {
                head = new Circle { X = rnd.Next(0, maxXpos), Y = rnd.Next(0, maxYpos) }; // create a new head for the snake
            } while (head.X == food.X && head.Y == food.Y);
            Snake.Add(head); // add the gead to the snake array

            label2.Text = Settings.Score.ToString(); // show the score to the label 2
        }

        private void movePlayer()
        {
            // the main loop for the snake head and parts
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                // if the snake head is active 
                if (i == 0)
                {
                    // move rest of the body according to which way the head is moving
                    switch (Settings.direction)
                    {
                        case Directions.Right:
                            Snake[i].X++;
                            break;
                        case Directions.Left:
                            Snake[i].X--;
                            break;
                        case Directions.Up:
                            Snake[i].Y--;
                            break;
                        case Directions.Down:
                            Snake[i].Y++;
                            break;
                    }

                    // restrict the snake from leaving the canvas
                    int maxXpos = pbCanvas.Size.Width / Settings.Width;
                    int maxYpos = pbCanvas.Size.Height / Settings.Height;

                    if (
                        Snake[i].X < 0 || Snake[i].Y < 0 ||
                        Snake[i].X > maxXpos || Snake[i].Y > maxYpos
                        )
                    {
                        // end the game is snake either reaches edge of the canvas

                        die();
                    }

                    // detect collision with the body
                    // this loop will check if the snake had an collision with other body parts
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            // if so we run the die function
                            die();
                        }
                    }

                    // detect collision between snake head and food
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        //if so we run the eat function
                        eat();
                    }

                }
                else
                {
                    // if there are no collisions then we continue moving the snake and its parts
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void generateFood()
        {
            int maxXpos = pbCanvas.Size.Width / Settings.Width;
            // create a maximum X position int with half the size of the play area
            int maxYpos = pbCanvas.Size.Height / Settings.Height;
            // create a maximum Y position int with half the size of the play area
            Random rnd = new Random(); // create a new random class
            food = new Circle { X = rnd.Next(0, maxXpos), Y = rnd.Next(0, maxYpos) };
            // create a new food with a random x and y
        }

        private void eat()
        {
            // add a part to body

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y

            };

            Snake.Add(body); // add the part to the snakes array
            Settings.Score += Settings.Points; // increase the score for the game
            label2.Text = Settings.Score.ToString(); // show the score on the label 2
            generateFood(); // run the generate food function
            _reward = 10;
        }

        private void die()
        {
            // change the game over Boolean to true
            Settings.GameOver = true;
            _reward = -100;
        }

        private void updateSreen(object sender, EventArgs e)
        {
            // this is the Timers update screen function. 
            // each tick will run this function

            if (Settings.GameOver == true)
            {

                // if the game over is true and player presses enter
                // we run the start game function

                startGame();

                //if (Input.KeyPress(Keys.Enter))
                //{
                //    startGame();
                //}
            }
            else
            {
                //if the game is not over then the following commands will be executed

                // below the actions will probe the keys being presse by the player
                // and move the accordingly

                //if (Input.KeyPress(Keys.Right) && Settings.direction != Directions.Left)
                //{
                //    Settings.direction = Directions.Right;
                //}
                //else if (Input.KeyPress(Keys.Left) && Settings.direction != Directions.Right)
                //{
                //    Settings.direction = Directions.Left;
                //}
                //else if (Input.KeyPress(Keys.Up) && Settings.direction != Directions.Down)
                //{
                //    Settings.direction = Directions.Up;
                //}
                //else if (Input.KeyPress(Keys.Down) && Settings.direction != Directions.Up)
                //{
                //    Settings.direction = Directions.Down;
                //}

                // AI logic
                _reward = 0;
                // get state
                EvaluateState();
                var distance = DistanceFromFood();
                // get action
                Settings.direction = (Directions)_brain.GetAction(_state);

                movePlayer(); // run move player function

                // train
                if (_previousState == null)
                {
                    _previousState = new double[_state.Length];
                }
                else
                {
                    //_brain.Remember(_previousState, (int)_previousDirection, _state, _previousReward, _previousGameOver);
                    queue.Enqueue((_previousState, (int)_previousDirection, _state, _previousReward, _previousGameOver));
                    xx++;
                    if(xx % 1000 == 0 && exploration > 0.22)
                    {
                        exploration -= 0.2;
                        _brain.SetExploration(exploration);
                        //MessageBox.Show($"Exploration set to {exploration}");
                    }
                    //if (xx < long.MaxValue)
                    //{
                    //    if (xx == 8000)
                    //        _brain.SetExploration(0);
                    //    _brain.ReplayExperience();
                    //    xx += 8;
                    //}
                }

                Array.Copy(_state, _previousState, _state.Length); // save previous state
                _previousDirection = Settings.direction;
                _previousReward = _reward;
                _previousGameOver = Settings.GameOver;

                if (distance >= DistanceFromFood())
                    _previousReward += 1;
                else
                    _previousReward -= 1;

            }

            pbCanvas.Invalidate(); // refresh the picture box and update the graphics on it

            //if (Settings.GameOver)
            //    Thread.Sleep(2000);
        }

        private void EvaluateState()
        {
            var head = Snake[0];
            var idx = 0;

            var topDanger = false;
            var rightDanger = false;
            var botDanger = false;
            var leftDanger = false;

            Array.Clear(_state, 0, _state.Length);

            int maxXpos = pbCanvas.Size.Width / Settings.Width;
            int maxYpos = pbCanvas.Size.Height / Settings.Height;

            for (int j = 1; j < Snake.Count; j++)
            {
                if (!topDanger && (head.Y - 1 == Snake[j].Y || head.Y - 1 < 0))
                {
                    topDanger = true;
                }

                if (!rightDanger && (head.X + 1 == Snake[j].X || head.X + 1 > maxXpos))
                {
                    rightDanger = true;
                }

                if (!botDanger && (head.Y + 1 == Snake[j].Y || head.Y + 1 > maxYpos))
                {
                    botDanger = true;
                }

                if (!leftDanger && (head.X - 1 == Snake[j].X || head.X - 1 < 0))
                {
                    leftDanger = true;
                }

                if (topDanger && rightDanger && botDanger && leftDanger)
                    break;
            }


            _state[idx++] = topDanger ? 1 : 0;
            _state[idx++] = rightDanger ? 1 : 0;
            _state[idx++] = botDanger ? 1 : 0;
            _state[idx++] = leftDanger ? 1 : 0;

            _state[idx++] = Settings.direction == Directions.Up ? 1 : 0; // snake is moving towards top
            _state[idx++] = Settings.direction == Directions.Right ? 1 : 0; // snake is moving towards right
            _state[idx++] = Settings.direction == Directions.Down ? 1 : 0; // snake is moving down
            _state[idx++] = Settings.direction == Directions.Left ? 1 : 0; // snake is moving towards left

            _state[idx++] = food.Y < head.Y ? 1 : 0; // food is up
            _state[idx++] = food.X > head.X ? 1 : 0; // food is to tthe right
            _state[idx++] = food.Y > head.Y ? 1 : 0; // food is down
            _state[idx++] = food.X < head.X ? 1 : 0; // food is to the left

            _state[idx++] = DistanceFromFood();
        }

        private double DistanceFromFood()
        {
            return Math.Sqrt(Math.Pow(Snake[0].X - food.X, 2) + Math.Pow(Snake[0].Y - food.Y, 2));
        }
    }
}
