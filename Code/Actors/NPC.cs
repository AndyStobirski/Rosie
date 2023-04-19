using Microsoft.Xna.Framework;
using Rosie.Code.Actors;
using Rosie.Code.Environment;
using Rosie.Code.GameData;
using Rosie.Code.Misc;
using System;

namespace Rosie.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class NPC : Actor
    {
        public NPC_Type Type { get; private set; }
        public NPC_SubType SubType { get; private set; }

        public Script script;
        public int ExperienceValue { get; private set; }

        public bool CanSee { get; set; } = true;
        public bool CanHear { get; set; } = true;
        public bool CanSmell { get; set; } = true;
        public bool CanSleep { get; set; } = true;
        public bool CanMove { get; set; } = true;

        /// <summary>
        /// Probability will sleep
        /// </summary>
        public int SleepProb { get; set; } = 15;

        /// <summary>
        /// Probability of player scent waking the sleeper
        /// </summary>
        public int ScentWakeUp { get; set; } = 8;

        public int HearSoundThreshold { get; set; } = 8;

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

        public void PlaceNPC(int pX, int pY, WayPoint PWayPoint)
        {
            X = pX;
            Y = pY;
            script.SetTargetWayPoint(PWayPoint);
        }

        public NPC(NPCDatum data)
        {
            SubType = data.SubType;
            Type = data.Type;
            Name = data.Name;
            Speed = data.Speed;
            HitPointsCurrent = HitPointsMax = data.MaxHitPoint;
            Gfx = data.Gfx;
            ExperienceValue = data.ExperienceValue;
            VisionRange = 5;

            Script s = null;

            switch (data.Script)
            {
                case "ScriptZombie":
                    s = new ScriptZombie();
                    break;
                case "ScriptBasic1":
                    s = new ScriptBasic1();
                    break;
                case "ScriptBasic":
                    s = new ScriptBasic();
                    break;
            }

            script = s;
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
            if (!CanMove)
                return;

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


