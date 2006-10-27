// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.PInvokeAddIn
{
	/// <summary>
	/// Displays a dialog so the user can browser for PInvoke signatures and
	/// insert one or more of them into the code.
	/// </summary>
	public class InsertPInvokeSignaturesCommand : AbstractMenuCommand
	{		
		/// <summary>
		/// Starts the command.
		/// </summary>
		public override void Run()
		{
			// Show PInvoke dialog.
			using(InsertPInvokeSignaturesForm form = new InsertPInvokeSignaturesForm()) {
				form.ShowDialog();				
			}
		}
	}
}
