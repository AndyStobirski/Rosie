namespace Rosie.Code.Misc
{
    public class Roll
    {
        public Roll(int pMultiplier, int pDice, int pModifier)
        {
            Dice = pDice;
            Multiplier = pMultiplier;
            Modifier = pModifier;
        }

        /// <summary>
        /// The dice to roll
        /// </summary>
        public int Dice { get; private set; }

        /// <summary>
        /// The number of times to roll the dice
        /// </summary>
        public int Multiplier { get; private set; }

        /// <summary>
        /// A value to add to the multiplied dice
        /// </summary>
        public int Modifier { get; private set; }
    }
}
