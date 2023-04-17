using Microsoft.Xna.Framework;

namespace Rosie.Code.Misc
{
    public static class Library
    {
        public static readonly Point[] Directions =
        {
                  new Point(-1, 1),  new Point(0, 1),   new Point(1, 1)
                , new Point(-1, 0),  new Point(0, 0),   new Point(1, 0)
                , new Point(-1, -1), new Point(0, -1),  new Point(1,-1)
        };

        public static readonly Point[] Directions1 =
        {
                  new Point(-1, 1),  new Point(0, 1),   new Point(1, 1)
                , new Point(-1, 0),   new Point(1, 0)
                , new Point(-1, -1), new Point(0, -1),  new Point(1,-1)
        };
    }
}
