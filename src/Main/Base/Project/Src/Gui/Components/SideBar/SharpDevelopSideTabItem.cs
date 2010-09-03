// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SharpDevelopSideTabItem : SideTabItem
	{
		public SharpDevelopSideTabItem(string name)
			: base(name)
		{
			Icon = WinFormsResourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag)
			: base(name, tag)
		{
			Icon = WinFormsResourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag, Bitmap icon)
			: base(name, tag, icon)
		{
		}
	}
}
