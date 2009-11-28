// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;

namespace WixBinding.Tests.Utils
{
	public class DerivedAddElementCommand : AddElementCommand
	{
		public DerivedAddElementCommand(string name, IWorkbench workbench)
			: base(name, workbench)
		{
		}
		
		public void CallOnClick()
		{
			base.OnClick(new EventArgs());
		}
	}
}
