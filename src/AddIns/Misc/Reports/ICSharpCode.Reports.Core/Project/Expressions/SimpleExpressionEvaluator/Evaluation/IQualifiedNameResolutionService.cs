using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Evaluation
{
    public interface IQualifiedNameResolutionService
    {
        object ResolveQualifiedName(object context, string[] qualifiedName);
    }
}