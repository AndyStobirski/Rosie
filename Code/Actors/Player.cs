using Rosie.Code;
using Rosie.Code.Items;

using Rosie.Code.Misc;
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
            var d = EntityData.RandomWeapon();
            Inventory.Add(d);
            EquipWeapon(d);
            var l = EntityData.RandomArmour();
            Inventory.Add(l);
            EquipArmour(l);
            BaseScent = 10;
        }


        public override void Draw()
        {
            throw new NotImplementedException();
        }




        new public void TakeItem(Item pItem)
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
