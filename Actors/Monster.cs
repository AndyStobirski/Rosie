using Microsoft.Xna.Framework;
using Rosie.Enums;
using Rosie.Map;
using System;

namespace Rosie.Entities
{
    public class Monster : Actor
    {


        public bool CanSeePlayer { get; set; }

        public Monster(Point pP)
        {
            X = pP.X;
            Y = pP.Y;
        }

        public WayPoint RoamTarget { get; set; }

        private MonsterBehaviour _behaviour;
        public MonsterBehaviour Behaviour { get { return _behaviour; } set { _behaviour = value; BehaviourChange?.Invoke(this, EventArgs.Empty); } }

        /// <summary>
        /// Move the monster to the specified coordinates, raising an event for when it happens
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        public override void Move(int pX, int pY)
        {
            ActorMoved?.Invoke(this, new ActorMovedEventArgs(X, Y, pX, pY, this));

            X = pX;
            Y = pY;

            if (pX == RoamTarget.X && pY == RoamTarget.Y)
            {
                FoundTarget?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Has the monster reached his target
        /// </summary>
        /// <returns></returns>
        public bool AtRoamTarget()
        {
            return RoamTarget.X == X && RoamTarget.Y == Y;
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public void Act()
        {
            switch (Behaviour)
            {
                case MonsterBehaviour.Fleeing:

                    break;

                case MonsterBehaviour.Hunting:

                    break;

                case MonsterBehaviour.Passive:

                    break;

                case MonsterBehaviour.Sleeping:

                    break;

                case MonsterBehaviour.Wandering:

                    break;
            }
        }

        public event EventHandler FoundTarget;
        public override event EventHandler<ActorMovedEventArgs> ActorMoved;

        public event EventHandler BehaviourChange;
    }
}
