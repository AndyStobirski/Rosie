using Rosie.Entities;
using System.Collections.Generic;
using System.Linq;
using Rosie.Misc;

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
                    monster.Attack(player);
                }
                else
                {
                    DirectMoveTowardsPlayer();
                }
            }
        }

    }
}
