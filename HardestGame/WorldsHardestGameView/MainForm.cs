using System;
using System.Drawing;
using Castle.Windsor;
using System.Windows.Forms;

using WorldsHardestGameModel.Game;
using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.Environment;
using WorldsHardestGameModel.MovementTypes;
using WorldsHardestGameModel.ConfigTemplates;
using WorldsHardestGameModel.DependencyRegistry;

namespace WorldsHardestGameView
{
    public partial class MainForm : Form
    {
        private const int XMargin = 130;
        private const int YMargin = 70;

        private readonly IWindsorContainer container;
        private readonly IGameLogic game;
        private readonly ILocalSettings localSettings;


        #region entity colors

        private Color playerColor = Color.Red;
        private Color obstacleColor = Color.Blue;
        private Color borderColor = Color.Black;
        private Color checkpointColor = Color.FromArgb(100, 0, 255, 0);
        private Color filledSquareColor = Color.DarkOliveGreen;//Color.FromArgb(100, 46, 58, 25);

        #endregion



        public MainForm()
        {
            container = new WindsorContainer().Install(new DependencyInstaller());
            game = container.Resolve<IGameLogic>();
            localSettings = container.Resolve<ILocalSettings>();
            InitializeComponent();
            BackColor = Color.Chocolate;
            game.InitializeGameEnvironment();
            updateTimer.Start();
        }

        /**/
        protected override CreateParams CreateParams
        {
            get
            {
                var handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;
                return handleParam;
            }
        }
        /**/

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

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var graphics = e.Graphics)
            {
                DrawGameBackGround(graphics);
                DrawCheckpoints(graphics);
                DrawPlayer(graphics);
                DrawObstacles(graphics);
#if DEBUG_MAIN_FORM
                DrawWallBoundariesForXYObstacles(graphics);
#endif
                DisplayScoreAndFails(graphics);

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
                                j * game.gameEnvironment.CELL_WIDTH + XMargin,
                                i * game.gameEnvironment.CELL_HEIGHT + YMargin,
                                game.gameEnvironment.CELL_WIDTH,
                                game.gameEnvironment.CELL_HEIGHT
                            );
                        DrawFilledSquare(graphics, color, rectangle);
                    }
                }
            }
        }


        private void DisplayScoreAndFails(Graphics graphics)
        {
            using (var font = new Font("Arial", 40))
            {
                using (var brush = new SolidBrush(Color.Black))
                {
                    graphics.DrawString($"LEVEL : {game.level}", font, brush, new PointF(200, 50));
                    graphics.DrawString($"FAILS : {game.fails}", font, brush, new PointF(1200, 50));
                }
            }
        }


        private void DrawPlayer(Graphics graphics)
        {
            var player = game.gameEnvironment.player;
            var borderRect = new Rectangle((int)player.topLeftPosition.X + XMargin,
                                            (int)player.topLeftPosition.Y + YMargin,
                                            Player.width,
                                            Player.height);
            DrawFilledSquare(graphics, borderColor, borderRect);

            if (borderRect.Width <= 10 || borderRect.Height <= 10)
            {
                throw new Exception($"Player's width {Player.width} and height {Player.height} must both be greater than 10");
            }

            borderRect.X += 5;
            borderRect.Y += 5;
            borderRect.Width -= 10;
            borderRect.Height -= 10;
            DrawFilledSquare(graphics, playerColor, borderRect);
        }


        private void DrawObstacles(Graphics graphics)
        {
            foreach(var obstacle in game.gameEnvironment.obstacles)
            {
                var rect = new Rectangle(
                        (int)obstacle.centre.X - game.gameEnvironment.CELL_WIDTH / 4 + XMargin,
                        (int)obstacle.centre.Y - game.gameEnvironment.CELL_HEIGHT / 4 + YMargin,
                        Obstacle.RADIUS,
                        Obstacle.RADIUS
                    );
                DrawFilledCircle(graphics, borderColor, rect);

                if (rect.Width <= 10 || rect.Height <= 10)
                {
                    throw new Exception($"Obstacle's radius {Obstacle.RADIUS} must be greater than 10");
                }

                rect.X += 5;
                rect.Y += 5;
                rect.Width -= 10;
                rect.Width -= 10;
                DrawFilledCircle(graphics, obstacleColor, rect);
            }
        }


        private void DrawCheckpoints(Graphics graphics)
        {
            foreach(var checkpoint in game.gameEnvironment.checkPoints)
            {
                var rect = new Rectangle
                    (
                        (int)checkpoint.topLeftPosition.X + XMargin,
                        (int)checkpoint.topLeftPosition.Y + YMargin,
                        (int)checkpoint.width,
                        (int)checkpoint.height
                    );
                DrawFilledSquare(graphics, checkpointColor, rect);

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
        /// Re-draw the environment every 20ms ~60FPS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            game.UpdateEntityStates();
            Invalidate();
        }


        private void DrawWallBoundariesForXYObstacles(Graphics graphics)
        {
            foreach(var b in game.gameEnvironment.obstacleBoundaries)
            {
                DrawFilledSquare(graphics,
                                 Color.Black,
                                 new Rectangle(
                                     (int)b.topLeftPosition.X + XMargin,
                                     (int)b.topLeftPosition.Y + YMargin,
                                     (int)b.width,
                                     (int)b.height)
                                 );
            }
        }



    }
}
