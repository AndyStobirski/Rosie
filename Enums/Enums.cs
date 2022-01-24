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
        Sleep
            , Roam
            , Fight
            , SeekEnemy
    }

}
