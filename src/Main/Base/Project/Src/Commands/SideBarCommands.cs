// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SideBarRenameTabItem : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			SideTabItem item = sideBar.ActiveTab.ChoosedItem;
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
			SideTabItem item = sideBar.ActiveTab.ChoosedItem;
			if (item != null) {
				if (MessageBox.Show(StringParser.Parse(
					"${res:SideBarComponent.ContextMenu.DeleteTabItemQuestion}", new StringTagPair("TabItem", item.Name)),
				                    ResourceService.GetString("Global.QuestionText"),
				                    MessageBoxButtons.YesNo,
				                    MessageBoxIcon.Question,
				                    MessageBoxDefaultButton.Button2)
				    == DialogResult.Yes)
				{
					sideBar.ActiveTab.Items.Remove(item);
					sideBar.Refresh();
				}
			}
		}
	}
	
	public class SideBarAddTabHeader : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			SideTab tab = new SideTab(sideBar, "New Tab");
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
				SideTab tab = sideBar.Tabs[index];
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
				SideTab tab = sideBar.Tabs[index];
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
				SideTab tab = sideBar.Tabs[index];
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
				SideTab tab = sideBar.Tabs[index];
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
			SideTab selectedSideTab = sideBar.GetTabAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (MessageBox.Show(StringParser.Parse(ResourceService.GetString("SideBarComponent.ContextMenu.DeleteTabHeaderQuestion"), new StringTagPair("TabHeader", selectedSideTab.DisplayName)),
			                    ResourceService.GetString("Global.QuestionText"),
			                    MessageBoxButtons.YesNo,
			                    MessageBoxIcon.Question,
			                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				sideBar.DeleteSideTab(selectedSideTab);
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
			int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.ChoosedItem);
			if (index > 0) {
				sideBar.ActiveTab.Exchange(index -1, index);
				sideBar.Refresh();
			}
		}
	}
	
	public class SideBarMoveActiveItemDown : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.ChoosedItem);
			if (index >= 0 && index < sideBar.ActiveTab.Items.Count - 1) {
				sideBar.ActiveTab.Exchange(index, index + 1);
				sideBar.Refresh();
			}
		}
	}
}
