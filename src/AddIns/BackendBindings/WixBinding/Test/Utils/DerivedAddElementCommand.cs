// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
