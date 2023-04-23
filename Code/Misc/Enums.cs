namespace Rosie.Code.Misc
{
    public enum GameViewMode
    {
        Game,
        MiniMap,
        InputHandlerMode
    }

    public enum GameViewSubMode
    {
        None,
        Inventory_Drop,
        Inventory_Equip
    }

    public enum GameStates
    {
        None
        , PlayerTurn
        , EnemyTurn
        , AwaitingMouseInput
        , Debugging
        , WaitingMouseClick
    }



    public enum keys
    {
        escape = 27,
        questionMark = 191,

        keypad1 = 97,
        keypad2 = 98,
        keypad3 = 99,
        keypad4 = 100,
        keypad5 = 101,
        keypad6 = 102,
        keypad7 = 103,
        keypad8 = 104,
        keypad9 = 105,

        keyA = 65,
        keyB = 66,
        keyC = 67,
        keyD = 68,
        keyE = 69,
        keyF = 70,
        keyG = 71,
        keyH = 72,
        keyI = 73,
        keyJ = 74,
        keyK = 75,
        keyL = 76,
        keyM = 77,
        keyN = 78,
        keyO = 79,
        keyP = 80,
        keyQ = 81,
        keyR = 82,
        keyS = 83,
        keyT = 84,
        keyU = 85,
        keyV = 86,
        keyW = 87,
        keyX = 88,
        keyY = 89,
        keyZ = 90,


        key0 = 48,


        keyLessThan = 188,
        keyGreaterThan = 190


    }


    public enum DisplayInformation
    {
        None,
        Inventory,
        Direction,
        Drop,
        Equip,
        ChooseDirection,
        SelectCell
    }

    public enum CommandType
    {
        Move, Take, Drop, Equip, Open, Close, StairsMove, MiniMap, Look
    }

    public enum NPC_STATE
    {
        Combat, Alert, Exploring, Sleeping, Idle, TrackScent

    }

    public enum NPC_Type
    {
        Alive
            , Undead
    }

    public enum NPC_SubType
    {
        Skeleton
        , Orc
        , Amphibian
        , Insect
    }

    public enum WEAPON_TYPE
    {
        None, Sword, Dagger, Hammer, Mace, PoleArm
    }

    public enum WEAPON_SUBTYPE
    {
        Fists
            , Sword_Small
            , Sword
            , Sword_Bastard
            , Dagger
            , Dagger_MainGauche
            , Hammer
            , Hammer_WarHammer
            , Mace
            , Mace_WoodenClub
            , PoleArm_Spear
            , PoleArm_Pike
    }

    public enum Armour_Type
    {
        Cloth,
        Leather,
        Ring,
        Chain,
        Plate
    }

    public enum Armour_SubType
    {
        Cloth
            , Leather
            , Ring
            , Chain
            , Plate
    }

}
