using System.Collections.Generic;

namespace Rosie.Code.Environment
{
    /// <summary>
    /// Used in NPC dungeon navigation
    /// </summary>
    public class WayPoint
    {
        public WayPoint(int x, int y)
        {
            X = x;
            Y = y;
            ConnectedPoints = new();
        }

        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// A list of rooms directly connected to the current room
        /// </summary>
        public List<WayPoint> ConnectedPoints { get; set; }


        public override string ToString()
        {
            return string.Format("({0}, {1}): ", X, Y);// + string.Join(", ", ConnectedPoints.Select(p => p.ToString()).ToArray());
        }


        public override bool Equals(object obj)
        {
            WayPoint pWP = obj as WayPoint;
            return X == pWP.X && Y == pWP.Y;
        }

    }
}
