using System;
using System.Drawing;
using System.Collections.Generic;

using WorldsHardestGameModel.EntityBase;
using WorldsHardestGameModel.Environment;

namespace WorldsHardestGameModel.MovementTypes
{
    public class RectangularMovement : MovementTypeBase
    {
        /// <summary>
        /// For cloclwise movement, the direction changes from index 0 to 1.
        /// Vice versa for anticlockwise.
        /// </summary>
        private readonly List<Dir_4> MOVEMENTS = new List<Dir_4>() { Dir_4.UP, Dir_4.RIGHT, Dir_4.DOWN, Dir_4.LEFT };

        private Dir_4 currentDir;
        private Dir_C movementType;

        private PointF boundaryTopLeftPos;
        private float boundaryWidth;
        private float boundaryHeight;

        public RectangularMovement(float velocity,
                                   PointF currentPosition,
                                   Dir_C movementType,
                                   PointF boundaryTopLeftPos,
                                   float boundaryWidth,
                                   float boundaryHeight)
                                 : base(velocity, currentPosition)
        {
            this.movementType = movementType;
            this.boundaryTopLeftPos = boundaryTopLeftPos;
            this.boundaryWidth = boundaryWidth;
            this.boundaryHeight = boundaryHeight;

            currentDir = EvaluateInitialDirBasedOnLocation();
        }

        public override void Move()
        {
            currentPosition.X += currentDir == Dir_4.LEFT ? -velocity :
                                 currentDir == Dir_4.RIGHT ? velocity :
                                 0;

            currentPosition.Y += currentDir == Dir_4.UP ? -velocity :
                                 currentDir == Dir_4.DOWN ? velocity :
                                 0;

            if (TimeToChangeDirection())
            {
                ChangeDirection();
            }
        }

        private void ChangeDirection()
        {
            var index = movementType == Dir_C.ANTICLOCKWISE ? 3 : 1;
            currentDir = MOVEMENTS[(MOVEMENTS.IndexOf(currentDir) + index) % 4];
        }

        private bool TimeToChangeDirection()
        {
            var pos = currentPosition;
            var topLeftPos = boundaryTopLeftPos;
            var height = boundaryHeight;
            var width = boundaryWidth;

            return (pos.X <= (topLeftPos.X + GameEnvironment.CELL_WIDTH / 2) &&
                    pos.Y >= (topLeftPos.Y + height - GameEnvironment.CELL_HEIGHT / 2)) ||

                   (pos.X <= (topLeftPos.X + GameEnvironment.CELL_WIDTH / 2) &&
                    pos.Y <= (topLeftPos.Y + GameEnvironment.CELL_HEIGHT / 2)) ||

                   (pos.X >= (topLeftPos.X + width - GameEnvironment.CELL_WIDTH / 2) &&
                   pos.Y <= (topLeftPos.Y + GameEnvironment.CELL_HEIGHT / 2)) ||

                   (pos.X >= (topLeftPos.X + width - GameEnvironment.CELL_WIDTH / 2) &&
                   pos.Y >= (topLeftPos.Y + height - GameEnvironment.CELL_HEIGHT / 2));
        }


        private Dir_4 EvaluateInitialDirBasedOnLocation()
        {
            var left = Dir_4.LEFT;
            var right = Dir_4.RIGHT;
            var up = Dir_4.UP;
            var down = Dir_4.DOWN;


            var boundary = new PointF(boundaryTopLeftPos.X + GameEnvironment.CELL_WIDTH / 2, boundaryTopLeftPos.Y + GameEnvironment.CELL_HEIGHT / 2);

            if (boundary.X == currentPosition.X)
            {
                if (boundary.Y == currentPosition.Y)
                    return movementType == Dir_C.CLOCKWISE ? right : down;
                if (boundary.Y + boundaryHeight - GameEnvironment.CELL_HEIGHT == currentPosition.Y)
                    return movementType == Dir_C.CLOCKWISE ? up : right;
                return movementType == Dir_C.CLOCKWISE ? up : down;
            }
            if (boundary.Y == currentPosition.Y)
            {
                if (boundary.X + boundaryWidth - GameEnvironment.CELL_WIDTH == currentPosition.X)
                    return movementType == Dir_C.CLOCKWISE ? down : left;
                return movementType == Dir_C.CLOCKWISE ? right : left;
            }
            if (boundary.X + boundaryWidth - GameEnvironment.CELL_WIDTH == currentPosition.X)
            {
                if (boundary.Y + boundaryHeight - GameEnvironment.CELL_HEIGHT == currentPosition.Y)
                    return movementType == Dir_C.CLOCKWISE ? left : up;
                return movementType == Dir_C.CLOCKWISE ? down : up;
            }
            if (boundary.Y + boundaryHeight - GameEnvironment.CELL_HEIGHT == currentPosition.Y)
                return movementType == Dir_C.CLOCKWISE ? left : right;
            return left;
        }


    }
}
