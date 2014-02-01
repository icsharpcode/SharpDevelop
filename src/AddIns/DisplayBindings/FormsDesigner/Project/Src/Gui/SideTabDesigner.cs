// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			this.ChosenItemChanged += SelectedTabItemChanged;
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
			SideTabItem item = (sender as SideTab).ChosenItem;
			if (item == null) {
				toolboxService.SetSelectedToolboxItem(null);
			} else {
				toolboxService.SetSelectedToolboxItem(item.Tag as ToolboxItem);
			}
		}
	}
}
