using Rosie.Misc;
using System;

namespace Rosie.Entities
{
    public class Player : Actor
    {
        public Player()
        {
            Speed = 5;
        }


        public override event EventHandler<ActorMovedEventArgs> ActorMoved;


        //  we are overriding the abstract clss because: https://stackoverflow.com/a/29831176/500181


        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Move(int pX, int pY)
        {

            int oldX = this.X;
            int oldY = this.Y;

            this.X += pX;
            this.Y += pY;

            ActorMoved?.Invoke(this, new ActorMovedEventArgs(oldX, oldY, X, Y , this));

        }

    }
}
