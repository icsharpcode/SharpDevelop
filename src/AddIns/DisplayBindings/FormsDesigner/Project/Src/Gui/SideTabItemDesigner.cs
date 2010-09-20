// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Module : FormDesigner
 * 
 * Project : FormDesigner Loading Library Control.
 * 
 * Source code altering : A1
 * 
 * Description : création of the Tab item displayed into the AXSideTabDesigner control.
 * 				 the control's creator initialize the toolboxitem of the tab item
 * 
 * Denis ERCHOFF						22/01/2003
 */


//		Denis ERCHOFF		22/01/2003		BEGIN		A1

using System;
using System.Drawing;
using System.Drawing.Design;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class SideTabItemDesigner : SharpDevelopSideTabItem
	{
		///<summary>create a tabitem from a toolboxitem. It init Icon and name from the tag</summary>
		public SideTabItemDesigner(ToolboxItem tag) : base(tag.DisplayName, tag)
		{
			CanBeRenamed = false;
			this.Icon = tag.Bitmap;
			ReloadToolBox();
		}
		
		///<summary>create a tabitem from a toolboxitem. It init Icon from the tag</summary>
		public SideTabItemDesigner(string name, ToolboxItem tag) : base(name, tag)
		{
			CanBeRenamed = false;
			this.Icon = tag.Bitmap;
			ReloadToolBox();
		}
		
		///<summary>create a default tabitem : a pointer icon with an empty toolboxitem</summary>
		public SideTabItemDesigner() : base("Pointer")
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
				ToolboxProvider.ToolboxService.AddToolboxItem(this.Tag as ToolboxItem);
			}
		}
	}
}
