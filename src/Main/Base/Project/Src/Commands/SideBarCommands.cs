// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SideBarRenameTabItem : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTabItem item = sideBar.ActiveTab.ChoosedItem;
			if (item != null) {
				sideBar.StartRenamingOf(item);
			}
		}
	}
	
	public class SideBarDeleteTabItem: AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTabItem item = sideBar.ActiveTab.ChoosedItem;
			
			
			
			if (item != null && MessageBox.Show(StringParser.Parse(ResourceService.GetString("SideBarComponent.ContextMenu.DeleteTabItemQuestion"), new string[,] { {"TabItem", item.Name}}),
			                    ResourceService.GetString("Global.QuestionText"), 
			                    MessageBoxButtons.YesNo, 
			                    MessageBoxIcon.Question,
			                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				sideBar.ActiveTab.Items.Remove(item);
				sideBar.Refresh();
			}
		}
	}
	
	public class SideBarAddTabHeader : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTab tab = new AxSideTab(sideBar, "New Tab");
			sideBar.Tabs.Add(tab);
			sideBar.StartRenamingOf(tab);
			sideBar.DoAddTab = true;
			sideBar.Refresh();
		} 
	}
	
	public class SideBarMoveTabUp : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.GetTabIndexAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (index > 0) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index - 1];
				sideBar.Tabs[index - 1] = tab;
				sideBar.Refresh();
			}
		} 
	}
	public class SideBarMoveTabDown : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.GetTabIndexAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (index >= 0 && index < sideBar.Tabs.Count - 1) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index + 1];
				sideBar.Tabs[index + 1] = tab;
				sideBar.Refresh();
			}
			
		} 
	}

	public class SideBarMoveActiveTabUp : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.Tabs.IndexOf(sideBar.ActiveTab);
			if (index > 0) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index - 1];
				sideBar.Tabs[index - 1] = tab;
				sideBar.Refresh();
			}
		} 
	}

	public class SideBarMoveActiveMoveTabDown : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.Tabs.IndexOf(sideBar.ActiveTab);
			if (index >= 0 && index < sideBar.Tabs.Count - 1) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index + 1];
				sideBar.Tabs[index + 1] = tab;
				sideBar.Refresh();
			}
		} 
	}
	
	public class SideBarDeleteTabHeader : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTab selectedSideTab = sideBar.GetTabAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			
			
			
			if (MessageBox.Show(StringParser.Parse(ResourceService.GetString("SideBarComponent.ContextMenu.DeleteTabHeaderQuestion"), new string[,] { {"TabHeader", selectedSideTab.Name}}),
			                    ResourceService.GetString("Global.QuestionText"), 
			                    MessageBoxButtons.YesNo, 
			                    MessageBoxIcon.Question,
			                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				sideBar.Tabs.Remove(selectedSideTab);
				sideBar.Refresh();
			}
		} 
	}
	
	public class SideBarRenameTabHeader : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			sideBar.StartRenamingOf(sideBar.GetTabAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y));
		} 
	}
	
	public class SideBarMoveActiveItemUp : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.SelectedItem);
			if (index > 0) {
				AxSideTabItem item = sideBar.ActiveTab.Items[index];
				sideBar.ActiveTab.Items[index] = sideBar.ActiveTab.Items[index - 1];
				sideBar.ActiveTab.Items[index - 1] = item;
				sideBar.Refresh();
			}
		}
	}
	
	public class SideBarMoveActiveItemDown : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.SelectedItem);
			if (index >= 0 && index < sideBar.ActiveTab.Items.Count - 1) {
				AxSideTabItem item = sideBar.ActiveTab.Items[index];
				sideBar.ActiveTab.Items[index] = sideBar.ActiveTab.Items[index + 1];
				sideBar.ActiveTab.Items[index + 1] = item;
				sideBar.Refresh();
			}
		} 
	}
}
