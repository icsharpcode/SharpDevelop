// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Description of ClassUnitTestNode.
	/// </summary>
	public class ClassUnitTestNode : UnitTestBaseNode
	{
		TestClass testClass;
		
		public TestClass TestClass {
			get { return testClass; }
		}
		
		public ClassUnitTestNode(TestClass testClass)
		{
			this.testClass = testClass;
		}
		
		public override object Text {
			get { return testClass.Name; }
		}
	}
}
