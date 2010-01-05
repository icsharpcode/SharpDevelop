using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for NumericOperator.
    /// </summary>
    public abstract class NumericOperator:BinaryOperator<double>
    {
        protected sealed override double EvaluateOperation(object left, object right)
        {
            var dLeft = TypeNormalizer.EnsureType<double>(left);
            var dRight = TypeNormalizer.EnsureType<double>(right);

            return EvaluateOperation(dLeft, dRight);
        }

        protected abstract double EvaluateOperation(double left, double right);

    }
}