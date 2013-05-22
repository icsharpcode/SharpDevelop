// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SharpDevelopSideTabItem : SideTabItem
	{
		public SharpDevelopSideTabItem(string name)
			: base(name)
		{
			Icon = SD.ResourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag)
			: base(name, tag)
		{
			Icon = SD.ResourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag, Bitmap icon)
			: base(name, tag, icon)
		{
		}
	}
}
