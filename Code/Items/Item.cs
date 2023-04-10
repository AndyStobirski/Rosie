using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Rosie.Code.Items
{
    public abstract class Item
    {
        public string Name { get; set; }
        public int Gfx { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Point Location => new Point(X, Y);

        public Item()
        {            
        }

        public Item(int pGfx)
        {
            this.Gfx = pGfx;
        }

    }

}