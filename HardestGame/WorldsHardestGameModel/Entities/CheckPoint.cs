using System.Drawing;

using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameModel.Entities
{
    public class CheckPoint : RectangularEntity
    {
        public CheckPoint(float width, float height, PointF topLeftPosition)
                         : base(width, height, topLeftPosition)
        {
        }
    }
}
