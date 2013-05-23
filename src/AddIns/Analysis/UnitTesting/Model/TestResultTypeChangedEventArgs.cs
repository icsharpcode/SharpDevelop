// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// EventArgs for ITest.ResultChanged.
	/// </summary>
	public class TestResultTypeChangedEventArgs : EventArgs
	{
		readonly TestResultType oldResult;
		readonly TestResultType newResult;
		
		public TestResultTypeChangedEventArgs(TestResultType oldResult, TestResultType newResult)
		{
			this.oldResult = oldResult;
			this.newResult = newResult;
		}
		
		public TestResultType NewResult {
			get { return newResult; }
		}
		
		public TestResultType OldResult {
			get { return oldResult; }
		}
	}
}
