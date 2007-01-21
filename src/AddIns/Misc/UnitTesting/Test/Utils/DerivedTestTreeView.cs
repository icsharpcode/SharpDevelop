// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		/// <summary>
		/// Calls the base class's OnBeforeSelect method.
		/// </summary>
		public void CallOnBeforeSelect(TreeViewCancelEventArgs e)
		{
			base.OnBeforeSelect(e);
		}
	}
}
