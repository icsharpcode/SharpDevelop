// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.PackageManagement
{
	public class AggregateExceptionErrorMessage
	{
		AggregateException ex;
		StringBuilder errorMessage = new StringBuilder();
		
		public AggregateExceptionErrorMessage(AggregateException ex)
		{
			this.ex = ex;
			BuildErrorMessage();
		}
		
		void BuildErrorMessage()
		{
			BuildErrorMessage(ex.InnerExceptions);
		}
		
		void BuildErrorMessage(IEnumerable<Exception> exceptions)
		{
			foreach (Exception ex in exceptions) {
				var aggregateEx = ex as AggregateException;
				if (aggregateEx != null) {
					BuildErrorMessage(aggregateEx.InnerExceptions);
				} else {
					errorMessage.AppendLine(ex.Message);
				}
			}
		}
		
		public override string ToString()
		{
			return errorMessage.ToString().TrimEnd();
		}
	}
}
