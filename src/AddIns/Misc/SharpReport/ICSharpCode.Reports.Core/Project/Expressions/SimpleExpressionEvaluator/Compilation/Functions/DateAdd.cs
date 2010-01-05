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
using System.Text.RegularExpressions;
using SimpleExpressionEvaluator.Compilation.Functions;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
    /// <summary>
    /// Add's the specified number of periods to the provided date.
    /// Argument 0 is the date to manipulate
    /// Argument 1 is the period to add d or D is Days, w or W is Weeks, m or M is Months, y or Y is Years
    /// Argument 2 is the number of periods to add
    /// </summary>
    public class DateAdd : Function<DateTime>
    {
        private static readonly Regex VALIDATOR = new Regex("[dDwWmMyY]", RegexOptions.Compiled);

        protected override int ExpectedArgumentCount
        {
            get
            {
                return 3;
            }
        }
        
        private static void ValidatePeriodCode(string code)
        {
            if (String.IsNullOrEmpty(code))
                throw new Exception(
                    "The second argument in DateAdd was not provided. It must be one of the following (case insensitive): d,w,m,y");

            if (code.Length != 1 || !VALIDATOR.IsMatch(code))
            {
                if (String.IsNullOrEmpty(code))
                    throw new Exception(
                        "The second argument in DateAdd was not provided. It must be one of the following (case insensitive): d,w,m,y");
            }
        }

        protected override DateTime EvaluateFunction(object[] args)
        {

            args[0] = args[0] == null ? DateTime.Today : TypeNormalizer.EnsureType(args[0], typeof (DateTime));

            args[1] = args[1].ToString().ToUpper();
            args[2] = TypeNormalizer.EnsureType(args[2], typeof (Int32));
            
            var startDate = (DateTime) args[0];
            if (startDate == DateTime.MinValue)
                startDate = DateTime.Today;

            string code = args[1].ToString();

            ValidatePeriodCode(code);

            var distance = (int) args[2];

            switch(code)
            {
                case "D":
                    return startDate.AddDays(distance);
                case "W":
                    return startDate.AddDays(distance*7);
                case "M":
                    return startDate.AddMonths(distance);
                case "Y":
                    return startDate.AddYears(distance);
                default:
                    return startDate.AddDays(distance);
            }
        }
    }
}