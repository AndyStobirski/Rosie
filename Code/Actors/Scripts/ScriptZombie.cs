using Rosie.Code.Misc;

namespace Rosie.Code.Actors.Scripts
{
    /// <summary>
    /// The most simple script, attempt to move towards and attack the player
    /// if they can be seen else do nothing
    /// </summary>
    public class ScriptZombie : Script
    {

        public ScriptZombie() : base()
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

        }

    }
}
