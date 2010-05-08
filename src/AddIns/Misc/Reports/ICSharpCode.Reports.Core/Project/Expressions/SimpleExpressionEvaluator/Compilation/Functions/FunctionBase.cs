using System;
using System.Collections.Generic;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
    public abstract class FunctionBase<T> : ExpressionNode<T>, IExpressionNodeWithArguments
    {
        public const int UnlimitedArguments = -1;

        protected List<IExpression> Arguments { get; set; }

        protected FunctionBase(): this(ExpressionNodeType.Function)
        {
        }

        protected FunctionBase(ExpressionNodeType nodeType):base(nodeType)
        {
            Arguments = new List<IExpression>();
        }

        protected virtual int ExpectedArgumentCount
        {
            get { return UnlimitedArguments; }
        }

        public virtual void AcceptArguments(params IExpression[] arguments)
        {
            if (Arguments.Count > 0 && ExpectedArgumentCount > 0)
                throw new ArgumentException(
                    "AcceptArguments has already been called for this function. AcceptArguments can only be called once.");

            if (arguments != null)
            {
                if (ExpectedArgumentCount != UnlimitedArguments && arguments.Length != ExpectedArgumentCount)
                    throw new ArgumentException(GetType().Name + " expects " + ExpectedArgumentCount +
                                                " but was provided with " + arguments.Length + " arguments.");

                Arguments.AddRange(arguments);
            }
        }

    }
}
