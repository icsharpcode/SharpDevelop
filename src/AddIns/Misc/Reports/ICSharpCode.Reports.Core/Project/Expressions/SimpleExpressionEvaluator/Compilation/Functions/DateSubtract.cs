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
