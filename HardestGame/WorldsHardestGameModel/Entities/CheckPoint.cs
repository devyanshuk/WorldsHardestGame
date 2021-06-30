using System.Drawing;

using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameModel.Entities
{
    public class CheckPoint : RectangularEntity
    {

        #region checkpoint aniation

        public int opacityVelocity;

        #endregion

        public CheckPoint(float width, float height, PointF topLeftPosition)
                         : base(width, height, topLeftPosition)
        {
        }
    }
}
