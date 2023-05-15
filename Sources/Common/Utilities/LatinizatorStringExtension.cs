namespace ClubbyBook.Common.Utilities
{
    using System.Collections.Generic;
    using System.Text;

    public static class LatinizatorStringExtension
    {
        private static Dictionary<char, string> convertDict = new Dictionary<char, string> {
          {'а', "a"},
          {'б', "b"},
          {'в', "v"},
          {'г', "g"},
          {'д', "d"},
          {'е', "e"},
          {'ё', "yo"},
          {'ж', "zh"},
          {'з', "z"},
          {'и', "i"},
          {'й', "j"},
          {'к', "k"},
          {'л', "l"},
          {'м', "m"},
          {'н', "n"},
          {'о', "o"},
          {'п', "p"},
          {'р', "r"},
          {'с', "s"},
          {'т', "t"},
          {'у', "u"},
          {'ф', "f"},
          {'х', "h"},
          {'ц', "c"},
          {'ч', "ch"},
          {'ш', "sh"},
          {'щ', "sch"},
          {'ъ', "j"},
          {'ы', "i"},
          {'ь', "j"},
          {'э', "e"},
          {'ю', "yu"},
          {'я', "ya"},
          {'А', "A"},
          {'Б', "B"},
          {'В', "V"},
          {'Г', "G"},
          {'Д', "D"},
          {'Е', "E"},
          {'Ё', "Yo"},
          {'Ж', "Zh"},
          {'З', "Z"},
          {'И', "I"},
          {'Й', "J"},
          {'К', "K"},
          {'Л', "L"},
          {'М', "M"},
          {'Н', "N"},
          {'О', "O"},
          {'П', "P"},
          {'Р', "R"},
          {'С', "S"},
          {'Т', "T"},
          {'У', "U"},
          {'Ф', "F"},
          {'Х', "H"},
          {'Ц', "C"},
          {'Ч', "Ch"},
          {'Ш', "Sh"},
          {'Щ', "Sch"},
          {'Ъ', "J"},
          {'Ы', "I"},
          {'Ь', "J"},
          {'Э', "E"},
          {'Ю', "Yu"},
          {'Я', "Ya"}
        };

        private static IList<char> replaceableSymbols = new List<char>() {
          ' ',
          '\t',
          '.',
          '_',
          '-',
          ',',
          ':',
          ';',
          '!',
          '?',
          '+',
          '#',
          '@',
          '\'',
          '<',
          '>'
        };

        public static string Latinize(this string sender)
        {
            string originalString = sender;

            StringBuilder sb = new StringBuilder();

            bool isOldLastSpace = false;

            originalString = originalString.Trim();

            foreach (char c in originalString)
            {
                if (!ValidateSymbol(c))
                    continue;

                if (replaceableSymbols.Contains(c))
                {
                    if (!isOldLastSpace)
                    {
                        isOldLastSpace = true;
                        sb.Append("-");
                    }
                }
                else
                    isOldLastSpace = false;

                if (isOldLastSpace)
                    continue;

                if (convertDict.ContainsKey(c))
                    sb.Append(convertDict[c]);
                else
                    sb.Append(c);
            }

            int symbolCountToRemoveFromEnd = 0;
            for (int i = sb.Length - 1; i >= 0 && sb[i] == '-'; i--, symbolCountToRemoveFromEnd++)
                ;
            int symbolCountToRemoveFromBegin = 0;
            for (int i = 0; i < sb.Length - 1 && sb[i] == '-'; i++, symbolCountToRemoveFromBegin++)
                ;

            return sb.ToString(symbolCountToRemoveFromBegin, sb.Length - symbolCountToRemoveFromEnd - symbolCountToRemoveFromBegin);
        }

        private static bool ValidateSymbol(char ch)
        {
            return (ch >= 'а' && ch <= 'я') || (ch >= 'А' && ch <= 'Я') ||
                   (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') ||
                   (ch >= '0' && ch <= '9') || replaceableSymbols.Contains(ch);
        }
    }
}