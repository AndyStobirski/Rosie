using Rosie.Enums;

namespace Rosie.Map
{
    /// <summary>
    /// Placed in the map and used by monsters to navigate
    /// </summary>
    public class WayPoint
    {
        public WayPoints Iam { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int ID { get; set; }

    }
}
