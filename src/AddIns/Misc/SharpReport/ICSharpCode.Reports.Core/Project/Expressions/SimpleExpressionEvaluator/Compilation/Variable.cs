using System;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation
{
    [NodeType(ExpressionNodeType.Variable)]
    public class Variable : ExpressionNode<object>
    {
        public Variable(string variableName) : base(ExpressionNodeType.Variable)
        {
            VariableName = variableName;
        }

        public string VariableName { get; protected set; }

        public override object Evaluate(IExpressionContext context)
        {
            if (context != null)
            {
                object val =  context.ResolveVariableValue(VariableName);

                if (val == null && String.Compare(VariableName, "current", true) == 0)
                    return context.ContextObject;

                if (val == null)
                    val = context.ResolveQualifiedName(context.ContextObject, new[] {VariableName});

                if (val != null)
                    return val;
            }
            return VariableName;
        }
    }
}