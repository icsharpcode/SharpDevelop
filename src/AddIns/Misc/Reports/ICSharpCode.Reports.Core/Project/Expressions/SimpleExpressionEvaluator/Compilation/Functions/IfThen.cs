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
$Log: /CoreServices/Hsi/ExpressionEvaluator/Parser/Functions/IfThen.cs $
 * 
 * 2     8/15/07 4:22p Nathan_stults
-----------------------------------------------------------------------------
*/

#endregion

using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
    /// <summary>
    /// Summary description for IfThen.
    /// </summary>
    public class IfThen : Function<object>
    {
        protected override int ExpectedArgumentCount
        {
            get { return UnlimitedArguments; }
        }

        protected override object EvaluateFunction(object[] args)
        {
            var bCondition = TypeNormalizer.EnsureType<bool>(args[0]);

            if (bCondition)
                return args[1];

            if (args.Length > 2)
                return args[2];

            return null;
        }
    }
}