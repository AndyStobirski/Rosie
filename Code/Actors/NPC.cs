using Microsoft.Xna.Framework;
using Rosie.Code.Actors;
using System;

namespace Rosie.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class NPC : Actor
    {
        public Script script;
        public int ExperienceValue { get; private set; }

        public string MonsterType { get; set; }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="pP">Monster location</param>
        public NPC(Point pP, Script pNPCScript)
        {
            X = pP.X;
            Y = pP.Y;
            VisionRange = 5;
            Name = "Monster";
            ExperienceValue = 10;

            script = pNPCScript;
            script.setMonster(this);
        }

        public void Act()
        {
            script.Act();
        }




        /// <summary>
        /// Move the monster to the specified coordinates, raising an event for when it happens
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        public override void Move(int pX, int pY)
        {
            ActorCompletedTurn?.Invoke(this, new ActorCompeletedTurnEventArgs(X, Y, pX, pY, this));

            X = pX;
            Y = pY;

            //RosieGame.AddMessage("Monster moved to {0},{1}", X, Y);
        }

        #region Overriden Methods 

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Die()
        {
            Died?.Invoke(this, new MonsterDiedEventArgs(this));
        }

        #endregion

        public override event EventHandler<ActorCompeletedTurnEventArgs> ActorCompletedTurn;


        public event EventHandler<MonsterDiedEventArgs> Died;
        public class MonsterDiedEventArgs : EventArgs
        {
            public NPC monster { get; private set; }

            public MonsterDiedEventArgs(NPC pMonster)
            {
                monster = pMonster;
            }
        }
    }
}


