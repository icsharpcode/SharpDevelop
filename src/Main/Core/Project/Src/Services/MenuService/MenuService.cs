using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.Core
{
	public static class MenuService 
	{
		static void ContextMenuPopupHandler(object sender, EventArgs e)
		{
			ContextMenuStrip  contextMenu = (ContextMenuStrip )sender;
			foreach (object o in contextMenu.Items) {
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
		}
		
		public static ContextMenuStrip CreateContextMenu(object owner, string addInTreePath)
		{
			if (addInTreePath == null) {
				return null;
			}
			try {
				ArrayList buildItems = AddInTree.GetTreeNode(addInTreePath).BuildChildItems(owner);
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				contextMenu.Opened += new EventHandler(ContextMenuPopupHandler);
				foreach (object item in buildItems) {
					if (item is ToolStripItem) {
						contextMenu.Items.Add((ToolStripItem)item);
					} else {
						ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
						contextMenu.Items.AddRange(submenuBuilder.BuildSubmenu(null, owner));
					}
				}
				ContextMenuPopupHandler(contextMenu, EventArgs.Empty);
				return contextMenu;
			} catch (TreePathNotFoundException) {
				Console.WriteLine("Warning tree path '" + addInTreePath +"' not found.");
				return null;
			} 
		}
		
		public static void ShowContextMenu(object owner, string addInTreePath, Control parent, int x, int y)
		{
			CreateContextMenu(owner, addInTreePath).Show(parent, new Point(x, y));
		}
		
		class QuickInsertMenuHandler
		{
			TextBoxBase targetControl;
			string      text;
			
			public QuickInsertMenuHandler(TextBoxBase targetControl, string text)
			{
				this.targetControl = targetControl;
				this.text          = text;
			}
			
			public EventHandler EventHandler {
				get {
					return new EventHandler(PopupMenuHandler);
				}
			}
			void PopupMenuHandler(object sender, EventArgs e)
			{
				targetControl.SelectedText += text;
			}
		}
		
		class QuickInsertHandler
		{
			Control               popupControl;
			ContextMenuStrip      quickInsertMenu;
			
			public QuickInsertHandler(Control popupControl, ContextMenuStrip quickInsertMenu)
			{
				this.popupControl    = popupControl;
				this.quickInsertMenu = quickInsertMenu;
				
				popupControl.Click += new EventHandler(showQuickInsertMenu);
			}
			
			void showQuickInsertMenu(object sender, EventArgs e)
			{
				Point cords = new Point(popupControl.Width, 0);
				quickInsertMenu.Show(popupControl, cords);
			}
		}
		
		public static void CreateQuickInsertMenu(TextBoxBase targetControl, Control popupControl, string[,] quickInsertMenuItems)
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			for (int i = 0; i < quickInsertMenuItems.GetLength(0); ++i) {
				if (quickInsertMenuItems[i, 0] == "-") {
					contextMenu.Items.Add(new MenuSeparator());
				} else {
					MenuCommand cmd = new MenuCommand(quickInsertMenuItems[i, 0],
					                                  new QuickInsertMenuHandler(targetControl, quickInsertMenuItems[i, 1]).EventHandler);
					contextMenu.Items.Add(cmd);
				}
			}
			new QuickInsertHandler(popupControl, contextMenu);
		}
	}
}
