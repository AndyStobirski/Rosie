namespace Rosie.Enums
{
    public enum GameViewMode
    {
        Game,
        MiniMap,
        IO
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
        , Debugging
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


        key0 = 48


    }


    public enum DisplayInformation
    {
        Inventory,
        Direction,
        Drop,
        Equip,
        ChooseDirection
    }

    public enum CommandType
    {
        Move, Take, Drop, Equip, Open, Close
    }



}
