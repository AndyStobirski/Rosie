using Rosie.Code.Misc;

namespace Rosie.Code.TextGenerators
{
    /// <summary>
    /// Random text generating utilities
    /// </summary>
    class GibberishGenerator
    {

        private static string[] syllables = { "ka", "tu", "mi", "ra", "le", "sa", "to", "na", "mo", "ko", "so", "to", "be", "ki", "shi", "me", "no", "su", "fu", "ha", "yo", "chi", "ka", "i", "ne", "hi", "ru", "mu", "wa", "ro", "se", "te", "bi", "ma", "ni", "ja", "nu", "ka", "me", "do", "ja", "ku", "li", "so", "ma", "fe", "gi", "hu", "ba", "bo", "pu", "ku", "go", "fu", "he", "wa", "ko", "pa", "pi", "ri", "nu", "ze", "mu", "bu", "va", "da", "ge", "je", "lo", "xe", "za", "vu", "ce", "se", "qe", "be", "pe", "we", "no", "ta", "ve", "bi", "de", "zu", "pu", "wo", "ti", "vo", "xe", "ku", "yo", "da", "ga", "do", "ke", "lu", "ni", "qu", "fa", "ha", "ja", "la", "me", "nu", "su", "wi", "xa", "ye", "zo" };

        private static string[] nonVowelSyllables = { "ch", "th", "sh", "ph", "kh", "gh", "bl", "cl", "fl", "gl", "pl", "sl", "br", "cr", "dr", "fr", "gr", "pr", "tr" };

        private static string[] vowelPairs = { "ai", "au", "ea", "ee", "ei", "eu", "ia", "ie", "io", "iu", "oa", "oe", "oi", "ou", "ua", "ue", "ui", "uo" };

        /// <summary>
        /// Generate a nonsense name
        /// </summary>
        /// <returns></returns>
        public static string GenerateName(int pMinSyllables = 1, int pMaxSyllables = 4)
        {
            return Word(true, pMinSyllables, pMaxSyllables);
        }

        /// <summary>
        /// Generate a string of nonsense
        /// </summary>
        /// <param name="pCapitalizeFirstLetter">Bool</param>
        /// <param name="pMinWords"></param>
        /// <param name="pMaxWords"></param>
        /// <param name="pMinSyllables">minimum syllables per word</param>
        /// <param name="pMaxSyllables">maximum syllables per word</param>
        /// <returns></returns>
        public static string Sentence(bool pCapitalizeFirstLetter, int pMinWords, int pMaxWords, int pMinSyllables = 1, int pMaxSyllables = 4)
        {
            string sentence = "";
            int wordCount = RandomWithSeed.Next(pMinWords, pMaxWords);
            for (int i = 0; i < wordCount; i++)
            {
                sentence += Word(false, pMinSyllables, pMaxSyllables);
                sentence += " ";
            }
            sentence = sentence.TrimEnd();
            if (pCapitalizeFirstLetter)
            {
                sentence = char.ToUpper(sentence[0]) + sentence.Substring(1);
            }
            return sentence;
        }

        /// <summary>
        /// Construct a word
        /// </summary>
        /// <param name="pCapitalizeFirstLetter">Bool</param>
        /// <param name="pMinSyllables">minimum syllables</param>
        /// <param name="pMaxSyllables">maximum syllables</param>
        /// <returns>Word</returns>
        public static string Word(bool pCapitalizeFirstLetter, int pMinSyllables, int pMaxSyllables)
        {
            string word = "";

            int syllableCount = RandomWithSeed.Next(pMinSyllables, pMaxSyllables); 
            for (int j = 0; j < syllableCount; j++)
            {
                if (j % 2 == 0)
                {
                    //frequently throw in nonvowels to make the word look normal
                    word += nonVowelSyllables[RandomWithSeed.Next(nonVowelSyllables.Length)];
                }
                else
                {
                    if (RandomWithSeed.Next(2) == 0)//toss a coin
                    {
                        word += vowelPairs[RandomWithSeed.Next(vowelPairs.Length)];
                    }
                    else
                    {
                        word += syllables[RandomWithSeed.Next(syllables.Length)];
                    }
                }
            }
            if (pCapitalizeFirstLetter)
            {
                word = char.ToUpper(word[0]) + word.Substring(1);
            }
            return word;
        }
    }
}
