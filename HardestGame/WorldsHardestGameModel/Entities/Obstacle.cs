using System.Drawing;

using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameModel.Entities
{
    public class Obstacle : CircularEntity
    {
        public MovementTypeBase movement;
        public float velocity;

        public Obstacle(PointF centre, float velocity, MovementTypeBase movement)
                       : base(centre)
        {
            this.centre = centre;
            this.velocity = velocity;
            this.movement = movement;
        }

        public void Move()
        {
            movement.Move();
            centre = movement.currentPosition;
        }

    }
}
