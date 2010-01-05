using System;
using System.Collections;
using SimpleExpressionEvaluator.Compilation;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for NotEquals.
    /// </summary>
    [Tokens("!=","<>")]
    public class NotEquals:BinaryOperator<bool>
    {
        protected override bool EvaluateOperation(object left, object right)
        {
            if (left == null && right == null)
                return false;

            if (left == null || right == null)
                return true;

            if (left is string || right is string)
                return StringComparer.CurrentCultureIgnoreCase.Compare(left, right) != 0;

            try
            {
                return Comparer.Default.Compare(left, right) != 0;
            }
            catch
            {
                //If the two objects can't be compared, then they certainly aren't equal.
                return true;
            }
        }

    }
}