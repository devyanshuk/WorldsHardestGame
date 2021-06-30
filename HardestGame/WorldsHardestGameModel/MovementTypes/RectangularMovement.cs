using System;
using System.Drawing;

using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameModel.MovementTypes
{
    public class RectangularMovement : MovementTypeBase
    {
        private Dir_4 currentDir;
        private Dir_C movementType;

        private PointF boundaryTopLeftPos;
        private float boundaryWidth;
        private float boundaryHeight;

        public RectangularMovement(float velocity,
                                   PointF currentPosition,
                                   Dir_4 currentDir,
                                   Dir_C movementType,
                                   PointF boundaryTopLeftPos,
                                   float boundaryWidth,
                                   float boundaryHeight)
                                 : base(velocity, currentPosition)
        {
            this.currentDir = currentDir;
            this.movementType = movementType;
            this.boundaryTopLeftPos = boundaryTopLeftPos;
            this.boundaryWidth = boundaryWidth;
            this.boundaryHeight = boundaryHeight;
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

        public override void ChangeDirection()
        {
            throw new NotImplementedException();
        }

        private bool TimeToChangeDirection()
        {
            var pos = currentPosition;
            var topLeftPos = boundaryTopLeftPos;
            var height = boundaryHeight;
            var width = boundaryWidth;

            return (pos.X <= topLeftPos.X && pos.Y >= topLeftPos.Y + height) ||
                   (pos.X <= topLeftPos.X && pos.Y <= topLeftPos.Y) ||
                   (pos.X >= topLeftPos.X + width && pos.Y <= topLeftPos.Y) ||
                   (pos.X >= topLeftPos.X + width && pos.Y >= topLeftPos.Y + height);
        }


    }
}
