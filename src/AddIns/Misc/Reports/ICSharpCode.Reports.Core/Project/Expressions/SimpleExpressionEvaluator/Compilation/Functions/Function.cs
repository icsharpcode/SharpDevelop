#region Copyright note and modification history
/*
-----------------------------------------------------------------------------
Copyright (c) 2006 HSI Inc. All Rights Reserved.

PROPRIETARY NOTICE: This software has been provided pursuant to a
License Agreement that contains restrictions on its use. This software
contains valuable trade secrets and proprietary information of
HSI Inc and is protected by Federal copyright law. It may not
be copied or distributed in any form or medium, disclosed to third parties,
or used in any manner that is not provided for in said License Agreement,
except with the prior written authorization of HSI Inc.

-----------------------------------------------------------------------------
$Log: $
-----------------------------------------------------------------------------
*/
#endregion

using System.Collections.Generic;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
    /// <summary>
    /// Summary description for Function.
    /// </summary>
    [NodeType(ExpressionNodeType.Function)]
    public abstract class Function<T> : FunctionBase<T>
    {
        protected Function(): this(ExpressionNodeType.Function)
        {
        }

        protected Function(ExpressionNodeType nodeType):base(nodeType)
        {
            Arguments = new List<IExpression>();
        }

        public override T Evaluate(IExpressionContext context)
        {
            var args = new object[Arguments.Count];
            for (int i = 0; i < args.Length; i++)
                args[i] = Arguments[i].Evaluate(context);
            return EvaluateFunction(args);
        }

        protected abstract T EvaluateFunction(params object[] args);

    }
}