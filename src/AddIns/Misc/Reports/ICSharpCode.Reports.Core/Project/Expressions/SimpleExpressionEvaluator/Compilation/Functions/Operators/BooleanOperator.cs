using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for BooleanOperator.
    /// </summary>
    public abstract class BooleanOperator:BinaryOperator<bool>
    {
        protected sealed override bool EvaluateOperation(object left, object right)
        {
            var bLeft = TypeNormalizer.EnsureType<bool>(left);
            var bRight = TypeNormalizer.EnsureType<bool>(right);

            return EvaluateOperation(bLeft, bRight);
        }

        protected abstract bool EvaluateOperation(bool left, bool right);
        
	  	
    }
}