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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
using ICSharpCode.SharpDevelop.Workbench;
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
				SD.MainThread.VerifyAccess();
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
		
		static bool IsControl(Type t)
		{
			return !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsSubclassOf(typeof(FrameworkElement));
		}

		static HashSet<string> addedAssemblys = new HashSet<string>();
		public void AddProjectDlls(OpenedFile file)
		{
			var compilation = SD.ParserService.GetCompilationForFile(file.FileName);
			foreach (var reference in compilation.ReferencedAssemblies) {
				string f = reference.GetReferenceAssemblyLocation();
				
				if (f != null && !addedAssemblys.Contains(f)) {
					try {
						// DO NOT USE Assembly.LoadFrom!!!
						// see http://community.sharpdevelop.net/forums/t/19968.aspx
						// TODO : reimplement this!
//						var assembly = Assembly.LoadFrom(f);
//
//						SideTab sideTab = new SideTab(sideBar, assembly.FullName.Split(new[] {','})[0]);
//						sideTab.DisplayName = StringParser.Parse(sideTab.Name);
//						sideTab.CanBeDeleted = false;
//						sideTab.ChoosedItemChanged += OnChoosedItemChanged;
//
//						sideTab.Items.Add(new WpfSideTabItem());
//
//						foreach (var t in assembly.GetExportedTypes())
//						{
//							if (IsControl(t))
//							{
//								sideTab.Items.Add(new WpfSideTabItem(t));
//							}
//						}
//
//						if (sideTab.Items.Count > 1)
//							sideBar.Tabs.Add(sideTab);
//
//						addedAssemblys.Add(f);
					} catch (Exception ex) {
						WpfViewContent.DllLoadErrors.Add(new SDTask(new BuildError(f, ex.Message)));
					}
				}
			}
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
				
				if (draggedItem.Tag != null) {
					return new System.Windows.DataObject(draggedItem.Tag);
				}
				else {
					return new System.Windows.DataObject();
				}
			}
		}
	}
}
