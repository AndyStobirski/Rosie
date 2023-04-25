using Microsoft.Xna.Framework;
using Rosie.Code;
using Rosie.Code.Items;
using Rosie.Code.Misc;
using Rosie.Misc;
using System;
using System.Collections.Generic;

namespace Rosie.Entities
{
    /// <summary>
    /// Base class for players and monsters
    /// 
    /// Notes:
    ///     1. The method Attack is where Combat messages are outputted to the console
    /// 
    /// </summary>
    public abstract class Actor : Location
    {
        static NDM d20 = new NDM(20, 0);

        /// <summary>
        /// The lower the number the faster the character, e.g. a character with twice the speed of another will
        /// be half as fast
        /// </summary>
        public int Speed { get; set; }
        public int Gfx { get; set; }
        public int VisionRange { get; set; }
        public int HitPointsMax { get; set; }
        public int HitPointsCurrent { get; set; }
        public int ArmorClass { get; set; }
        public Weapon WeaponPrimary { get; set; }
        public Armour ArmourEquiped { get; set; }

        public string Class { get; set; }
        public string Name { get; set; }
        public int Gold { get; set; }
        public List<Item> Inventory { get; set; }

        public string ID => Class + " " + Name;

        public Point Location => new Point(X, Y);

        /// <summary>
        /// The current level of scent an actor has, used in the generation
        /// of SenseData
        /// </summary>
        public int BaseScent { get; set; }

        #region Abstract methods

        public void ModifyCurrentHitPoints(int pValue)
        {
            HitPointsCurrent += pValue;
            if (HitPointsCurrent < 0)
            {
                RaiseActorActorActivity(this, ActorActivityType.Died, null);
            }
            else
            {
                RaiseActorActorActivity(this, ActorActivityType.Damaged, pValue);
            }
        }

        public abstract void Move(int pX, int pY);

        public abstract void Draw();



        public void TakeItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void DropItem(Item item)
        {
            if (item == WeaponPrimary) WeaponPrimary = null;
            if (item == ArmourEquiped) ArmourEquiped = null;
            Inventory.Remove(item);
        }


        public void EquipArmour(Armour pArmour)
        {
            ArmourEquiped = pArmour;
        }

        public void EquipWeapon(Weapon pWeapon)
        {
            WeaponPrimary = pWeapon;
        }

        #endregion

        /// <summary>
        /// A d20 roll used for attack
        /// </summary>
        /// <returns></returns>
        public int GetAttackValue()
        {
            // TODO Add code for modifiers to the AttackValue - check for weapons, rings etc
            return Roller.Roll(d20);
        }

        /// <summary>
        /// Attack the specified actor, taking into account target defence
        /// </summary>
        /// <param name="pActor">Actor attack</param>
        public void Attack(Actor pActor)
        {
            int AttackerDamage = Roller.Roll(WeaponPrimary.Damage);
            int TargetResistsant = pActor.ArmourEquiped == null ? 0 : Roller.Roll(pActor.ArmourEquiped.Defence);
            int damage = (AttackerDamage - TargetResistsant > 0) ? AttackerDamage - TargetResistsant : 0;

            string msg;

            if (this is Player)
            {
                msg = MessageStrings.Battle_Damage_Player;
                (pActor as NPC).script.State = NPC_STATE.Alert;

            }
            else
            {
                msg = MessageStrings.Battle_Damage_Monster;
            }


            RosieGame.AddMessage(msg, ID, damage.ToString(), TargetResistsant.ToString());

            pActor.ModifyCurrentHitPoints(-damage);
        }

        /// <summary>
        /// Check if the target can be attacked - distance of weapon and attack roll are checked
        /// </summary>
        /// <param name="pActor"></param>
        /// <returns></returns>
        public bool CanAttack(Actor pActor)
        {
            return MapUtils.CellDistance(X, Y, pActor.X, pActor.Y) <= WeaponPrimary.Range
                && (Roller.Roll(d20) >= pActor.ArmorClass);
        }



        #region ActorActivityOccured
        public class ActorActivity : EventArgs
        {
            public ActorActivityType Activity { get; set; }
            public object[] Data { get; set; }
        }
        public event EventHandler<ActorActivity> ActorActivityOccured;
        protected void RaiseActorActorActivity(Actor pSource, ActorActivityType pActivity, params object[] pData)
        {
            ActorActivityOccured?.Invoke(pSource, new ActorActivity() { Data = pData, Activity = pActivity });
        }
        #endregion

        protected void RaiseActorCompletedTurn(Actor pSource, ActorCompeletedTurnEventArgs pArgs)
        {
            ActorCompletedTurn?.Invoke(pSource, pArgs);
        }

        public event EventHandler<ActorCompeletedTurnEventArgs> ActorCompletedTurn;
        public class ActorCompeletedTurnEventArgs : EventArgs
        {
            public ActorCompeletedTurnEventArgs(int pOldX, int pOldY, int pNewX, int pNewY, Actor pInhabitant)
            {
                Before = new Point(pOldX, pOldY);
                After = new Point(pNewX, pNewY);
                Inhabitant = pInhabitant;
            }

            public Actor Inhabitant { get; private set; }
            public Point Before { get; private set; }
            public Point After { get; private set; }
        }

    }
}