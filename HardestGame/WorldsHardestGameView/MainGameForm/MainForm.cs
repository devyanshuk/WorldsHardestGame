using System;
using System.IO;
using System.Media;
using System.Drawing;
using Castle.Windsor;
using Newtonsoft.Json;
using System.Windows.Forms;

using WorldsHardestGameModel.Game;
using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.Environment;
using WorldsHardestGameModel.MovementTypes;
using WorldsHardestGameModel.ConfigTemplates;
using WorldsHardestGameModel.DependencyRegistry;

using WorldsHardestGameView.ConfigTempates;

using Color = System.Drawing.Color;

namespace WorldsHardestGameView.MainGameForm
{
    public partial class MainForm : Form
    {
        private const int XOffset = 40;
        private const int YOffset = 40;

        private readonly IWindsorContainer container;
        private readonly IGameLogic game;
        private readonly ILocalSettings localSettings;
        private readonly SoundPlayer backgroundMusic;
        private readonly Bitmap coinImageBitmap;

        private readonly LevelMessageRoot levelDescriptions;

        #region level description

        private bool displayingLevelDescription = true;
        private int levelDescriptionOpacity = 255;
        private int levelDescriptionOpacityVelocity = -5;

        #endregion


        #region entity colors

        private Color playerColor = Color.Red;
        private Color obstacleColor = Color.Blue;
        private Color borderColor = Color.Black;
        private Color checkpointColor = Color.Green;
        private int checkpointOpacity = 150;
        private Color filledSquareColor = Color.DarkOliveGreen;

        #endregion


        #region player animation

        private bool playerAdvancedToNextLevel = false;
        private bool playerDead = false;
        private int playerDeathAnimationOpacity = 255;
        private int playerDeathAnimationVelocity = -20;

        #endregion


        #region checkpoint animation

        private CheckPoint animatedCheckpoint;
        private int checkpointAnimationOpacity;
        private int checkpointAnimationVelocity = 10;

        #endregion



