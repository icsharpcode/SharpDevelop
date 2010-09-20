// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	/// <summary>
	/// Derived version of the TestProjectTreeNode class that 
	/// allows us to call the UpdateImageListIndex method directly.
	/// </summary>
	public class DerivedTestProjectTreeNode : TestProjectTreeNode
	{
		public DerivedTestProjectTreeNode(TestProject testProject)
			: base(testProject)
		{
		}
		
		/// <summary>
		/// Calls the base class's UpdateImageListIndex method.
		/// </summary>
		public void CallUpdateImageListIndex(TestResultType result)
		{
			base.UpdateImageListIndex(result);
		}
	}
}
