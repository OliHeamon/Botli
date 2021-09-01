using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Botli.Modules.Maths.Parsing
{
    public class Lexer
    {
        public List<char> Delimiters => new List<char>
        {
            '^', '+', '-', '*', '/', '(', ')', ','
        };

        private Dictionary<string, string> Constants => new Dictionary<string, string>
        {
            ["e"] = Math.E.ToString(),
            ["pi"] = Math.PI.ToString()
        };

        public List<string> Tokenise(string expression, List<string> functionNames)
        {
            string buffer = "";

            List<string> result = new List<string>();

            for (int i = 0; i < expression.Length; i++)
            {
                char character = expression[i];

                bool nextIsNumber = i < expression.Length - 1 && double.TryParse(expression[i + 1].ToString(), out _);
                bool previousIsNumber = i > 0 && double.TryParse(expression[i - 1].ToString(), out _);

                bool nextIsNegativeNumber = character == '-' && nextIsNumber && !previousIsNumber;

                if (Delimiters.Contains(character) && !nextIsNegativeNumber)
                {
                    if (buffer.Length > 0)
                    {
                        result.Add(buffer);
                    }

                    result.Add(character.ToString(CultureInfo.InvariantCulture));

                    buffer = "";
                }
                else
                {
                    buffer += character;
                }
            }

            if (!string.IsNullOrEmpty(buffer))
            {
                result.Add(buffer);
            }

            for (int i = 0; i < result.Count; i++)
            {
                string token = result[i].ToLowerInvariant();

                // Substitute constant tokens for their value.
                if (!functionNames.Contains(token) && Constants.Keys.Contains(token))
                {
                    result[i] = Constants[token];
                }
            }

            return result;
        }
    }
}
