using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions.Operators;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    [Tokens("!","not")]
    public class Not : UnaryOperator<bool>
    {
        protected override bool EvaluateOperation(object arg)
        {
            var val = TypeNormalizer.EnsureType<bool>(arg);
            return !val;
        }
    }
}