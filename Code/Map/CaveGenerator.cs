using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Map;

namespace Rosie.Map
{
    /// <summary>
    /// Code taken from here: http://www.roguebasin.com/index.php?title=Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels
    /// </summary>
    public class CaveGenerator : Generator
    {
        public override Level Build()
        {
            Level = new Level();

            RandomFillMap();

            return Level;
        }

        public override Point GetStartLocation()
        {
            int x, y;
            do
            {
                x = _rnd.Next(0, Level.Map.GetLength(0));
                y = _rnd.Next(0, Level.Map.GetLength(1));
            } while (Level.Map[x, y] == null);
            return new Point(x, y);
        }



        public int MapWidth { get; set; } = 40;
        public int MapHeight { get; set; } = 21;
        public int PercentAreWalls { get; set; } = 40;



        public void MakeCaverns()
        {
            // By initilizing column in the outter loop, its only created ONCE
            for (int column = 0, row = 0; row <= MapHeight - 1; row++)
            {
                for (column = 0; column <= MapWidth - 1; column++)
                {
                    Level.Map[column, row] = PlaceWallLogic(column, row);
                }
            }
        }

        public Tile PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentWalls(x, y, 1, 1);


            if (Level.Map[x, y] != null)
            {
                if (numWalls >= 4)
                {
                    return new Floor();
                }
                if (numWalls < 2)
                {
                    return null;
                }

            }
            else
            {
                if (numWalls >= 5)
                {
                    return new Floor();
                }
            }
            return null;
        }

        public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;

            int iX = startX;
            int iY = startY;

            int wallCounter = 0;

            for (iY = startY; iY <= endY; iY++)
            {
                for (iX = startX; iX <= endX; iX++)
                {
                    if (!(iX == x && iY == y))
                    {
                        if (IsWall(iX, iY))
                        {
                            wallCounter += 1;
                        }
                    }
                }
            }
            return wallCounter;
        }

        bool IsWall(int x, int y)
        {
            // Consider out-of-bound a wall
            if (IsOutOfBounds(x, y))
            {
                return true;
            }

            if (Level.Map[x, y] != null)
            {
                return true;
            }

            if (Level.Map[x, y] == null)
            {
                return false;
            }
            return false;
        }

        bool IsOutOfBounds(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return true;
            }
            else if (x > MapWidth - 1 || y > MapHeight - 1)
            {
                return true;
            }
            return false;
        }

        public void BlankMap()
        {
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    Level.Map[column, row] = null;
                }
            }
        }

        public void RandomFillMap()
        {
            // New, empty map
            Level.Map = new Tile[MapWidth, MapHeight];

            int mapMiddle = 0; // Temp variable
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    // If coordinants lie on the the edge of the map (creates a border)
                    if (column == 0)
                    {
                        Level.Map[column, row] = new Floor();
                    }
                    else if (row == 0)
                    {
                        Level.Map[column, row] = new Floor();
                    }
                    else if (column == MapWidth - 1)
                    {
                        Level.Map[column, row] = new Floor();
                    }
                    else if (row == MapHeight - 1)
                    {
                        Level.Map[column, row] = new Floor();
                    }
                    // Else, fill with a wall a random percent of the time
                    else
                    {
                        mapMiddle = (MapHeight / 2);

                        if (row == mapMiddle)
                        {
                            Level.Map[column, row] = null;
                        }
                        else
                        {
                            Level.Map[column, row] = RandomPercent(PercentAreWalls);
                        }
                    }
                }
            }
        }

        Tile RandomPercent(int percent)
        {
            if (percent >= _rnd.Next(1, 101))
            {
                return new Floor();
            }
            return null;
        }

        public override Level Build(int pWidth, int pHeight)
        {
            throw new System.NotImplementedException();
        }
    }
}
