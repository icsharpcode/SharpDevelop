using System;
using System.Collections;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for GreaterThanEqual.
    /// </summary>
    [Tokens(">=")]
    public class GreaterThanEqual:BinaryOperator<bool>
    {
        protected override bool EvaluateOperation(object left, object right)
        {
            if (left == null && right == null) return true;

            if (left == null)
                return false;

            if (right == null)
                return true;

            if (left is string || right is string)
            {
                return StringComparer.CurrentCultureIgnoreCase.Compare(left.ToString(), right.ToString()) >= 0;
            }

            return Comparer.Default.Compare(left, right) >= 0;
        }

		
    }
}