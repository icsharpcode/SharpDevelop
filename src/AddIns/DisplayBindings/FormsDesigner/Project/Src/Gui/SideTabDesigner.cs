// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Denis ERCHOFF" email="d_erchoff@hotmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class SideTabDesigner : AxSideTab
	{
		protected bool loadImages = true;
		IToolboxService toolboxService;
		
		protected SideTabDesigner(AxSideBar sideBar, string name, IToolboxService toolboxService) : base(sideBar, name)
		{
			this.toolboxService = toolboxService;
			this.CanSaved = false;
			
			AddDefaultItem();
			this.ChoosedItemChanged += SelectedTabItemChanged;
		}
		
		protected void AddDefaultItem()
		{
			this.Items.Add(new SideTabItemDesigner());
		}
		
		///<summary>Load an assembly's controls</summary>
		public SideTabDesigner(AxSideBar sideBar, Category category, IToolboxService toolboxService) : this(sideBar, category.Name, toolboxService)
		{
			foreach (ToolComponent component in category.ToolComponents) {
				if (component.IsEnabled) {
					ToolboxItem toolboxItem = new ToolboxItem();
					toolboxItem.TypeName    = component.FullName;
					toolboxItem.Bitmap      = ToolboxProvider.ComponentLibraryLoader.GetIcon(component);
					toolboxItem.DisplayName = component.Name;
					Assembly asm = component.LoadAssembly();
					toolboxItem.AssemblyName = asm.GetName();
					
					this.Items.Add(new SideTabItemDesigner(toolboxItem));
				}
			}
		}
		
		void SelectedTabItemChanged(object sender, EventArgs e)
		{
			AxSideTabItem item = (sender as AxSideTab).ChoosedItem;
			if (item == null) {
				toolboxService.SetSelectedToolboxItem(null);
			} else {
				toolboxService.SetSelectedToolboxItem(item.Tag as ToolboxItem);
			}
		}
	}
}
