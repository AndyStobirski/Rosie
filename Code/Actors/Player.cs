using Rosie.Code;
using Rosie.Code.Items;
using Rosie.Code.Items.Weapons;
using System;
using System.Collections.Generic;

namespace Rosie.Entities
{
    public class Player : Actor
    {
        public int ExperiencePoints { get; set; }




        public Player()
        {
            Speed = 10;
            Gfx = (int)GFXValues.PLAYER_WIZARD;
            Name = "Player";
            ExperiencePoints = 0;
            Inventory = new List<Item>();
            VisionRange = 8;
            HitPointsCurrent = 100;
            HitPointsMax = 100;
            var d = new Dagger();
            Inventory.Add(d);
            EquipWeapon(d);
            var l = new LeatherArmour();
            Inventory.Add(l);
            EquipArmour(l);
        }


        public override event EventHandler<ActorCompeletedTurnEventArgs> ActorCompletedTurn;

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Move(int pX, int pY)
        {

            int oldX = X;
            int oldY = Y;

            X += pX;
            Y += pY;

            ActorCompletedTurn?.Invoke(this, new ActorCompeletedTurnEventArgs(oldX, oldY, X, Y, this));

        }


        public void TakeItem(Item pItem)
        {
            if (pItem is GoldCoins)
            {
                Gold += (pItem as GoldCoins).Value;
                pItem = null;
            }
            else
            {
                Inventory.Add(pItem);
            }
        }
    }
}
