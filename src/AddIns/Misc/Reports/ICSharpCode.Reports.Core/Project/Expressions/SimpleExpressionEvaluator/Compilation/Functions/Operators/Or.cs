using SimpleExpressionEvaluator.Compilation;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for Or.
    /// </summary>
    [Tokens("||","or")]
    public class Or:BooleanOperator
    {
        protected override bool EvaluateOperation(bool left, bool right)
        {
            return left || right;
        }

    }
}