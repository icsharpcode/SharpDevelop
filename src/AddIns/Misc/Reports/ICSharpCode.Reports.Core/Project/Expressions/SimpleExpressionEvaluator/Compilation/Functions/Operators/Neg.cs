using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions.Operators;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    [Tokens("-")]
    public class Neg : UnaryOperator<double>
    {
        protected override double EvaluateOperation(object arg)
        {
            var val = TypeNormalizer.EnsureType<double>(arg);
            return val*-1;
        }
    }
}