        public MainForm()
        {
            GameLogic.onPlayerDeath += OnPlayerDeath;
            GameLogic.onPlayerInsideCheckpointWithCoins += CheckpointAnimation;
            GameLogic.onLevelAdvance += OnLevelAdvance;
            checkpointAnimationOpacity = checkpointOpacity;
            container = new WindsorContainer().Install(new DependencyInstaller());
            game = container.Resolve<IGameLogic>();
            localSettings = container.Resolve<ILocalSettings>();
            InitializeComponent();
            SetStyle(flag : ControlStyles.AllPaintingInWmPaint
                            | ControlStyles.UserPaint
                            | ControlStyles.DoubleBuffer,
                     value : true);
            BackColor = Color.Chocolate;
            coinImageBitmap = new Bitmap(FilePaths.CoinImagePath);
            backgroundMusic = new SoundPlayer(FilePaths.BackgroundMusicPath);
            backgroundMusic.PlayLooping();
            levelDescriptions = JsonConvert.DeserializeObject<LevelMessageRoot>(File.ReadAllText(FilePaths.LevelMessageJson));
            updateTimer.Start();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            game.gameEnvironment.player.RegisterNewDirection((Dir_4)e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            game.gameEnvironment.player.DeregisterNewDirection((Dir_4)e.KeyCode);
        }

        private void DisplayLevelMessage(Graphics graphics)
        {
            using (var font = new Font("Arial", 40))
            {
                using (var brush = new SolidBrush(Color.FromArgb(levelDescriptionOpacity, Color.Blue)))
                {
                    graphics.DrawString(levelDescriptions.levelMessages[game.level], font, brush, new PointF(200, 360));
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;

            if (!displayingLevelDescription)
            {
                DrawGameBackGround(graphics);
                DrawCoins(graphics);
                DrawCheckpoints(graphics);
                DrawPlayer(graphics);
                DrawObstacles(graphics);
#if DEBUG_MAIN_FORM
            DrawWallBoundariesForXYObstacles(graphics);
#endif
                DisplayScoreAndFails(graphics);
            }
            else
            {
                DisplayLevelMessage(graphics);
            }

        }

        private void DrawGameBackGround(Graphics graphics)
        {
            var freeSpaces = game.gameEnvironment.freeSpaces;

            for(int i = 0; i < localSettings.MapHeight; i++)
            {
                for (int j = 0; j < localSettings.MapWidth; j++)
                {
                    if (freeSpaces[i, j])
                    {
                        var color = (j % 2 == i % 2) ? Color.White : filledSquareColor;
                        var rectangle = new Rectangle
                            (
                                j * GameEnvironment.CELL_WIDTH + XOffset,
                                i * GameEnvironment.CELL_HEIGHT + YOffset,
                                GameEnvironment.CELL_WIDTH,
                                GameEnvironment.CELL_HEIGHT
                            );
                        DrawFilledSquare(graphics, color, rectangle);
                    }
                }
            }
        }


        private void DrawCoins(Graphics graphics)
        {
            foreach(var coin in game.gameEnvironment.coins)
            {
                graphics.DrawImage(coinImageBitmap, new Point((int)coin.centre.X - GameEnvironment.CELL_WIDTH / 4 + XOffset,
                                                              (int)coin.centre.Y - GameEnvironment.CELL_HEIGHT / 4 + YOffset));

            }
        }


        private void DisplayScoreAndFails(Graphics graphics)
        {
            using (var font = new Font("Arial", 40))
            {
                using (var brush = new SolidBrush(Color.Black))
                {
                    graphics.DrawString($"LEVEL : {game.level}", font, brush, new PointF(150, 50));
                    graphics.DrawString($"FAILS : {game.fails}", font, brush, new PointF(1000, 50));
                    graphics.DrawString($"COINS : {game.coinsCollected}", font, brush, new PointF(550, 50));
                }
            }
        }


        private void DrawPlayer(Graphics graphics)
        {
            var player = game.gameEnvironment.player;
            var borderRect = new Rectangle((int)player.topLeftPosition.X + XOffset,
                                            (int)player.topLeftPosition.Y + YOffset,
                                            Player.width,
                                            Player.height);

            var borderCol = playerDead ? Color.FromArgb(playerDeathAnimationOpacity, borderColor) : borderColor;
            DrawFilledSquare(graphics, borderCol, borderRect);

            if (borderRect.Width <= 10 || borderRect.Height <= 10)
            {
                throw new Exception($"Player's width {Player.width} and height {Player.height} must both be greater than 10");
            }

            borderRect.X += 5;
            borderRect.Y += 5;
            borderRect.Width -= 10;
            borderRect.Height -= 10;

            var playerCol = playerDead ? Color.FromArgb(playerDeathAnimationOpacity, playerColor) : playerColor;
            DrawFilledSquare(graphics, playerCol, borderRect);
        }


        private void DrawObstacles(Graphics graphics)
        {
            foreach(var obstacle in game.gameEnvironment.obstacles)
            {
                var rect = new Rectangle(
                        (int)obstacle.centre.X - GameEnvironment.CELL_WIDTH / 4 + XOffset,
                        (int)obstacle.centre.Y - GameEnvironment.CELL_HEIGHT / 4 + YOffset,
                        Obstacle.RADIUS * 2,
                        Obstacle.RADIUS * 2
                    );
                DrawFilledCircle(graphics, borderColor, rect);

                if (rect.Width <= 10 || rect.Height <= 10)
                {
                    throw new Exception($"Obstacle's radius {Obstacle.RADIUS} must be greater than 10");
                }

                rect.X += 5;
                rect.Y += 5;
                rect.Width -= 10;
                rect.Height -= 10;
                DrawFilledCircle(graphics, obstacleColor, rect);
            }
        }


        private void DrawCheckpoints(Graphics graphics)
        {
            foreach(var checkpoint in game.gameEnvironment.checkPoints)
            {
                var rect = new Rectangle
                    (
                        (int)checkpoint.topLeftPosition.X + XOffset,
                        (int)checkpoint.topLeftPosition.Y + YOffset,
                        (int)checkpoint.width,
                        (int)checkpoint.height
                    );

                var opacity = checkpointOpacity;
                if (animatedCheckpoint == checkpoint)
                {
                    opacity = checkpointAnimationOpacity;
                    checkpointAnimationOpacity += checkpointAnimationVelocity;
                    if (checkpointAnimationOpacity >= 255)
                    {
                        checkpointAnimationVelocity = -Math.Abs(checkpointAnimationVelocity);
                        checkpointAnimationOpacity = 255;
                    }
                    else if (checkpointAnimationOpacity <= checkpointOpacity)
                    {
                        checkpointAnimationVelocity = Math.Abs(checkpointAnimationVelocity);
                        checkpointAnimationOpacity = checkpointOpacity;
                        animatedCheckpoint = null;
                    }
                }
                var color = Color.FromArgb(opacity, checkpointColor);
                DrawFilledSquare(graphics, color, rect);

            }
        }




        private void DrawFilledSquare(Graphics graphics, Color color, Rectangle rect)
        {
            using (var brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, rect);
            }
        }

        private void DrawFilledCircle(Graphics graphics, Color color, Rectangle rect)
        {
            using (var brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, rect);
            }
        }


        /// <summary>
        /// Re-draw the environment every 40ms ~25FPS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!playerDead && !displayingLevelDescription)
            {
                game.UpdateEntityStates();
            }
            if (playerDead || playerAdvancedToNextLevel)
            {
                playerDeathAnimationOpacity += playerDeathAnimationVelocity;
                if (playerDeathAnimationOpacity <= 0)
                {
                    if (playerDead)
                    {
                        game.OnFail();
                    }
                    else
                    {
                        displayingLevelDescription = true;
                    }
                    playerDead = false;
                    playerAdvancedToNextLevel = false;
                    playerDeathAnimationOpacity = 255;
                    
                }
            }
            else if (displayingLevelDescription)
            {
                levelDescriptionOpacity += levelDescriptionOpacityVelocity;
                if (levelDescriptionOpacity <= 0)
                {
                    displayingLevelDescription = false;
                    levelDescriptionOpacity = 255;
                    game.AdvanceNextLevel();
                }
            }
            Invalidate();
        }


        private void DrawWallBoundariesForXYObstacles(Graphics graphics)
        {
            foreach(var b in game.gameEnvironment.obstacleBoundaries)
            {
                DrawFilledSquare(graphics,
                                 Color.Black,
                                 new Rectangle(
                                     (int)b.topLeftPosition.X + XOffset,
                                     (int)b.topLeftPosition.Y + YOffset,
                                     (int)b.width,
                                     (int)b.height)
                                 );
            }
        }


        public void OnPlayerDeath(object sender, EventArgs e)
        {
            playerDead = true;
        }


        public void CheckpointAnimation(object sender, EventArgs e)
        {
            animatedCheckpoint = sender as CheckPoint;
        }

        public void OnLevelAdvance(object sender, EventArgs e)
        {
            playerAdvancedToNextLevel = true;
            //displayingLevelDescription = true;
        }



    }
}
