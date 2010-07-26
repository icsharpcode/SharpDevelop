using SimpleExpressionEvaluator.Compilation;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for Subtract.
    /// </summary>
    [Tokens("-")]
    public class Subtract:NumericOperator
    {
        protected override double EvaluateOperation(double left, double right)
        {
            return left - right;
        }
    }
}