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
