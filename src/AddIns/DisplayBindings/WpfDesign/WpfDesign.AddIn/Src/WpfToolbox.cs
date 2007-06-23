// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
				Debug.Assert(WorkbenchSingleton.InvokeRequired == false);
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
			sideTab.Items.Add(new WpfSideTabItem(typeof(WPF.Button)));
			sideTab.Items.Add(new WpfSideTabItem(typeof(WPF.CheckBox)));
			sideTab.Items.Add(new WpfSideTabItem(typeof(WPF.TextBox)));
			sideTab.Items.Add(new WpfSideTabItem(typeof(WPF.Grid)));
			sideTab.Items.Add(new WpfSideTabItem(typeof(WPF.Canvas)));
			
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
			Debug.WriteLine("WpfToolbox.OnCurrentToolChanged");
//			for (int i = 0; i < this.Items.Count; i++) {
//				if (((ListBoxItem)this.Items[i]).Tag == toolService.CurrentTool) {
//					this.SelectedIndex = i;
//					return;
//				}
//			}
//			this.SelectedIndex = -1;
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
