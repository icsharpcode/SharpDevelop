using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    [Tokens("=~")]
    public class RegexMatches : BinaryOperator<bool>
    {
        protected override bool EvaluateOperation(object left, object right)
        {
            if (left == null && right == null)
                return true;

            if (left == null || right == null)
                return false;
            
            return Regex.IsMatch(left.ToString(), right.ToString(),RegexOptions.IgnoreCase);
        }
    }
}
