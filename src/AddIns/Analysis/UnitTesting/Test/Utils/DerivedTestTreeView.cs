// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	/// <summary>
	/// Derives from the TestTreeView class and allows us to directly
	/// call the OnBeforeSelect method.
	/// </summary>
	public class DerivedTestTreeView : TestTreeView
	{
		public DerivedTestTreeView()
			: base(new MockTestFrameworksWithNUnitFrameworkSupport())
		{
		}
		
		/// <summary>
		/// Calls the base class's OnBeforeSelect method.
		/// </summary>
		public void CallOnBeforeSelect(TreeViewCancelEventArgs e)
		{
			base.OnBeforeSelect(e);
		}
	}
}
