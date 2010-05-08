// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents the method that will handle the TestCollection's
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
			get {
				return testClass;
			}
		}
	}
}
