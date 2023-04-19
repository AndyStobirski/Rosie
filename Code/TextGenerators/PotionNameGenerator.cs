using System;
using System.Linq;

namespace Rosie.Code.TextGenerators
{
    using System;

    public static class PotionGenerator
    {
        // Lists of possible values for potion type, effect, color, and liquid state
        private static readonly string[] PotionTypes = { "Elixir", "Draught", "Potion", "Tincture", "Brew", "Philtre" };
        private static readonly string[] PotionEffects = { "healing", "invisibility", "levitation", "strength", "poison", "mana regeneration", "fire resistance", "frost resistance", "teleportation", "flight", "berserker rage", "speed", "water breathing", "night vision", "beauty", "transformation", "confusion", "memory loss", "courage", "enlightenment" };
        private static readonly string[] PotionColors = { "red", "blue", "green", "yellow", "orange", "purple", "black", "white", "gold", "silver", "pink", "brown", "gray", "transparent", "iridescent", "glowing" };
        private static readonly string[] LiquidStates = { "tepid", "bubbling", "gaseous", "viscous", "smoking", "frothing", "effervescent", "murky", "slimy", "milky", "thick", "sparkling", "crackling" };

        private static readonly Random Random = new Random();

        public static string GeneratePotionDescription()
        {
            // Choose a random potion type, effect, color, and liquid state
            string potionType = PotionTypes[Random.Next(PotionTypes.Length)];
            string potionEffect = PotionEffects[Random.Next(PotionEffects.Length)];
            string potionColor = PotionColors[Random.Next(PotionColors.Length)];
            string liquidState = LiquidStates[Random.Next(LiquidStates.Length)];

            // Generate a random potion description using the chosen values
            string description;
            if (new[] { "invisibility", "levitation", "flight", "water breathing", "night vision" }.Contains(potionEffect))
            {
                description = $"{potionType} of {potionEffect}";
            }
            else
            {
                description = $"{potionType} of {potionEffect} ({potionColor})";
            }

            // Add the liquid state to the description
            description += $", {liquidState}";

            return description;
        }
    }

}
