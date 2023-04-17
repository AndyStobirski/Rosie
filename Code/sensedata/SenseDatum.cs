using System.Drawing;

namespace Rosie.Code.sensedata
{
    public class SenseDatum
    {
        public string Name { get; set; }
        public int Gfx { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Point Location => new Point(X, Y);

        public SenseDatum()
        {
        }

    }
}
