using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    [Tokens(".")]
    [NodeType(ExpressionNodeType.BinaryOperator)]
    public class Dot : FunctionBase<object>
    {
        protected override int ExpectedArgumentCount
        {
            get
            {
                return 2;
            }
        }

        public override object Evaluate(IExpressionContext context)
        {
            IExpression left = Arguments[0];
            object newContext = null;

            if (left != null)
                newContext = left.Evaluate(context);
            var tempContext = new ExpressionContext(newContext);

            IExpression right = Arguments[1];
            if (right != null)
                return right.Evaluate(tempContext);
            return null;
        }
    }
}
