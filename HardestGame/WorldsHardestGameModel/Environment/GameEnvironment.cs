using System.Collections.Generic;

using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.ConfigTemplates;

namespace WorldsHardestGameModel.Environment
{
    public class GameEnvironment : IGameEnvironment
    {

        private readonly ILocalSettings localSettings;

        public const int CELL_WIDTH = 60;
        public const int CELL_HEIGHT = 60;

        public Player player { get; set; }
        public List<Wall> walls { get; set; }
        public List<Obstacle> obstacles { get; set; }
        /// <summary>
        /// obstacleBoundaries list is mostly used by obstacles moving
        /// in the X-Y direction This will tell them when to change
        /// directions. It's also used to reduce obstacle collision
        /// check against all all walls in the game environment.
        /// </summary>
        public List<Wall> obstacleBoundaries { get; set; } 
        public int numberOfCoins { get; set; }
        public HashSet<Coin> coins { get; set; }
        public List<CheckPoint> checkPoints { get; set; }
        public bool[,] freeSpaces { get; set; }

        public GameEnvironment(ILocalSettings localSettings)
        {
            this.localSettings = localSettings;
            walls = new List<Wall>();
            obstacles = new List<Obstacle>();
            obstacleBoundaries = new List<Wall>();
            numberOfCoins = 0;
            coins = new HashSet<Coin>();
            checkPoints = new List<CheckPoint>();
            freeSpaces = new bool[localSettings.MapHeight, localSettings.MapWidth];
        }

        public void ClearAll()
        {
            walls.Clear();
            obstacles.Clear();
            obstacleBoundaries.Clear();
            coins.Clear();
            checkPoints.Clear();
            freeSpaces = new bool[localSettings.MapHeight, localSettings.MapWidth];
            numberOfCoins = 0;
        }
    }
}
