// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
