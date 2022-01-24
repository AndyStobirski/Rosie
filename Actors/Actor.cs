using Microsoft.Xna.Framework;
using System;

namespace Rosie.Entities
{
    public abstract class Actor
    {
        /// <summary>
        /// The lower the number the faster the character, e.g. a character with twice the speed of another will
        /// be half as fast
        /// </summary>
        public int Speed { get; set; } = 10;
        public int Gfx { get; set; } = 0;
        public int X { get; set; }
        public int Y { get; set; }
        public int VisionRange { get; set; } = 5;

        public Point Location => new Point(X, Y);

        public abstract event EventHandler<ActorMovedEventArgs> ActorMoved;

        public abstract void Move(int pX, int pY);

        public abstract void Draw();

        public class ActorMovedEventArgs : EventArgs
        {
            public ActorMovedEventArgs(int pOldX, int pOldY, int pNewX, int pNewY, Actor pInhabitant)
            {
                Before = new Point(pOldX, pOldY);
                After = new Point(pNewX, pNewY);
                Inhabitant = pInhabitant;
            }

            public Actor Inhabitant {get;set;}

            public Point Before { get; set; }
            public Point After { get; set; }
        }

    }
}