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
$Log: /CoreServices/Hsi/ExpressionEvaluator/Parser/Functions/ToString.cs $
 * 
 * 1     9/07/06 6:56p Nathan_stults
 * Moved various classes from Hsi.FlightLink.Cad.DomainModel to
 * Hsi.FlightLink.DomainModel
 * 
 * 1     7/24/06 4:03p Alex_wang
 * Migrated from Trinity.
 * 
 * 1     6/29/06 9:55a Alex_wang
 * Initial check-in.
-----------------------------------------------------------------------------
*/
#endregion

using System;


namespace SimpleExpressionEvaluator.Compilation.Functions
{
    /// <summary>
    /// A C# type function that converts the 2nd and the rest of arguments to string using the 1st argument 
    /// as the formatter. If there is only one argument passed in, the string representation of that argument
    /// is returned.
    /// </summary>
    public class ToString : Function<string>
    {
        protected override string EvaluateFunction(object[] args)
        {
            if (args.Length == 1)
                return args[0].ToString();
            if (args.Length > 1)
            {
                int nTarget = args.Length - 1;
                string strFormat = args[0].ToString();
                var newArgs = new object[nTarget];
                Array.Copy(args, 1, newArgs, 0, nTarget);
                try
                {
                    return String.Format(strFormat, newArgs);
                }
                catch (Exception ex)
                {
                    return "#ERROR:" + ex.Message + "#";
                }
            }
            return null;
        }
    }
}