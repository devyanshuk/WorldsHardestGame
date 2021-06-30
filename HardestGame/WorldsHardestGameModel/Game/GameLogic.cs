using System.Drawing;

using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.Environment;
using WorldsHardestGameModel.MovementTypes;
using WorldsHardestGameModel.Levels.Parser;
using WorldsHardestGameModel.ConfigTemplates;

namespace WorldsHardestGameModel.Game
{
    public class GameLogic : IGameLogic
    {
        private readonly IParser parser;
        private readonly ILocalSettings localSettings;

        public IGameEnvironment gameEnvironment { get; set; }

        public int level { get; set; }

        public int fails { get; set; }

        public GameLogic(IParser parser,
                         ILocalSettings localSettings,
                         IGameEnvironment gameEnvironment)
        {
            this.parser = parser;
            this.localSettings = localSettings;
            this.gameEnvironment = gameEnvironment;
            this.level = 1;
            this.fails = 0;
        }

        public void InitializeGameEnvironment()
        {
            gameEnvironment.ClearAll();
            parser.ParseLevel(level, gameEnvironment);
            this.fails = 0;
        }

        public void AdvanceNextLevle()
        {
            level = (level + 1) % 9;
            if (level == 0)
            {
                level = 1;
            }
            InitializeGameEnvironment();
        }

        public void UpdateEntityStates()
        {
            gameEnvironment.player.UpdatePosition();

            ObstacleWallCollisionCheck();

            foreach(var obstacle in gameEnvironment.obstacles)
            {
                obstacle.Move();
            }

        }


        /// <summary>
        /// Only obstacles moving in the XY direction changes direction upon
        /// wall collision. The other two have a pre-degined movement configuration.
        /// </summary>
        private void ObstacleWallCollisionCheck()
        {
            foreach(var obstacle in gameEnvironment.obstacles)
            {
                if (obstacle.movement is XYMovement xymov)
                {
                    foreach(var obstacleBoundary in gameEnvironment.obstacleBoundaries)
                    {
                        PointF posToCheck = new PointF();

                        switch (xymov.currentDir)
                        {
                            case Dir_4.LEFT:
                                posToCheck = new PointF(obstacle.centre.X - Obstacle.RADIUS / 2, obstacle.centre.Y);
                                break;

                            case Dir_4.RIGHT:
                                posToCheck = new PointF(obstacle.centre.X + Obstacle.RADIUS / 2, obstacle.centre.Y);
                                break;

                            case Dir_4.DOWN:
                                posToCheck = new PointF(obstacle.centre.X, obstacle.centre.Y + Obstacle.RADIUS / 2);
                                break;

                            case Dir_4.UP:
                                posToCheck = new PointF(obstacle.centre.X, obstacle.centre.Y - Obstacle.RADIUS / 2);
                                break;
                        }
                        if (obstacleBoundary.IsCollision(posToCheck))
                        {
                            xymov.ChangeDirection();
                        }
                    }
                }
            }
        }
    }
}
