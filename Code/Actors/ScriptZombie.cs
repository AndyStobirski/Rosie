using Rosie.Entities;
using Rosie.Enums;

namespace Rosie.Code.Actors
{
    public class ScriptZombie : Script
    {

        public ScriptZombie(NPC pMon) : base(pMon)
        {

        }

        public override void Act()
        {
            if (CanSeePlayer())
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
                Wander();
            }
        }

    }
}
