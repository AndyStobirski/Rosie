namespace Rosie.Enums
{
    public enum GameViewMode
    {
        Game,
        MiniMap
    }

    public enum TileTypes
    {
        Floor
            , Door
            , Wall
    }

    public enum GameStates
    {
        None
        , PlayerTurn
        , EnemyTurn
        , Debugging
    }

    public enum WayPoints
    {
        Room
            , Corridor
            , Door
    }

    public enum MonsterBehaviour
    {
        Passive
            , Sleeping  //  ZZZ
            , Wandering //  roaming, but undecided as to attack 
            , Hunting   //  roaming with the intention of attacking the player on sight
            , Fleeing   //  trying to get as far away as possible from the player
    }

}
