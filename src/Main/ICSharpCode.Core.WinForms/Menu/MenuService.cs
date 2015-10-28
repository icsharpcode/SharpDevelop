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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public static class MenuService
	{
		public static Action<System.Windows.Input.ICommand, object> ExecuteCommand;
		public static Func<System.Windows.Input.ICommand, object, bool> CanExecuteCommand;
		
		public static void AddItemsToMenu(ToolStripItemCollection collection, object owner, string addInTreePath)
		{
			AddItemsToMenu(collection, AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
		}
		
		static void AddItemsToMenu(ToolStripItemCollection collection, IEnumerable<MenuItemDescriptor> descriptors)
		{
			foreach (MenuItemDescriptor descriptor in descriptors) {
				object item = CreateMenuItemFromDescriptor(descriptor);
				if (item is ToolStripItem) {
					collection.Add((ToolStripItem)item);
					if (item is IStatusUpdate)
						((IStatusUpdate)item).UpdateStatus();
				} else {
					IMenuItemBuilder submenuBuilder = (IMenuItemBuilder)item;
					collection.AddRange(submenuBuilder.BuildItems(descriptor.Codon, descriptor.Parameter).Cast<ToolStripItem>().ToArray());
				}
			}
		}
		
		static object CreateMenuItemFromDescriptor(MenuItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Command";
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new MenuSeparator(codon, descriptor.Parameter, descriptor.Conditions);
				case "CheckBox":
					return new MenuCheckBox(codon, descriptor.Parameter, descriptor.Conditions);
				case "Item":
				case "Command":
					return new MenuCommand(codon, descriptor.Parameter, descriptor.Conditions);
				case "Menu":
					return new Menu(codon, descriptor.Parameter, ConvertSubItems(descriptor.SubItems), descriptor.Conditions);
				case "Builder":
					return codon.AddIn.CreateObject(codon.Properties["class"]);
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		internal static ArrayList ConvertSubItems(IList items)
		{
			ArrayList r = new ArrayList();
			if (items != null) {
				foreach (MenuItemDescriptor descriptor in items) {
					r.Add(CreateMenuItemFromDescriptor(descriptor));
				}
			}
			return r;
		}
		
		public static ContextMenuStrip CreateContextMenu(object owner, string addInTreePath)
		{
			if (addInTreePath == null) {
				return null;
			}
			try {
				var descriptors = AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, true);
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				contextMenu.Items.Add(new ToolStripMenuItem("dummy"));
				contextMenu.Opening += delegate {
					contextMenu.Items.Clear();
					AddItemsToMenu(contextMenu.Items, descriptors);
				};
				contextMenu.Opened += ContextMenuOpened;
				contextMenu.Closed += ContextMenuClosed;
				return contextMenu;
			} catch (TreePathNotFoundException) {
				MessageService.ShowError("Warning tree path '" + addInTreePath +"' not found.");
				return null;
			}
		}
		
		static bool isContextMenuOpen;
		
		public static bool IsContextMenuOpen {
			get {
				return isContextMenuOpen;
			}
		}
		
		static void ContextMenuOpened(object sender, EventArgs e)
		{
			isContextMenuOpen = true;
			ContextMenuStrip contextMenu = (ContextMenuStrip)sender;
			foreach (object o in contextMenu.Items) {
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
		}
		
		static void ContextMenuClosed(object sender, EventArgs e)
		{
			isContextMenuOpen = false;
		}
		
		public static void ShowContextMenu(object owner, string addInTreePath, Control parent, int x, int y)
		{
			ContextMenuStrip menu = CreateContextMenu(owner, addInTreePath);
			if (menu != null) {
				menu.Show(parent, new Point(x, y));
			}
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
