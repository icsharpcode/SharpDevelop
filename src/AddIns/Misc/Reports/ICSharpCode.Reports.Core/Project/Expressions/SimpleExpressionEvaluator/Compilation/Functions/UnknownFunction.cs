using System;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
    public class UnknownFunction : ExpressionNode<object>
    {
        public UnknownFunction(string functionName, IExpression[] args) : base(ExpressionNodeType.Function)
        {
            FunctionName = functionName;
            Arguments = args;
        }

        public string FunctionName { get; set; }
        public IExpression[] Arguments { get; set; }

        public override object Evaluate(IExpressionContext context)
        {
            Func<object[], object> unknownFunction = context != null ? context.ResolveFunction(FunctionName) : null;
            if (unknownFunction != null)
            {
                object[] funcArgs;
                if (Arguments != null && Arguments.Length > 0)
                {
                    funcArgs = new object[Arguments.Length];
                    for (int i = 0; i < Arguments.Length; i++)
                        funcArgs[i] = Arguments[i].Evaluate(context);
                }
                else
                    funcArgs = new object[0];

                return unknownFunction(funcArgs);
            }
            throw new InvalidOperationException("A function specified in the expression (" + FunctionName +
                                                ") does not exist.");
        }
    }
}