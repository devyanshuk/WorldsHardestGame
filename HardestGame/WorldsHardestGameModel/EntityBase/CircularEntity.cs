using System;
using System.Drawing;

namespace WorldsHardestGameModel.EntityBase
{
    public class CircularEntity
    {
        public const int RADIUS = 30;

        public PointF centre;

        public CircularEntity(PointF centre)
        {
            this.centre = centre;
        }

        private float EuclideanDistance(PointF one, PointF two)
        {
            return (float)Math.Sqrt((one.X - two.X) * (one.X - two.X) +
                                    (one.Y - two.Y) * (one.Y - two.Y));
        }

        public virtual bool IsCollision(PointF pos)
        {
            return EuclideanDistance(centre, pos) <= RADIUS;
        }
    }
}
