using Rosie.Code.Misc;
using Rosie.Misc;

namespace Rosie.Code.Actors
{
    public class ScriptBasic : Script
    {

        public ScriptBasic() : base()
        {

        }


        public override void Act()
        {

            if (State == NPC_STATE.Sleeping)
            {
                Sleep();
            }
            else if (CanSeePlayer())
            {
                if (monster.CanAttack(player))
                {
                    State = NPC_STATE.Combat;
                    monster.Attack(player);
                }
                else
                {
                    DirectMoveTowardsPoint(player.X, player.Y);
                }
            }
            else
            {

                if (Roller.Roll(d20) > SleepProb)
                {
                    SetSleep();
                }
                else
                {
                    Wander();
                }


            }
        }
    }
}
