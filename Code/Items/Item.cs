using System.Drawing;

namespace Rosie.Code.Items
{
    public abstract class Item : Location
    {
        public string Name { get; set; }
        public int Gfx { get; set; }


        public Point Location => new Point(X, Y);

        public Item()
        {
        }

        public void SetLocation(int pX, int pY)
        {
            X = pX;
            Y = pY;
        }

        public Item(int pGfx)
        {
            this.Gfx = pGfx;
        }

    }

}