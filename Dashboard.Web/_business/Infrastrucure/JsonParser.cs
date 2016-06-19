using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Web._business.Infrastrucure
{
    public static class JsonParser
    {
        public static string ParseJson(string json, char characterToReplace, char replacementCharacter)
        {
            var bracketCount = 0;

            var jsonChars = json.ToCharArray();

            for (int i = 0; i < jsonChars.Length; i++)
            {
                var character = jsonChars[i];

                if (character.Equals('{'))
                {
                    bracketCount++;
                }
                else if (character.Equals('}'))
                {
                    bracketCount--;
                }
                else if (character.Equals(characterToReplace) && bracketCount > 0)
                {
                    jsonChars[i] = replacementCharacter;
                }
            }

            var retString = new string(jsonChars);

            return retString;
        }
    }
}