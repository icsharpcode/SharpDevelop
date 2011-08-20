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
		
		ToolboxProvider toolbox;
		
		public ToolboxProvider Toolbox {
			get { return toolbox; }
		}
		
		///<summary>Load an assembly's controls</summary>
		public SideTabDesigner(ToolboxProvider toolbox, SideBarControl sideBar, Category category, IToolboxService toolboxService)
			: this(sideBar, category.Name, toolboxService)
		{
			this.toolbox = toolbox;
			foreach (ToolComponent component in category.ToolComponents) {
				if (component.IsEnabled) {
					ToolboxItem toolboxItem = new ToolboxItem();
					toolboxItem.TypeName    = component.FullName;
					toolboxItem.Bitmap      = toolbox.ComponentLibraryLoader.GetIcon(component);
					toolboxItem.DisplayName = component.Name;
					toolboxItem.AssemblyName = toolbox.ComponentLibraryLoader.GetAssemblyName(component);
					
					this.Items.Add(new SideTabItemDesigner(toolbox, toolboxItem));
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
