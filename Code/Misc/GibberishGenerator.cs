using Rosie.Misc;
using System;

namespace Rosie.Code.Misc
{
    class GibberishGenerator
    {

        static Roll D8 = new Roll(6, 2);

        private static Random rand = new Random();
        private static string[] syllables = { "ka", "tu", "mi", "ra", "le", "sa", "to", "na", "mo", "ko", "so", "to", "be", "ki", "shi", "me", "no", "su", "fu", "ha", "yo", "chi", "ka", "i", "ne", "hi", "ru", "mu", "wa", "ro", "se", "te", "bi", "ma", "ni", "ja", "nu", "ka", "me", "do", "ja", "ku", "li", "so", "ma", "fe", "gi", "hu", "ba", "bo", "pu", "ku", "go", "fu", "he", "wa", "ko", "pa", "pi", "ri", "nu", "ze", "mu", "bu", "va", "da", "ge", "je", "lo", "xe", "za", "vu", "ce", "se", "qe", "be", "pe", "we", "no", "ta", "ve", "bi", "de", "zu", "pu", "wo", "ti", "vo", "xe", "ku", "yo", "da", "ga", "do", "ke", "lu", "ni", "qu", "fa", "ha", "ja", "la", "me", "nu", "su", "wi", "xa", "ye", "zo" };
        private static string[] nonVowelSyllables = { "ch", "th", "sh", "ph", "kh", "gh", "bl", "cl", "fl", "gl", "pl", "sl", "br", "cr", "dr", "fr", "gr", "pr", "tr" };

        public static string GenerateName(bool capitalizeFirstLetter)
        {
            string name = "";
            int syllableCount = rand.Next(2, Roller.Roll(D8)); // Random number of syllables between 2 and 3
            for (int j = 0; j < syllableCount; j++)
            {
                if (j % 2 == 0)
                {
                    name += nonVowelSyllables[rand.Next(nonVowelSyllables.Length)];
                }
                else
                {
                    name += syllables[rand.Next(syllables.Length)];
                }
            }
            if (capitalizeFirstLetter)
            {
                name = char.ToUpper(name[0]) + name.Substring(1);
            }
            return name;
        }
    }
}
