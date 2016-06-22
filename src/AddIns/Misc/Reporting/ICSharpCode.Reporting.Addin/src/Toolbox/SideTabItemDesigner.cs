/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.03.2014
 * Time: 12:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Design;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reporting.Addin.Toolbox
{
	/// <summary>
	/// Description of SideTabItemDesigner.
	/// </summary>
		public class SideTabItemDesigner : SharpDevelopSideTabItem
	{
		///<summary>create a tabitem from a toolboxitem. It init Icon and name from the tag</summary>
		public SideTabItemDesigner(ToolboxItem tag) : base(tag.DisplayName, tag)
		{
			if (tag == null) {
				throw new ArgumentNullException("tag");
			}
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
			var pointerBitmap = new Bitmap(IconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
			this.Icon = pointerBitmap;
			this.Tag  = null;
			ReloadToolBox();
		}
		
		///<summary>it force to reload toolboxitem into the ToolboxService when the hostchange</summary>
		public void ReloadToolBox()
		{
			if (this.Name != "Pointer") {
//				ToolboxProvider.ToolboxService.AddToolboxItem(this.Tag as ToolboxItem);
			}
		}
	}
}
