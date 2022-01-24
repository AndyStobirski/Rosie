using Microsoft.Xna.Framework;
using Rosie.Misc;
using System;
using System.ComponentModel;

namespace Rosie.Map
{
    public abstract class Generator
    {
        protected Random _rnd = new Random(12345);
        /// <summary>
        /// Contains the map
        /// </summary>
        public CurrentLevel Level { get; set; }
        public abstract CurrentLevel Build();
        public abstract Point GetStartLocation();


        [Category("Cave Generation"), Description("The size of the map"), DisplayName("Map Size")]
        public Size MapSize { get; set; }

    }
}
