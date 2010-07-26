using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions.Operators;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for And.
    /// </summary>
    [Tokens("&","&&","and")]
    public class And:BooleanOperator
    {
        protected override bool EvaluateOperation(bool left, bool right)
        {
            return left && right;
        }
    }
}