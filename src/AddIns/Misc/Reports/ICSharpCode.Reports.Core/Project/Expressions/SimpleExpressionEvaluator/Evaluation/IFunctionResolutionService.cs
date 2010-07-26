using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Evaluation
{
    public interface IFunctionResolutionService
    {
        Func<object[], object> ResolveFunction(string functionName);
    }
}