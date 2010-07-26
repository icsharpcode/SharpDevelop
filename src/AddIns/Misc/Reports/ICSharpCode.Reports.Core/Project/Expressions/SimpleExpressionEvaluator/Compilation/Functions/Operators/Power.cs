using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.Compilation;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    [Tokens("^")]
    public class Power : NumericOperator
    {
        protected override double EvaluateOperation(double left, double right)
        {
            return Math.Pow(left, right);
        }
    }
}