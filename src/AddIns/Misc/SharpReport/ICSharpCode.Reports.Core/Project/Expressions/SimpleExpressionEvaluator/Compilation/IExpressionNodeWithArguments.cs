using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Compilation
{
    public interface IExpressionNodeWithArguments
    {
        void AcceptArguments(params IExpression[] arguments);
    }
}