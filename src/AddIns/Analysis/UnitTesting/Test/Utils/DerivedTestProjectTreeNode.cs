// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
