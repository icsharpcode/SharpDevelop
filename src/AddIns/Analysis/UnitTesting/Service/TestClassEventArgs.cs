// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents the class that will handle the TestCollection's
	/// TestClassAdded or TestClassRemoved events.
	/// </summary>
	public delegate void TestClassEventHandler(object source, TestClassEventArgs e);
	
	/// <summary>
	/// Provides data for the TestCollection's TestClassAdded and TestClassRemoved events.
	/// </summary>
	public class TestClassEventArgs
	{
		TestClass testClass;
		
		public TestClassEventArgs(TestClass testClass)
		{
			this.testClass = testClass;
		}
		
		public TestClass TestClass {
			get { return testClass; }
		}
	}
}
