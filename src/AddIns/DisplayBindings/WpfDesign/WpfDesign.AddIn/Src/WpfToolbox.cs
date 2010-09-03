// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
using WPF = System.Windows.Controls;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Manages the WpfToolbox.
	/// </summary>
	public class WpfToolbox
	{
		static WpfToolbox instance;
		
		public static WpfToolbox Instance {
			get {
				WorkbenchSingleton.AssertMainThread();
				if (instance == null) {
					instance = new WpfToolbox();
				}
				return instance;
			}
		}
		
		IToolService toolService;
		WpfSideBar sideBar;
		
		public WpfToolbox()
		{
			sideBar = new WpfSideBar();
			SideTab sideTab = new SideTab(sideBar, "Windows Presentation Foundation");
			sideTab.DisplayName = StringParser.Parse(sideTab.Name);
			sideTab.CanBeDeleted = false;
			sideTab.ChoosedItemChanged += OnChoosedItemChanged;
			
			sideTab.Items.Add(new WpfSideTabItem());
			foreach (Type t in Metadata.GetPopularControls())
				sideTab.Items.Add(new WpfSideTabItem(t));
			
			sideBar.Tabs.Add(sideTab);
			sideBar.ActiveTab = sideTab;
		}
		
		void OnChoosedItemChanged(object sender, EventArgs e)
		{
			if (toolService != null) {
				ITool newTool = null;
				if (sideBar.ActiveTab != null && sideBar.ActiveTab.ChoosedItem != null) {
					newTool = sideBar.ActiveTab.ChoosedItem.Tag as ITool;
				}
				toolService.CurrentTool = newTool ?? toolService.PointerTool;
			}
		}
		
		public Control ToolboxControl {
			get { return sideBar; }
		}
		
		public IToolService ToolService {
			get { return toolService; }
			set {
				if (toolService != null) {
					toolService.CurrentToolChanged -= OnCurrentToolChanged;
				}
				toolService = value;
				if (toolService != null) {
					toolService.CurrentToolChanged += OnCurrentToolChanged;
					OnCurrentToolChanged(null, null);
				}
			}
		}
		
		void OnCurrentToolChanged(object sender, EventArgs e)
		{
			object tagToFind;
			if (toolService.CurrentTool == toolService.PointerTool) {
				tagToFind = null;
			} else {
				tagToFind = toolService.CurrentTool;
			}
			if (sideBar.ActiveTab.ChoosedItem != null) {
				if (sideBar.ActiveTab.ChoosedItem.Tag == tagToFind)
					return;
			}
			foreach (SideTabItem item in sideBar.ActiveTab.Items) {
				if (item.Tag == tagToFind) {
					sideBar.ActiveTab.ChoosedItem = item;
					sideBar.Refresh();
					return;
				}
			}
			foreach (SideTab tab in sideBar.Tabs) {
				foreach (SideTabItem item in tab.Items) {
					if (item.Tag == tagToFind) {
						sideBar.ActiveTab = tab;
						sideBar.ActiveTab.ChoosedItem = item;
						sideBar.Refresh();
						return;
					}
				}
			}
			sideBar.ActiveTab.ChoosedItem = null;
			sideBar.Refresh();
		}
		
		sealed class WpfSideBar : SharpDevelopSideBar
		{
			protected override object StartItemDrag(SideTabItem draggedItem)
			{
				if (this.ActiveTab.ChoosedItem != draggedItem && this.ActiveTab.Items.Contains(draggedItem)) {
					this.ActiveTab.ChoosedItem = draggedItem;
				}
				return new System.Windows.DataObject(draggedItem.Tag);
			}
		}
	}
}
