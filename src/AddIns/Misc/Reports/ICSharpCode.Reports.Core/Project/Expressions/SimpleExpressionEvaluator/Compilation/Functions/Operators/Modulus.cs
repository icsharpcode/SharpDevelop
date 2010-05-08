using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions.Operators;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for Modulus.
    /// </summary>
    [Tokens("%")]
    public class Modulus:NumericOperator
    {
        protected override double EvaluateOperation(double left, double right)
        {
            if (right == 0.0)
                return 0.0;
            return left%right;
        }

    }
}