using System;
using System.Collections;
using SimpleExpressionEvaluator.Compilation;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for Equals.
    /// </summary>
    [Tokens("=","==","is")]
    public class Equals:BinaryOperator<bool>
    {
      
        protected override bool EvaluateOperation(object left, object right)
        {
            if (left == null && right == null)
                return true;

            if (left == null || right == null)
                return false;

            if (left is string || right is string)
                return StringComparer.CurrentCultureIgnoreCase.Compare(left, right) == 0;

            try
            {
                return Comparer.Default.Compare(left, right) == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}