using Rosie.Enums;

namespace Rosie.Code.Actors
{
    public class ScriptBasic : Script
    {
        public ScriptBasic() : base()
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
