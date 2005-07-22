// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SharpDevelopSideTabItem : AxSideTabItem
	{
		
		
		public SharpDevelopSideTabItem(string name) : base(name)
		{
			Icon = ResourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag) : base(name, tag)
		{
			Icon = ResourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag, Bitmap icon) : base(name, tag, icon)
		{
		}
	}
}
