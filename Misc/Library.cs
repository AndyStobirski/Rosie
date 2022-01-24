using Microsoft.Xna.Framework;

namespace Rosie.Misc
{
    /// <summary>
    /// Static class containing game wide variables
    /// </summary>
    public static class Library
    {
        /// <summary>
        /// Generic list of points which contain 4 directions
        /// </summary>
        public static Point[] Directions = new Point[]
        {
            new Point (0,-1)    //north
            , new Point(0,1)    //south
            , new Point (1,0)   //east
            , new Point (-1,0)  //west
        };

        /// <summary>
        /// Offset for 9 diagonal directions
        /// </summary>
        public static Point[] Directions1 = new Point[]
        {
            new Point (0,-1)    //north
            , new Point(0,1)    //south
            , new Point (1,0)   //east
            , new Point (-1,0)  //west
            , new Point (1,-1)  //northeast
            , new Point(-1,-1)  //northwest
            , new Point (-1,1)  //southwest
            , new Point (1,1)   //southeast
            , new Point(0,0)    //centre
        };

    }

}
