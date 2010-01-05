using System;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation
{
    public enum ExpressionNodeType
    {
        Literal,
        Variable,
        BinaryOperator,
        UnaryOperator,
        Function,
        QualifiedName,
        Expression
    }

    public abstract class ExpressionNode<T> : IExpression<T>
    {
        protected ExpressionNode(ExpressionNodeType nodeType)
        {
            NodeType = nodeType;
        }

        public ExpressionNodeType NodeType { get; private set; }

        public virtual Type ReturnType
        {
            get { return typeof(T); }
        }

        #region IExpression Members

        public abstract T Evaluate(IExpressionContext context);

        object IExpression.Evaluate(IExpressionContext context)
        {
            return Evaluate(context);
        }

        #endregion
    }
}