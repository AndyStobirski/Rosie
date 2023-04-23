using Rosie.Code.GameData;
using Rosie.Code.Items;
using Rosie.Code.Misc;
using Rosie.Entities;
using System.Collections.Generic;

namespace Rosie.Code
{
    /// <summary>
    /// Data for items is stored here
    /// </summary>
    public static class EntityData
    {
        public static List<WeaponDatum> Weapons { get; private set; } = new()
            {
            new WeaponDatum(WEAPON_TYPE.None,WEAPON_SUBTYPE.Fists,"Small Sword", 1,725,1, 4,0),
                new WeaponDatum(WEAPON_TYPE.Sword,WEAPON_SUBTYPE.Sword_Small,"Small Sword", 1,725,1, 4,0)
                ,new WeaponDatum(WEAPON_TYPE.Sword,WEAPON_SUBTYPE.Sword,"Sword", 1,729,1, 6,0)
                ,new WeaponDatum(WEAPON_TYPE.Sword,WEAPON_SUBTYPE.Sword_Bastard,"Bastard Sword", 2,732,1, 8,0)
                ,new WeaponDatum(WEAPON_TYPE.Dagger,WEAPON_SUBTYPE.Dagger,"Dagger", 1,722,1, 2,0)
                ,new WeaponDatum(WEAPON_TYPE.Dagger,WEAPON_SUBTYPE.Dagger_MainGauche,"Main Gauche", 1,723,1, 4,0)
                ,new WeaponDatum(WEAPON_TYPE.Hammer,WEAPON_SUBTYPE.Hammer_WarHammer,"War Hammer", 1,1125,1, 6,0)
                ,new WeaponDatum(WEAPON_TYPE.Mace,WEAPON_SUBTYPE.Mace,"Mace", 1,707,1, 6,0)
                ,new WeaponDatum(WEAPON_TYPE.Mace,WEAPON_SUBTYPE.Mace_WoodenClub,"Wooden Club", 1,706,1, 8,0)
                ,new WeaponDatum(WEAPON_TYPE.PoleArm,WEAPON_SUBTYPE.PoleArm_Spear,"Spear", 2,761,1, 10,0)
                ,new WeaponDatum(WEAPON_TYPE.PoleArm,WEAPON_SUBTYPE.PoleArm_Pike,"Pike", 3,758,1, 12,0)
            };
        public static List<ArmourDatum> Armours { get; private set; } = new()
            {
                new ArmourDatum("Cloth Armour", 806, Armour_Type.Cloth, Armour_SubType.Cloth,1,2,0)
                , new ArmourDatum("Leather Armour", 811, Armour_Type.Leather, Armour_SubType.Leather,1,4,0)
                , new ArmourDatum("Ring Mail Armour", 814,Armour_Type.Ring, Armour_SubType.Ring,1,6,0)
                , new ArmourDatum("Chainmail Armour",822, Armour_Type.Chain, Armour_SubType.Chain, 1,8,0)
                , new ArmourDatum("Plate Armour", 827,Armour_Type.Plate, Armour_SubType.Plate, 1,10,0)
            };

        public static List<NPCDatum> NPCs { get; private set; } = new()
        {
            new NPCDatum(NPC_Type.Undead, NPC_SubType.Skeleton, "Skeleton", 10,10,"ScriptZombie",(int)GFXValues.MONSTER_SKELETON, 10)
            ,new NPCDatum(NPC_Type.Alive, NPC_SubType.Orc, "Orc", 10,10,"ScriptBasic",(int)GFXValues.MONSTER_ORC, 10)
            ,new NPCDatum(NPC_Type.Alive, NPC_SubType.Orc, "Orc Knight", 10,10,"ScriptBasic1",(int)GFXValues.MONSTER_ORCKNIGHT, 20)
            ,new NPCDatum(NPC_Type.Alive, NPC_SubType.Orc, "Orc Warrior", 10,10,"ScriptBasic1",(int)GFXValues.MONSTER_ORCWARRIOR, 50)
            ,new NPCDatum(NPC_Type.Alive, NPC_SubType.Amphibian, "Giant Frog", 10,10,"ScriptBasic",(int)GFXValues.MONSTER_GIANTFROG, 50)
       ,new NPCDatum(NPC_Type.Alive, NPC_SubType.Insect, "Giant Spider", 10,10,"ScriptZombie",(int)GFXValues.MONSTER_GIANTSPIDER, 50)
            , new NPCDatum(NPC_Type.Undead, NPC_SubType.Skeleton, "Flying Skull", 5,10,"ScriptZombie",(int)GFXValues.MONSTER_FLYINGSKULL, 10)
            , new NPCDatum(NPC_Type.Alive, NPC_SubType.Insect, "Fire Ant", 5,10,"ScriptZombie",(int)GFXValues.MONSTER_FIREANT, 10)
        };

        public static Armour RandomArmour()
        {
            return new Armour(Armours[RandomWithSeed.Next(Armours.Count)]);
        }

        public static Weapon RandomWeapon()
        {
            return new Weapon(Weapons[RandomWithSeed.Next(Weapons.Count)]);
        }

        public static NPC RandomNPC()
        {
            return new NPC(NPCs[RandomWithSeed.Next(NPCs.Count)]);
        }
    }
}
