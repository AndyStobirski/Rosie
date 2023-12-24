using Rosie.Code.Actors.Scripts;
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

        public string NPCType { get; private set; }



        /// <summary>
        /// Set the initial location and waypoint
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="PWayPoint"></param>
        public void PlaceNPC(int pX, int pY, WayPoint PWayPoint)
        {
            X = pX;
            Y = pY;
            script.SetTargetWayPoint(PWayPoint);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data required to configure the NPC</param>
        public NPC(NPCDatum data)
        {
            SubType = data.SubType;
            Type = data.Type;
            NPCType = data.Name;
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

        public string Description()
        {

            var d = new string[]
            {
                this.NPCType + " (first name " + this.Name + ")"
                , "It's current state is " + this.script.State.ToString()
                , "It's hipoints are " + HitPointsCurrent.ToString() + " / " + HitPointsMax.ToString()
            };

            return String.Join("\r\n", d);
        }
        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}


