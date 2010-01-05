using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation
{
    [NodeType(ExpressionNodeType.Literal)]
    public class Literal<T> : ExpressionNode<T>
    {
        public Literal(T value) : base(ExpressionNodeType.Literal)
        {
            Value = value;
        }

        public T Value { get; protected set; }

        public override T Evaluate(IExpressionContext context)
        {
            return Value;
        }
    }
}