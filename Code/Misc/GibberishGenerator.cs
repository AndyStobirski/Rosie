namespace Rosie.Code.Misc
{

    class GibberishGenerator
    {

        private static string[] syllables = { "ka", "tu", "mi", "ra", "le", "sa", "to", "na", "mo", "ko", "so", "to", "be", "ki", "shi", "me", "no", "su", "fu", "ha", "yo", "chi", "ka", "i", "ne", "hi", "ru", "mu", "wa", "ro", "se", "te", "bi", "ma", "ni", "ja", "nu", "ka", "me", "do", "ja", "ku", "li", "so", "ma", "fe", "gi", "hu", "ba", "bo", "pu", "ku", "go", "fu", "he", "wa", "ko", "pa", "pi", "ri", "nu", "ze", "mu", "bu", "va", "da", "ge", "je", "lo", "xe", "za", "vu", "ce", "se", "qe", "be", "pe", "we", "no", "ta", "ve", "bi", "de", "zu", "pu", "wo", "ti", "vo", "xe", "ku", "yo", "da", "ga", "do", "ke", "lu", "ni", "qu", "fa", "ha", "ja", "la", "me", "nu", "su", "wi", "xa", "ye", "zo" };

        private static string[] nonVowelSyllables = { "ch", "th", "sh", "ph", "kh", "gh", "bl", "cl", "fl", "gl", "pl", "sl", "br", "cr", "dr", "fr", "gr", "pr", "tr" };

        private static string[] vowelPairs = { "ai", "au", "ea", "ee", "ei", "eu", "ia", "ie", "io", "iu", "oa", "oe", "oi", "ou", "ua", "ue", "ui", "uo" };

        public static string GenerateName()
        {
            return Generate(true, 1, 1);
        }

        public static string Generate(bool capitalizeFirstLetter, int pMinLength, int pMaxLength)
        {
            string sentence = "";
            int wordCount = RandomWithSeed.Next(pMinLength, pMaxLength);
            for (int i = 0; i < wordCount; i++)
            {
                int syllableCount = RandomWithSeed.Next(1, 4); // Random number of syllables between 1 and 3
                for (int j = 0; j < syllableCount; j++)
                {
                    if (j % 2 == 0)
                    {
                        sentence += nonVowelSyllables[RandomWithSeed.Next(nonVowelSyllables.Length)];
                    }
                    else
                    {
                        if (RandomWithSeed.Next(2) == 0)
                        {
                            sentence += vowelPairs[RandomWithSeed.Next(vowelPairs.Length)];
                        }
                        else
                        {
                            sentence += syllables[RandomWithSeed.Next(syllables.Length)];
                        }
                    }
                }
                sentence += " ";
            }
            sentence = sentence.TrimEnd();
            if (capitalizeFirstLetter)
            {
                sentence = char.ToUpper(sentence[0]) + sentence.Substring(1);
            }
            return sentence;
        }
    }

}
