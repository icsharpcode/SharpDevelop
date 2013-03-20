/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.02.2013
 * Time: 20:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
	/// <summary>
	/// DateSubtract (endDate,startDate)
	/// </summary>
	public class DateSubtract: Function<TimeSpan>
	{
		
		protected override int ExpectedArgumentCount
		{
			get{return 2;}
		}
		

		protected override TimeSpan EvaluateFunction(object[] args)
		{

			args[0] = args[0] == null ? DateTime.Today : TypeNormalizer.EnsureType(args[0], typeof (DateTime));

			args[1] = args[1] == null ? DateTime.Today : TypeNormalizer.EnsureType(args[1], typeof (DateTime));

			var endDate = (DateTime) args[0];
			if (endDate == DateTime.MinValue) {
				endDate = DateTime.Today;
			}
			var startDate = (DateTime) args[1];
			if (startDate == DateTime.MinValue)
				startDate = DateTime.Today;
			
			return endDate.Subtract(startDate);
		}
	}
}
	
