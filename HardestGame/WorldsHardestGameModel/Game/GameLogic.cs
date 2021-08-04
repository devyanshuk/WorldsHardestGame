using System;
using System.Collections.Generic;

using WorldsHardestGameModel.Entities;
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
        public static EventHandler onPlayerInsideCheckpointWithCoins;
        public static EventHandler onLevelAdvance;

        public IGameEnvironment gameEnvironment { get; }


        public int level { get; private set; }

        public int fails { get; private set; }

        private bool insideCheckpoint;

        public int coinsCollected { get; private set; }

        private int numberOfCoinsSinceLastCheckpointSave;
        private HashSet<Coin> collectedCoins;

        public GameLogic(IParser parser,
                         ILocalSettings localSettings,
                         IGameEnvironment gameEnvironment)
        {
            this.parser = parser;
            this.localSettings = localSettings;
            this.gameEnvironment = gameEnvironment;
            level = 0;
            fails = 0;
            coinsCollected = 0;
            insideCheckpoint = false;
            collectedCoins = new HashSet<Coin>();

        }

        public void InitializeGameEnvironment()
        {
            gameEnvironment.ClearAll();
            parser.ClearAll();
            parser.ParseLevel(level, gameEnvironment);
            fails = 0;
            coinsCollected = 0;
        }

        public void AdvanceNextLevel()
        {
            level = 1 + level % 9;
            InitializeGameEnvironment();
        }

        public void UpdateEntityStates()
        {
            gameEnvironment.player.UpdatePosition();
            CheckPlayerWallCollision();
            CheckPlayerCheckpointCollision();
            CheckPlayerCoinCollision();
            CheckPlayerObstacleAndObstacleWallCollision();
            foreach(var obstacle in gameEnvironment.obstacles)
            {
                obstacle.Move();
            }
            CheckGameProgress();
        }


        private void CheckGameProgress()
        {
            if (insideCheckpoint && coinsCollected == gameEnvironment.numberOfCoins)
            {
                onLevelAdvance?.Invoke(this, null);
            }
        }


        private void CheckPlayerCheckpointCollision()
        {
            if (gameEnvironment.player.isMoving)
            {
                foreach (var checkpoint in gameEnvironment.checkPoints)
                {
                    if (checkpoint.IsCollision(gameEnvironment.player))
                    {
                        insideCheckpoint = true;
                        if (numberOfCoinsSinceLastCheckpointSave > 0)
                        {
                            numberOfCoinsSinceLastCheckpointSave = 0;
                            collectedCoins.Clear();
                            onPlayerInsideCheckpointWithCoins?.Invoke(checkpoint, null);
                        }
                        return;
                    }
                }
                insideCheckpoint = false;
            }
        }


        private void CheckPlayerCoinCollision()
        {
            foreach(var coin in gameEnvironment.coins)
            {
                if (gameEnvironment.player.IsCollision(coin))
                {
                    coinsCollected++;
                    numberOfCoinsSinceLastCheckpointSave++;
                    collectedCoins.Add(coin);
                }
            }

            foreach (var collectedCoin in collectedCoins)
            {
                gameEnvironment.coins.Remove(collectedCoin);
            }
        }


        private void CheckPlayerObstacleAndObstacleWallCollision()
        {
            foreach(var obstacle in gameEnvironment.obstacles)
            {
                // Player-Obstacle collision
                if (!insideCheckpoint && gameEnvironment.player.IsCollision(obstacle))
                {
                    fails++;
                    RemoveAllUnsavedCollectedCoins();
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


        private void RemoveAllUnsavedCollectedCoins()
        {
            if (numberOfCoinsSinceLastCheckpointSave > 0)
            {
                coinsCollected -= numberOfCoinsSinceLastCheckpointSave;
                foreach (var collectedCoin in collectedCoins)
                {
                    gameEnvironment.coins.Add(collectedCoin);
                }
                numberOfCoinsSinceLastCheckpointSave = 0;
                collectedCoins.Clear();
            }
        }


        private void CheckPlayerWallCollision()
        {
            if (gameEnvironment.player.isMoving)
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
        }


        public void OnFail()
        {
            gameEnvironment.player.topLeftPosition = gameEnvironment.player.initialTopLeftPosition;
        }
    }
}
