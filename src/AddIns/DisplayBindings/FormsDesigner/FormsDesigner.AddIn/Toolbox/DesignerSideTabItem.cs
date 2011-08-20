// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Design;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class DesignerSideTabItem : SharpDevelopSideTabItem
	{
		ToolboxProvider provider;
		
		///<summary>create a tabitem from a toolboxitem. It init Icon and name from the tag</summary>
		public DesignerSideTabItem(ToolboxProvider provider, ToolboxItem tag) : base(tag.DisplayName, tag)
		{
			this.provider = provider;
			CanBeRenamed = false;
			this.Icon = tag.Bitmap;
			ReloadToolBox();
		}
		
		///<summary>create a tabitem from a toolboxitem. It init Icon from the tag</summary>
		public DesignerSideTabItem(ToolboxProvider provider, string name, ToolboxItem tag) : base(name, tag)
		{
			this.provider = provider;
			CanBeRenamed = false;
			this.Icon = tag.Bitmap;
			ReloadToolBox();
		}
		
		///<summary>create a default tabitem : a pointer icon with an empty toolboxitem</summary>
		public DesignerSideTabItem() : base("Pointer")
		{
			CanBeRenamed = false;
			CanBeDeleted = false;
			Bitmap pointerBitmap = new Bitmap(IconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
//			ToolboxItem toolboxItemPointer = new ToolboxItem();
//			toolboxItemPointer.Bitmap      = pointerBitmap;
//			toolboxItemPointer.DisplayName = "Pointer";
			this.Icon = pointerBitmap;
			this.Tag  = null;
			ReloadToolBox();
		}
		
		///<summary>it force to reload toolboxitem into the ToolboxService when the hostchange</summary>
		public void ReloadToolBox()
		{
			if (this.Name != "Pointer") {
				provider.ToolboxService.AddToolboxItem(this.Tag as ToolboxItem);
			}
		}
	}
}
