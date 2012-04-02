// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	public delegate void TestFinishedEventHandler(object source, TestFinishedEventArgs e);
	
	public class TestFinishedEventArgs : EventArgs
	{
		TestResult result;
		
		public TestFinishedEventArgs(TestResult result)
		{
			this.result = result;
		}
		
		public TestResult Result {
			get { return result; }
		}
	}
}
