// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
