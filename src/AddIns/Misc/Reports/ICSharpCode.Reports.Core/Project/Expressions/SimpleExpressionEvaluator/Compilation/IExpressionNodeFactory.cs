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

using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Compilation;

namespace SimpleExpressionEvaluator.Compilation
{
    /// <summary>
    /// Summary description for IExpressionNodeFactory.
    /// </summary>
    public interface IExpressionNodeFactory
    {
        IExpression CreateBinaryOperator(string token,IExpression leftArgument,IExpression rightArgument);
        
        IExpression CreateUnaryOperator(string token, IExpression argument);

        IExpression CreateFunction(string functionName, params IExpression[] arguments);

        Literal<T> CreateLiteral<T>(T literalValue);

        Variable CreateVariable(string variableName);

        QualifiedName CreateQualifiedName(string[] qualifiedName);
    }
}