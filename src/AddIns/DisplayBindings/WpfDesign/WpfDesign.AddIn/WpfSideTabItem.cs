// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 2573 $</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Description of WpfSideTabItem.
	/// </summary>
	public class WpfSideTabItem : SharpDevelopSideTabItem
	{
		public WpfSideTabItem(Type componentType) : base(componentType.Name, new CreateComponentTool(componentType))
		{
			CanBeRenamed = false;
//			this.Icon = tag.Bitmap;
		}
		
		///<summary>create a default tabitem : a pointer icon with an empty toolboxitem</summary>
		public WpfSideTabItem() : base("Pointer")
		{
			CanBeRenamed = false;
			CanBeDeleted = false;
			Bitmap pointerBitmap = new Bitmap(IconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
			this.Icon = pointerBitmap;
			this.Tag  = null;
		}
	}
}
