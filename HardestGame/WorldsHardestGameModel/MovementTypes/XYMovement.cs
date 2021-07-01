using System.Drawing;

using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameModel.MovementTypes
{
    public class XYMovement : MovementTypeBase
    {
        public Dir_4 currentDir;

        public XYMovement(float velocity,
                          PointF currentPosition,
                          Dir_4 currentDir)
                        : base(velocity, currentPosition)
        {
            this.currentDir = currentDir;
        }

        public override void Move()
        {
            currentPosition.X += currentDir == Dir_4.LEFT ? -velocity :
                                 currentDir == Dir_4.RIGHT ? velocity :
                                 0;

            currentPosition.Y += currentDir == Dir_4.UP ? -velocity :
                                 currentDir == Dir_4.DOWN ? velocity :
                                 0;
        }

        public void ChangeDirection()
        {
            currentDir = currentDir == Dir_4.LEFT ? Dir_4.RIGHT :
                         currentDir == Dir_4.RIGHT ? Dir_4.LEFT :
                         currentDir == Dir_4.UP ? Dir_4.DOWN :
                         Dir_4.UP;
        }
    }
}
