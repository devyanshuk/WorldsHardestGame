using System.Drawing;
using System.Collections.Generic;

using WorldsHardestGameModel.EntityBase;
using WorldsHardestGameModel.MovementTypes;

namespace WorldsHardestGameModel.Entities
{
    public class Player : RectangularEntity
    {
        public new const int width = 40, height = 40;

        public const float velocity = 5.0F;

        /// <summary>
        /// value for a key will be true if the user is
        /// pressing the key. Key will be false when the 
        /// corrsponding key is released.
        /// </summary>
        public Dictionary<Dir_4, bool> keyPresses;

        /// <summary>
        /// value will be true for a Dir_4 type if the
        /// player cannot move in that particular direction.
        /// </summary>
        public Dictionary<Dir_4, bool> blockedDirections;

        public Player(PointF topLeftPosition)
                     : base(width, height, topLeftPosition)
        {
            keyPresses = new Dictionary<Dir_4, bool>()
            {
                { Dir_4.UP, false },
                { Dir_4.DOWN, false },
                { Dir_4.LEFT, false },
                { Dir_4.RIGHT, false }
            };

            blockedDirections = new Dictionary<Dir_4, bool>(keyPresses);
        }

        public void UpdatePosition()
        {
            var dx = keyPresses[Dir_4.LEFT] && !blockedDirections[Dir_4.LEFT] ? -1 :
                     keyPresses[Dir_4.RIGHT] && !blockedDirections[Dir_4.RIGHT] ? 1 :
                     0;

            var dy = keyPresses[Dir_4.UP] && !blockedDirections[Dir_4.UP] ? -1 :
                     keyPresses[Dir_4.DOWN] && !blockedDirections[Dir_4.DOWN] ? 1 :
                     0;

            topLeftPosition.X += dx * velocity;
            topLeftPosition.Y += dy * velocity;
        }


        /// <summary>
        /// If the cast failed, i.e. a key other than U, D, L, R
        /// was pressed, return without updating the dictionary.
        /// </summary>
        /// <param name="newDir"></param>
        public void RegisterNewDirection(Dir_4 newDir)
        {
            try
            {
                keyPresses[newDir] = true;
            }
            catch (KeyNotFoundException)
            {
                return;
            }
        }


        public void DeregisterNewDirection(Dir_4 newDir)
        {
            try
            {
                keyPresses[newDir] = false;
            }
            catch (KeyNotFoundException)
            {
                return;
            }
        }


        public void RegisterUnmovableDirections(Dictionary<Dir_4, bool> unmovableDirections)
        {
            blockedDirections = unmovableDirections;
        }


    }
}
