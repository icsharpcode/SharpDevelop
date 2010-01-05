using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator
{
    public interface IExpressionEvaluator
    {
        T EvaluateExpression<T>(string expression, object context);
        T EvaluateExpression<T>(string expression, IExpressionContext context);
        
        IExpression<T> CompileExpression<T>(string expression);
    }
}
