using Microsoft.Xna.Framework;

namespace Rosie.Animation
{
    /// <summary>
    /// Move the specified text from the starting point, via the provided vector
    /// for a specific amount of time
    /// </summary>
    public class EffectText : Effect
    {
        public EffectText(int pX, int pY, string pText, int pMoveX, int pMoveY, double pElapsedTime, Color pColour)
        {

            Current = new(pX, pY);

            X = pX;
            Y = pY;
            Text = pText;
            Direction = new Vector2(pMoveX, pMoveY);
            LifeSpan = 500;
            LiveTo = pElapsedTime + LifeSpan;
            Colour = pColour;
            Speed = 32;


        }


        /// <summary>
        /// Direction the text is moving
        /// </summary>
        public Vector2 Direction { get; set; }



        /// <summary>
        /// Life of animation in milliseconds
        /// </summary>
        public double LifeSpan { get; set; }

        /// <summary>
        /// Calculate value of absolutegame time that when exceeded
        /// the effect is expired
        /// </summary>
        public double LiveTo { get; set; }

        public override bool Expired(double pGameTime)
        {
            return pGameTime > LiveTo;
        }

        /// <summary>
        /// Move the sprites current location using deltatime
        /// tom make framerame independent movement
        /// </summary>
        public override void Update(float pDeltaTime)
        {
            Current.X += (Direction.X * Speed * pDeltaTime);
            Current.Y += (Direction.Y * Speed * pDeltaTime);
        }

    }
}
