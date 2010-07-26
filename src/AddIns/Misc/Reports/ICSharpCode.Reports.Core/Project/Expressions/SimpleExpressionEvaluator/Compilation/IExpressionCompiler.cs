using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Compilation
{
    public interface IExpressionCompiler
    {
        IExpression<T> CompileExpression<T>(string expression);
    }
}