using System;
using System.Collections.Generic;

using WorldsHardestGameModel.Extensions;
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

        public static EventHandler onPlayerDeath;

        public IGameEnvironment gameEnvironment { get; }

        public int level { get; private set; }

        public int fails { get; private set; }

        public int coinsCollected { get; private set; }

        public GameLogic(IParser parser,
                         ILocalSettings localSettings,
                         IGameEnvironment gameEnvironment)
        {
            this.parser = parser;
            this.localSettings = localSettings;
            this.gameEnvironment = gameEnvironment;
            this.level = 1;
            this.fails = 0;
            this.coinsCollected = 0;

        }

        public void InitializeGameEnvironment()
        {
            gameEnvironment.ClearAll();
            parser.ParseLevel(level, gameEnvironment);
            this.fails = 0;
            this.coinsCollected = 0;
        }

        public void AdvanceNextLevle()
        {
            level = 1 + level % 9;
            InitializeGameEnvironment();
        }

        public void UpdateEntityStates()
        {

            gameEnvironment.player.UpdatePosition();

            CheckPlayerWallCollision();

            CheckPlayerObstacleCollision();

            foreach(var obstacle in gameEnvironment.obstacles)
            {
                obstacle.Move();
            }

        }


        public void CheckPlayerObstacleCollision()
        {
            foreach(var obstacle in gameEnvironment.obstacles)
            {
                // Player-Obstacle collision
                if (gameEnvironment.player.IsCollision(obstacle))
                {
                    fails++;
                    onPlayerDeath?.Invoke(this, null);
                }

                // XY direction moving obstacle - Wall collision
                if (obstacle.movement is XYMovement xymov)
                {
                    foreach (var obstacleBoundary in gameEnvironment.obstacleBoundaries)
                    {
                        if (obstacleBoundary.IsCollision(obstacle))
                        {
                            xymov.ChangeDirection();
                        }
                    }
                }
            }
        }


        public void CheckPlayerWallCollision()
        {
            var unmovableDirectionsDict = new Dictionary<Dir_4, bool>()
            {
                { Dir_4.UP, false },
                { Dir_4.DOWN, false },
                { Dir_4.LEFT, false },
                { Dir_4.RIGHT, false }
            };


            foreach (var wall in gameEnvironment.walls)
            {
                var unmovableDirections = gameEnvironment.player.CheckCollision(wall);
                if (unmovableDirections.Count > 0)
                {
                    foreach (var unmovableDirection in unmovableDirections)
                    {
                        unmovableDirectionsDict[unmovableDirection] = true;
                    }
                }

            }
            gameEnvironment.player.RegisterUnmovableDirections(unmovableDirectionsDict);
        }


        public void OnFail()
        {
            gameEnvironment.player.topLeftPosition = gameEnvironment.player.initialTopLeftPosition;
        }
    }
}
