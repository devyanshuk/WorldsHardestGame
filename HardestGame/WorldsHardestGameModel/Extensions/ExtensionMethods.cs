using System.Drawing;
using System.Collections.Generic;

using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.EntityBase;
using WorldsHardestGameModel.Environment;
using WorldsHardestGameModel.MovementTypes;

namespace WorldsHardestGameModel.Extensions
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Check if any of of 4 edges of a square lies within the
        /// boundaries of a circle. Player-Obstacle collision.
        /// Obstacle-Wall collision
        /// </summary>
        /// <param name="circ"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsCollision(this CircularEntity circ, RectangularEntity rect)
        {
            return circ.IsCollision(rect.topLeftPosition) ||
                   circ.IsCollision(new PointF(rect.topLeftPosition.X + rect.width, rect.topLeftPosition.Y)) ||
                   circ.IsCollision(new PointF(rect.topLeftPosition.X, rect.topLeftPosition.Y + rect.height)) ||
                   circ.IsCollision(new PointF(rect.topLeftPosition.X + rect.width, rect.topLeftPosition.Y + rect.height));
        }


        /// <summary>
        /// Given one of the coordinates of player and wall, and width/height of the wall, check if
        /// player lies within the range.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool WithinBounds(float a, float b, float r) => a >= b - r && a <= b + r;



        /// <summary>
        /// ___________                   ________
        /// |         | _________        | wall   |
        /// |         ||   wall  |       |        |
        /// | player  -----R.....|       |     ...|A______
        /// |         | COLLISION|       |_____.__||player|
        /// |_________A..........|             X---.      | => COLLISION
        ///           |__________|                 |______|
        ///   (i)                               (ii)   
        /// 
        /// case(i) player's rightmost hitpoint R (centreX + width / 2)
        /// is within the bounds of the wall in the X-axis and A is within bounds in the Y-axis.
        /// In this case, the player can't move right.
        /// 
        /// case(ii) player's leftmost hitpoint X (centreX - width / 2) is within
        /// the bounds of the wall in the X-axis and A is within bounds in the Y-axis.
        /// In this case, the player can't move left.
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static List<Dir_4> CheckCollision(this Player player, Wall wall)
        {

            var retVal = new List<Dir_4>();

            var playerCentre = new PointF(player.topLeftPosition.X + Player.width / 2,
                                          player.topLeftPosition.Y + Player.height / 2);

            var playerTopY = player.topLeftPosition.Y;
            var playerBottomY = player.topLeftPosition.Y + Player.height;
            var playerLeftX = player.topLeftPosition.X;
            var playerRightX = player.topLeftPosition.X + Player.width;

            var wallCentre = new PointF(wall.topLeftPosition.X + wall.width / 2,
                                        wall.topLeftPosition.Y + wall.height / 2);

            var leftHitPoint = playerCentre.X  - Player.width / 2;
            var rightHitPoint = playerCentre.X + Player.width / 2;
            var upperHitPoint = playerCentre.Y - Player.height / 2;
            var lowerHitPoint = playerCentre.Y + Player.height / 2;

            if (WithinBounds(playerTopY, wallCentre.Y, wall.height / 2 - 1) || WithinBounds(playerBottomY, wallCentre.Y, wall.height / 2 - 1))
            {
                if (WithinBounds(leftHitPoint, wallCentre.X, wall.width / 2))
                {
                    retVal.Add(Dir_4.LEFT);
                }

                if (WithinBounds(rightHitPoint, wallCentre.X, wall.width / 2))
                {
                    retVal.Add(Dir_4.RIGHT);
                }
            }

            if (WithinBounds(playerLeftX, wallCentre.X, wall.width / 2 - 1) || WithinBounds(playerRightX, wallCentre.X, wall.width / 2 - 1))
            {
                if (WithinBounds(upperHitPoint, wallCentre.Y, wall.height / 2))
                {
                    retVal.Add(Dir_4.UP);
                }

                if (WithinBounds(lowerHitPoint, wallCentre.Y, wall.height / 2))
                {
                    retVal.Add(Dir_4.DOWN);
                }

            }

            return retVal;
        }
    }
}
