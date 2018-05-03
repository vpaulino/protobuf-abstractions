using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Serialization.Proto.Schemas.Transformers
{
    internal class SplitByMessagesTransformer : ITransformer
    {

        internal SplitByMessagesTransformer()
        {
            TargetType = null;

        }

       private string FixToken(string token, string qualifier)
        {
            if (token.TrimStart().StartsWith("enum") || token.TrimStart().StartsWith("message"))
                return token;

            return $"{Environment.NewLine}{qualifier} {token}";
        }

       private IEnumerable<string> SplitToken(string token, string separator)
        {
            var messageTokens = token.Split(new string[] { $"{Environment.NewLine}{separator} " }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select<string, string>((stringToken) => string.IsNullOrWhiteSpace(stringToken) ? string.Empty : FixToken(stringToken, separator))
                                            .Where(line => !string.IsNullOrWhiteSpace(line));

            return messageTokens;

        }

        public Type TargetType { get; private set; }

        public bool TryTransform(IEnumerable<string> input, out IEnumerable<string> output)
        {
            List<string> result = new List<string>();

            foreach (var inputMessage in input)
            {
                var messageTokens = SplitToken(inputMessage, "message");

                foreach (var messageToken in messageTokens)
                {
                    var enumTokens = SplitToken(messageToken, "enum");

                    result.AddRange(enumTokens);

                } 

            }

            output = result;

            return result.Count > 0;
 
        }
    }
}
