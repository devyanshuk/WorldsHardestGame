using System.Drawing;

namespace WorldsHardestGameModel.EntityBase
{
    public abstract class MovementTypeBase
    {
        public float velocity { get; private set; }
        public PointF currentPosition;

        public MovementTypeBase(float velocity, PointF currentPosition)
        {
            this.velocity = velocity;
            this.currentPosition = currentPosition;
        }

        #region abstract methods

        public abstract void Move();
        public abstract void ChangeDirection();

        #endregion
    }
}
