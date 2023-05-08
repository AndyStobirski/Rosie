using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Map;
using Rosie.Misc;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rosie.Map
{
    /// <summary>
    /// Base class for generators
    /// </summary>
    public abstract class Generator
    {
        protected Point[] Directions { get; set; } = new Point[]
{
            new Point (0,-1)    //north
            , new Point(0,1)    //south
            , new Point (1,0)   //east
            , new Point (-1,0)  //west
};
        protected Point[] Directions1 { get; set; } = new Point[]
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


        /// <summary>
        /// Contains the map
        /// </summary>
        public Level Level { get; set; }
        public abstract Level Build();

        public abstract Level Build(int pWidth, int pHeight);
        public abstract Point GetStartLocation();


        [Category("Cave Generation"), Description("The size of the map"), DisplayName("Map Size")]
        public Size MapSize { get; set; }

        public List<WayPoint> WayPoints { get; set; }

    }
}
