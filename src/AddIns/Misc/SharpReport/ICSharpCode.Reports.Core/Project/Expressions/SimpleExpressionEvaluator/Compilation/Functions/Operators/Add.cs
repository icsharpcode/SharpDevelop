using System;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions.Operators;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for Add.
    /// </summary>
    [Tokens("+")]
    public class Add : BinaryOperator<object>
    {
        public Add()
        {
            //This can concatenate strings, so 
            //normalization may fail in several
            //cases. Therefore, we'll 
            //deal with type normalization manually.
            AutoNormalizeArguments = false;
        }

        protected override object EvaluateOperation(object left, object right)
        {
            if (left == null)
                return right;

            if (right == null)
                return left;

            if ((left is string) ||
                (right is string))
                return left.ToString() + right;

            if (left is DateTime && right is int)
                return ((DateTime) left).AddDays((int) right);

            if (left is DateTime && right is TimeSpan)
                return ((DateTime) left).Add((TimeSpan) right);
            
            var nLeft = TypeNormalizer.EnsureType<double>(left);
            var nRight = TypeNormalizer.EnsureType<double>(right);

            return nLeft + nRight;
        }
    }
}