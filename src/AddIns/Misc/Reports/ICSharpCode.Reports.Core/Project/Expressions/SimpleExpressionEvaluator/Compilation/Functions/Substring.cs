/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.04.2011
 * Time: 19:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;

namespace SimpleExpressionEvaluator.Compilation.Functions
{
	/// <summary>
	/// Description of Substring.
	/// </summary>
	public class Substring:Function<string>
	{
		protected override string EvaluateFunction(object[] args)
		{
			  if (args.Length == 1)
			  {
			  	return args[0].ToString();
			  }
			   string str = args[0].ToString();
			   if (args.Length == 2) {
			   	 var start = Convert.ToInt32(args[1],CultureInfo.CurrentCulture);
			   return str.Substring(start);
			   }
			   else if (args.Length == 3) {
			   	var start = Convert.ToInt32(args[1]);
			   	var len = Convert.ToInt32(args[2]);
			   	  return str.Substring(start,len);
			   }
			   throw new ArgumentException("Wrong number of Arguments");
		} 
	}
}
