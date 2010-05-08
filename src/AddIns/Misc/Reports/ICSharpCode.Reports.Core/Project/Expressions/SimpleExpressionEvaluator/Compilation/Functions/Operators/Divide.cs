using System;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions.Operators;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for Divide.
    /// </summary>
    [Tokens("/")]
    public class Divide:NumericOperator
    {
        protected override double EvaluateOperation(double left, double right)
        {
            if (right == 0)
                return Double.NaN;
            return left/right;
        }
    }
}