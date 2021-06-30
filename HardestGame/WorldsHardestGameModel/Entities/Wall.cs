using System.Drawing;

using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameModel.Entities
{
    public class Wall : RectangularEntity
    {
        public Wall(int width, int height, PointF topLeftPosition)
                   : base(width, height, topLeftPosition)
        {
        }
    }
}
