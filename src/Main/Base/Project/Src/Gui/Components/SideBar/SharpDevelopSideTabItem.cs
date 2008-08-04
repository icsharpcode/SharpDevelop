// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
