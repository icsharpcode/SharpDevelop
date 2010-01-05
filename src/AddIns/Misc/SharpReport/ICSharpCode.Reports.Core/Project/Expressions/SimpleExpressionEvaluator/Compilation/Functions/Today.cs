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

using System;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
    /// <summary>
    /// Summary description for Today.
    /// </summary>
    public class Today:Function<DateTime>
    {
        protected override int ExpectedArgumentCount
        {
            get
            {
                return 0;
            }
        }
        protected override DateTime EvaluateFunction(object[] args)
        {
            return DateTime.Today;
        }

    }
}