using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Evaluation
{
    public class ChildExpressionContext : IExpressionContext
    {
        public ChildExpressionContext(IExpressionContext parentContext,object contextObject)
        {
            ParentContext = parentContext;
            ContextObject = contextObject;
        }

        public IExpressionContext ParentContext { get; private set; }
        public object ContextObject { get; private set;}

        public object ResolveVariableValue(string variableName)
        {
            return ParentContext.ResolveVariableValue(variableName);
        }

        public Func<object[], object> ResolveFunction(string functionName)
        {
            return ParentContext.ResolveFunction(functionName);
        }

        public object ResolveQualifiedName(object context, string[] qualifiedName)
        {
            return ParentContext.ResolveQualifiedName(context, qualifiedName);
        }
    }
}
