namespace Rosie.Code.Misc
{
    public class Roll
    {


        /// <summary>
        /// Roll the dice and the add the modifier to the outcome
        /// 
        /// e.g 6, 2 produces a range of 3 to 8
        /// 
        /// </summary>
        /// <param name="pDice"></param>
        /// <param name="pModifier"></param>
        public Roll(int pDice, int pModifier)
        {
            Dice = pDice;
            Multiplier = 1;
            Modifier = pModifier;
        }

        /// <summary>
        /// Roll the dice multiplier number of times and add modifier
        /// e.g. 2, 6, 3 produces a range of (2 - 12) + 3
        /// </summary>
        /// <param name="pMultiplier"></param>
        /// <param name="pDice"></param>
        /// <param name="pModifier"></param>
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
