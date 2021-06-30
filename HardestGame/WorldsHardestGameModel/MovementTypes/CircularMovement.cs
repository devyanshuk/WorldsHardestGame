using System;
using System.Drawing;

using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameModel.MovementTypes
{
    public class CircularMovement : MovementTypeBase
    {

        private readonly PointF centreOfRotation;

        private float angle;
        private Dir_C currentDir;

        public CircularMovement(float velocity,
                                PointF currentPosition,
                                PointF centreOfRotation,
                                Dir_C currentDir)
                              : base(velocity, currentPosition)
        {
            this.centreOfRotation = centreOfRotation;
            this.currentDir = currentDir;
            angle = GetAngleTo(centreOfRotation, currentPosition);
        }


        private float GetAngleTo(PointF centreOfRot, PointF currPos)
        {
            var retVal = default(float);

            if (currPos.Y == centreOfRot.Y)
            {
                retVal = (float)(currPos.X < centreOfRot.X ?
                                 Math.PI + velocity + Math.PI / 180 :
                                 2 * Math.PI * velocity * Math.PI / 180);
            }
            else if (currPos.X == centreOfRot.X)
            {
                retVal = (float)(currPos.Y < centreOfRot.Y ?
                                 270 * Math.PI / 180 * velocity * Math.PI / 180 :
                                 Math.PI / 2 * velocity * Math.PI / 180);
            }

            return retVal;
        }

        private float EuclideanDistance()
        {
            return (float)Math.Sqrt((centreOfRotation.X - currentPosition.X) * (centreOfRotation.X - currentPosition.X) +
                                    (centreOfRotation.Y - currentPosition.Y) * (centreOfRotation.Y - currentPosition.Y));
        }

        public override void Move()
        {
            var newAngle = (float)(velocity * Math.PI / 180);
            angle += currentDir == Dir_C.CLOCKWISE ? newAngle : -newAngle;
            var distance = EuclideanDistance();
            currentPosition.X = (float)(centreOfRotation.X + distance * Math.Cos(angle));
            currentPosition.Y = (float)(centreOfRotation.Y + distance * Math.Sin(angle));
        }


        public override void ChangeDirection()
        {
            throw new NotImplementedException();
        }
    }
}
