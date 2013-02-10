/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.02.2013
 * Time: 20:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
	 /// <summary>
   	/// DateSubtract (endDate,startDate)
    /// </summary>
	public class DateSubtract: Function<TimeSpan>
	{
		 private static readonly Regex VALIDATOR = new Regex("[dDwWmMyY]", RegexOptions.Compiled);

        protected override int ExpectedArgumentCount
        {
            get
            {
                return 2;
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

        protected override TimeSpan EvaluateFunction(object[] args)
        {

        	args[0] = args[0] == null ? DateTime.Today : TypeNormalizer.EnsureType(args[0], typeof (DateTime));

        	args[1] = args[1] == null ? DateTime.Today : TypeNormalizer.EnsureType(args[1], typeof (DateTime));

        	var endDate = (DateTime) args[0];
        	
        	var startDate = (DateTime) args[1];
        	if (startDate == DateTime.MinValue)
        		startDate = DateTime.Today;

        	
        	Console.WriteLine(endDate.Subtract(startDate));
        	var s = endDate.Subtract(startDate);
        	return endDate.Subtract(startDate);
        }
    }
}
	
