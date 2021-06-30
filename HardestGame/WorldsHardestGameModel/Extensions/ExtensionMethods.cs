using System.Drawing;

using WorldsHardestGameModel.EntityBase;

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


        public static bool IsCollision(this RectangularEntity rect, CircularEntity circ)
        {
            return circ.IsCollision(new PointF(circ.centre.X + (float)CircularEntity.RADIUS, circ.centre.Y)) ||
                   circ.IsCollision(new PointF(circ.centre.X - (float)CircularEntity.RADIUS, rect.topLeftPosition.Y)) ||
                   circ.IsCollision(new PointF(circ.centre.X, circ.centre.Y + (float)CircularEntity.RADIUS)) ||
                   circ.IsCollision(new PointF(circ.centre.X + (float)CircularEntity.RADIUS, circ.centre.Y + (float)CircularEntity.RADIUS));
        }


        /// <summary>
        /// Check if any of the 4 edges of a square lies within the
        /// boundaries of a square. Player-wall collision
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static bool IsCollision(this RectangularEntity rect1, RectangularEntity rect2)
        {
            return rect1.IsCollision(rect2.topLeftPosition) ||
                   rect1.IsCollision(new PointF(rect2.topLeftPosition.X + rect2.width, rect2.topLeftPosition.Y)) ||
                   rect1.IsCollision(new PointF(rect2.topLeftPosition.X, rect2.topLeftPosition.Y + rect2.height)) ||
                   rect1.IsCollision(new PointF(rect2.topLeftPosition.X + rect2.width, rect2.topLeftPosition.Y + rect2.width));
        }
    }
}
