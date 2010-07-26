using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions.Operators;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for Multiply.
    /// </summary>
    [Tokens("*")]
    public class Multiply:NumericOperator
    {
        protected override double EvaluateOperation(double left, double right)
        {
            return left*right;
        }

    }
}