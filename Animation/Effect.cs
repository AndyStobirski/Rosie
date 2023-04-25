using Microsoft.Xna.Framework;

namespace Rosie.Animation
{
    public abstract class Effect
    {
        protected float X { get; set; }
        protected float Y { get; set; }

        public Vector2 Current;
        protected float Speed { get; set; } = 32.0f; // 32 pixels per second

        public abstract void Update(float pDeltaTime);

        public abstract bool Expired(double pGameTime);

        /// <summary>
        /// Text to display
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Sprite number to draw
        /// </summary>
        public int Gfx { get; set; }


        public Color Colour { get; set; } = Color.White;


    }
}
