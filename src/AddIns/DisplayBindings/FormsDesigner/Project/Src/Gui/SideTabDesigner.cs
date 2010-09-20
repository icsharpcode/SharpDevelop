// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing.Design;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class SideTabDesigner : SideTab
	{
		protected bool loadImages = true;
		IToolboxService toolboxService;
		
		protected SideTabDesigner(SideBarControl sideBar, string name, IToolboxService toolboxService)
			: base(sideBar, name)
		{
			this.DisplayName = StringParser.Parse(name);
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
		public SideTabDesigner(SideBarControl sideBar, Category category, IToolboxService toolboxService) : this(sideBar, category.Name, toolboxService)
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
			SideTabItem item = (sender as SideTab).ChoosedItem;
			if (item == null) {
				toolboxService.SetSelectedToolboxItem(null);
			} else {
				toolboxService.SetSelectedToolboxItem(item.Tag as ToolboxItem);
			}
		}
	}
}
