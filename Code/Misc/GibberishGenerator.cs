namespace Rosie.Code.Misc
{

    class GibberishGenerator
    {

        private static string[] syllables = { "ka", "tu", "mi", "ra", "le", "sa", "to", "na", "mo", "ko", "so", "to", "be", "ki", "shi", "me", "no", "su", "fu", "ha", "yo", "chi", "ka", "i", "ne", "hi", "ru", "mu", "wa", "ro", "se", "te", "bi", "ma", "ni", "ja", "nu", "ka", "me", "do", "ja", "ku", "li", "so", "ma", "fe", "gi", "hu", "ba", "bo", "pu", "ku", "go", "fu", "he", "wa", "ko", "pa", "pi", "ri", "nu", "ze", "mu", "bu", "va", "da", "ge", "je", "lo", "xe", "za", "vu", "ce", "se", "qe", "be", "pe", "we", "no", "ta", "ve", "bi", "de", "zu", "pu", "wo", "ti", "vo", "xe", "ku", "yo", "da", "ga", "do", "ke", "lu", "ni", "qu", "fa", "ha", "ja", "la", "me", "nu", "su", "wi", "xa", "ye", "zo" };

        private static string[] nonVowelSyllables = { "ch", "th", "sh", "ph", "kh", "gh", "bl", "cl", "fl", "gl", "pl", "sl", "br", "cr", "dr", "fr", "gr", "pr", "tr" };

        private static string[] vowelPairs = { "ai", "au", "ea", "ee", "ei", "eu", "ia", "ie", "io", "iu", "oa", "oe", "oi", "ou", "ua", "ue", "ui", "uo" };

        public static string GenerateName(bool capitalizeFirstLetter)
        {
            string name = "";

            // Random number of syllables between 2 and 3
            int syllableCount = RandomWithSeed.Next(2, 4);

            for (int j = 0; j < syllableCount; j++)
            {
                // Randomly choose syllable type
                int syllableType = RandomWithSeed.Next(3);

                if (syllableType == 0)
                {
                    // Non-vowel syllable
                    name += nonVowelSyllables[RandomWithSeed.Next(nonVowelSyllables.Length)];
                }
                else if (syllableType == 1)
                {
                    // Single vowel syllable
                    name += syllables[RandomWithSeed.Next(syllables.Length)];
                }
                else
                {
                    // Vowel pair syllable
                    name += vowelPairs[RandomWithSeed.Next(vowelPairs.Length)];
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
