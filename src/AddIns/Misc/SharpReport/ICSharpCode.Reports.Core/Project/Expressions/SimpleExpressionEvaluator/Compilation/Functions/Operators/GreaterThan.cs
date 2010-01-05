using System;
using System.Collections;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for GreaterThan.
    /// </summary>
    [Tokens(">")]
    public class GreaterThan:BinaryOperator<bool>
    {
        protected override bool EvaluateOperation(object leftVal, object rightVal)
        {
            if (leftVal == null && rightVal == null)
                return false;
            
            if (leftVal == null)
                return false;
            
            if (rightVal == null)
                return true;

            if (leftVal is string || rightVal is string)
                return StringComparer.CurrentCultureIgnoreCase.Compare(leftVal.ToString(), rightVal.ToString()) > 0;


            try
            {
                return Comparer.Default.Compare(leftVal, rightVal) > 0;
            }
            catch
            {
                return false;
            }
        }

    }
}