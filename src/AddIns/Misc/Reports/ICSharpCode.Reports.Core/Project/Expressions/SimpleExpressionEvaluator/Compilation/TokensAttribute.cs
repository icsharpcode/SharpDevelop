using System;
using System.Collections.Generic;

namespace SimpleExpressionEvaluator.Compilation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TokensAttribute : Attribute
    {
        public string[] Tokens { get; private set; }

        public TokensAttribute(params string[] tokens)
        {
            Tokens = tokens ?? new string[0];
        }

        public static List<string> FindTokens(Type type)
        {
            if (type == null)
                return null;

            var tokens = new List<string>();
            
            var atts = type.GetCustomAttributes(typeof (TokensAttribute), true);
            if (atts != null && atts.Length > 0)
            {
                foreach (TokensAttribute att in atts)
                {
                    if (att.Tokens != null && att.Tokens.Length > 0)
                    {
                        tokens.AddRange(att.Tokens);
                    }
                }
            }
            return tokens;
        }
    }
}