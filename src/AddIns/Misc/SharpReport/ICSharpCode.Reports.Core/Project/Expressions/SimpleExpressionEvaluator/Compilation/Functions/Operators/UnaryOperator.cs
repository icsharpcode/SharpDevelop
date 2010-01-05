using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    [NodeType(ExpressionNodeType.UnaryOperator)]
    public abstract class UnaryOperator<T> : Function<T>
    {
        protected override int ExpectedArgumentCount
        {
            get
            {
                return 1;
            }
        }
        protected override sealed T EvaluateFunction(object[] args)
        {
            object arg = args[0];

            return EvaluateOperation(arg);
        }

        protected abstract T EvaluateOperation(object arg);
    }
}