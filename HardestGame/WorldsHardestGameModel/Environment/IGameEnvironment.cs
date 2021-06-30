using System.Collections.Generic;

using WorldsHardestGameModel.Entities;

namespace WorldsHardestGameModel.Environment
{
    public interface IGameEnvironment
    {
        Player player { get; set; }

        List<Wall> walls { get; set; }

        List<Wall> obstacleBoundaries { get; set; }

        List<Obstacle> obstacles { get; set; }

        int numberOfCoins { get; set; }

        List<Coin> coins { get; set; }

        List<CheckPoint> checkPoints { get; set; }

        bool[,] freeSpaces { get; set; }


        void ClearAll();
    }
}
