using System.Drawing;

namespace WorldsHardestGameModel.EntityBase
{
    public class RectangularEntity
    {
        public float width;
        public float height;
        public PointF topLeftPosition;

        public RectangularEntity(float width, float height, PointF topLeftPosition)
        {
            this.width = width;
            this.height = height;
            this.topLeftPosition = topLeftPosition;
        }

        public bool IsCollision(PointF pos)
        {
            return pos.X >= topLeftPosition.X &&
                   pos.X <= topLeftPosition.X + width &&
                   pos.Y >= topLeftPosition.Y &&
                   pos.Y <= topLeftPosition.Y + height;
        }
    }
}
