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
		
		private SharpDevelopSideBar _sideBar;
		internal SharpDevelopSideBar SideBar { 
			get { return _sideBar; }
			set
			{
				_sideBar = value;
				var sideTab = new SideTab(_sideBar, "Windows Presentation Foundation");
				sideTab.DisplayName = StringParser.Parse(sideTab.Name);
				sideTab.CanBeDeleted = false;
				sideTab.ChoosedItemChanged += OnChoosedItemChanged;

				sideTab.Items.Add(new WpfSideTabItem());
				foreach (Type t in Metadata.GetPopularControls())
					sideTab.Items.Add(new WpfSideTabItem(t));
				_sideBar.Tabs.Add(sideTab);
				_sideBar.ActiveTab = sideTab;
			}
		}
		
		void OnChoosedItemChanged(object sender, EventArgs e)
		{
			if (toolService != null) {
				ITool newTool = null;
				if (_sideBar.ActiveTab != null && _sideBar.ActiveTab.ChoosedItem != null)
				{
					newTool = _sideBar.ActiveTab.ChoosedItem.Tag as ITool;
				}
				toolService.CurrentTool = newTool ?? toolService.PointerTool;
			}
		}
		
		public Control ToolboxControl {
			get { return _sideBar; }
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
			
			if (_sideBar == null)
				return;
			
			if (_sideBar.ActiveTab.ChoosedItem != null)
			{
				if (_sideBar.ActiveTab.ChoosedItem.Tag == tagToFind)
					return;
			}
			foreach (SideTabItem item in _sideBar.ActiveTab.Items)
			{
				if (item.Tag == tagToFind) {
					_sideBar.ActiveTab.ChoosedItem = item;
					_sideBar.Refresh();
					return;
				}
			}
			foreach (SideTab tab in _sideBar.Tabs)
			{
				foreach (SideTabItem item in tab.Items) {
					if (item.Tag == tagToFind) {
						_sideBar.ActiveTab = tab;
						_sideBar.ActiveTab.ChoosedItem = item;
						_sideBar.Refresh();
						return;
					}
				}
			}
			_sideBar.ActiveTab.ChoosedItem = null;
			_sideBar.Refresh();
		}
	}
	
	public sealed class WpfSideBar : SharpDevelopSideBar
	{
		private static WpfSideBar _instance;
		public static WpfSideBar Instance
		{
			get
			{
				return _instance ?? (_instance = new WpfSideBar());
			}
		}
		
		private WpfSideBar()
		{
			WpfToolbox.Instance.SideBar = this;
		}
		
		protected override object StartItemDrag(SideTabItem draggedItem)
		{
			if (ActiveTab.ChoosedItem != draggedItem && ActiveTab.Items.Contains(draggedItem))
			{
				ActiveTab.ChoosedItem = draggedItem;
			}
			return new System.Windows.DataObject(draggedItem.Tag);
		}
	}
}
