using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Evaluation;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator
{
    public class Expression<T> : ExpressionNode<T>
    {
        public Expression(IExpression root):base(ExpressionNodeType.Expression)
        {
            Root = root;
        }

        public IExpression Root { get; protected set; }

        #region IExpression Members

        public override T Evaluate(IExpressionContext context)
        {
            return TypeNormalizer.EnsureType<T>(Root.Evaluate(context));
        }

        #endregion
    }
}