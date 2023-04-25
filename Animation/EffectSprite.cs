using Microsoft.Xna.Framework;

namespace Rosie.Animation
{
    /// <summary>
    /// Create a sprite that is moved betwen two points (Location and Target) 
    /// at the specified speed
    /// </summary>
    public class EffectSprite : Effect
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pStartX"></param>
        /// <param name="pStartY"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <param name="gfx"></param>
        public EffectSprite(float pStartX, float pStartY, float targetX, float targetY, int gfx)
        {
            Current = new(pStartX, pStartY);
            Target = new(targetX, targetY);
            Gfx = gfx;

            Speed = 512;

            Direction = Vector2.Normalize(Target - Current);
            Velocity = Direction * Speed;
        }

        /// <summary>
        /// The location to move the sprite to
        /// </summary>
        private Vector2 Target;

        /// <summary>
        /// The threshhold in distance calculation
        /// </summary>
        private float DistanceThreshold { get; set; } = 5.0f;

        /// <summary>
        /// Direction sprite is moving in
        /// </summary>
        private Vector2 Direction;

        /// <summary>
        /// Velocity at which sprite is moving
        /// </summary>
        private Vector2 Velocity;

        /// <summary>
        /// Check to see if the distance between start and finished meets 
        /// the threshold
        /// </summary>
        /// <returns></returns>
        public override bool Expired(double pGameTime)
        {
            return Vector2.Distance(Current, Target) < DistanceThreshold;
        }

        /// <summary>
        /// Move the sprites current location using deltatime
        /// tom make framerame independent movement
        /// </summary>
        /// <param name="pDeltaTime"></param>
        public override void Update(float pDeltaTime)
        {
            Current += Velocity * pDeltaTime;
        }

    }
}