// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents the method that will handle the TestCollection's
	/// TestMethodAdded or TestMethodRemoved events.
	/// </summary>
	public delegate void TestMethodEventHandler(object source, TestMethodEventArgs e);
	
	/// <summary>
	/// Provides data for the TestCollection's TestMethodAdded and TestMethodRemoved events.
	/// </summary>
	public class TestMethodEventArgs
	{
		TestMethod testMethod;
		
		public TestMethodEventArgs(TestMethod testMethod)
		{
			this.testMethod = testMethod;
		}
		
		public TestMethod TestMethod {
			get {
				return testMethod;
			}
		}
	}
}
