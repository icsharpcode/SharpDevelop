using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for BinaryOperator.
    /// </summary>
    [NodeType(ExpressionNodeType.BinaryOperator)]
    public abstract class BinaryOperator<T>:Function<T>
    {
        protected BinaryOperator():base(ExpressionNodeType.BinaryOperator)
        {
            AutoNormalizeArguments = true;
        }

        protected bool AutoNormalizeArguments
        {
            get; set;
        }

        protected override int ExpectedArgumentCount
        {
            get
            {
                return 2;
            }
        }

        protected sealed override T EvaluateFunction(object[] args)
        {
            var left = args[0];
            var right = args[1];

            if (AutoNormalizeArguments)
                TypeNormalizer.NormalizeTypes(ref left, ref right);

            return EvaluateOperation(left, right);
        }

        protected abstract T EvaluateOperation(object left, object right);
	
    }
}