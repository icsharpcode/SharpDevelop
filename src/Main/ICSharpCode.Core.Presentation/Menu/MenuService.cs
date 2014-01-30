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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using WinForms = System.Windows.Forms;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Creates WPF menu controls from the AddIn Tree.
	/// </summary>
	public static class MenuService
	{
		internal sealed class MenuCreateContext
		{
			public UIElement InputBindingOwner;
			public string ActivationMethod;
			public bool ImmediatelyExpandMenuBuildersForShortcuts;
		}
		
		static Dictionary<string, System.Windows.Input.ICommand> knownCommands = LoadDefaultKnownCommands();
		
		static Dictionary<string, System.Windows.Input.ICommand> LoadDefaultKnownCommands()
		{
			var knownCommands = new Dictionary<string, System.Windows.Input.ICommand>();
			foreach (Type t in new Type[] { typeof(ApplicationCommands), typeof(NavigationCommands) }) {
				foreach (PropertyInfo p in t.GetProperties()) {
					knownCommands.Add(p.Name, (System.Windows.Input.ICommand)p.GetValue(null, null));
				}
			}
			return knownCommands;
		}
		
		/// <summary>
		/// Gets a known WPF command.
		/// </summary>
		/// <param name="commandName">The name of the command, e.g. "Copy".</param>
		/// <returns>The WPF ICommand with the given name, or null if the command was not found.</returns>
		public static System.Windows.Input.ICommand GetKnownCommand(string commandName)
		{
			if (commandName == null)
				throw new ArgumentNullException("commandName");
			System.Windows.Input.ICommand command;
			lock (knownCommands) {
				if (knownCommands.TryGetValue(commandName, out command))
					return command;
			}
			return null;
		}
		
		/// <summary>
		/// Registers a WPF command for use with the &lt;MenuItem command="name"&gt; syntax.
		/// </summary>
		public static void RegisterKnownCommand(string name, System.Windows.Input.ICommand command)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (command == null)
				throw new ArgumentNullException("command");
			lock (knownCommands) {
				knownCommands.Add(name, command);
			}
		}
		
		public static void UpdateStatus(IEnumerable menuItems)
		{
			if (menuItems == null)
				return;
			foreach (object o in menuItems) {
				IStatusUpdate cmi = o as IStatusUpdate;
				if (cmi != null)
					cmi.UpdateStatus();
			}
		}
		
		public static void UpdateText(IEnumerable menuItems)
		{
			if (menuItems == null)
				return;
			foreach (object o in menuItems) {
				IStatusUpdate cmi = o as IStatusUpdate;
				if (cmi != null)
					cmi.UpdateText();
			}
		}
		
		public static ContextMenu CreateContextMenu(object owner, string addInTreePath)
		{
			IList items = CreateUnexpandedMenuItems(
				new MenuCreateContext { ActivationMethod = "ContextMenu" },
				AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
			return CreateContextMenu(items);
		}
		
		public static ContextMenu ShowContextMenu(UIElement parent, object owner, string addInTreePath)
		{
			ContextMenu menu = new ContextMenu();
			menu.ItemsSource = CreateMenuItems(menu, owner, addInTreePath, "ContextMenu");
			menu.PlacementTarget = parent;
			menu.IsOpen = true;
			return menu;
		}
		
		internal static ContextMenu CreateContextMenu(IList subItems)
		{
			var contextMenu = new ContextMenu() {
				ItemsSource = new object[1]
			};
			contextMenu.Opened += (sender, args) => {
				contextMenu.ItemsSource = ExpandMenuBuilders(subItems, true);
				args.Handled = true;
			};
			return contextMenu;
		}
		
		public static IList CreateMenuItems(UIElement inputBindingOwner, object owner, string addInTreePath, string activationMethod = null, bool immediatelyExpandMenuBuildersForShortcuts = false)
		{
			IList items = CreateUnexpandedMenuItems(
				new MenuCreateContext {
					InputBindingOwner = inputBindingOwner,
					ActivationMethod = activationMethod,
					ImmediatelyExpandMenuBuildersForShortcuts =immediatelyExpandMenuBuildersForShortcuts
				},
				AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
			return ExpandMenuBuilders(items, false);
		}
		
		sealed class MenuItemBuilderPlaceholder
		{
			readonly IMenuItemBuilder builder;
			readonly Codon codon;
			readonly object caller;
			
			public MenuItemBuilderPlaceholder(IMenuItemBuilder builder, Codon codon, object caller)
			{
				this.builder = builder;
				this.codon = codon;
				this.caller = caller;
			}
			
			public IEnumerable<object> BuildItems()
			{
				return builder.BuildItems(codon, caller);
			}
		}
		
		internal static IList CreateUnexpandedMenuItems(MenuCreateContext context, IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			if (descriptors != null) {
				foreach (MenuItemDescriptor descriptor in descriptors) {
					result.Add(CreateMenuItemFromDescriptor(context, descriptor));
				}
			}
			return result;
		}
		
		static IList ExpandMenuBuilders(ICollection input, bool addDummyEntryIfMenuEmpty)
		{
			List<object> result = new List<object>(input.Count);
			foreach (object o in input) {
				MenuItemBuilderPlaceholder p = o as MenuItemBuilderPlaceholder;
				if (p != null) {
					IEnumerable<object> c = p.BuildItems();
					if (c != null)
						result.AddRange(c);
				} else {
					result.Add(o);
					IStatusUpdate statusUpdate = o as IStatusUpdate;
					if (statusUpdate != null) {
						statusUpdate.UpdateStatus();
						statusUpdate.UpdateText();
					}
				}
			}
			if (addDummyEntryIfMenuEmpty && result.Count == 0) {
				result.Add(new MenuItem { Header = "(empty menu)", IsEnabled = false });
			}
			return result;
		}
		
		static object CreateMenuItemFromDescriptor(MenuCreateContext context, MenuItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Command";
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new ConditionalSeparator(codon, descriptor.Parameter, false, descriptor.Conditions);
				case "CheckBox":
					return new MenuCheckBox(context.InputBindingOwner, codon, descriptor.Parameter, descriptor.Conditions);
				case "Item":
				case "Command":
					return new MenuCommand(context.InputBindingOwner, codon, descriptor.Parameter, createCommand, context.ActivationMethod, descriptor.Conditions);
				case "Menu":
					var item = new CoreMenuItem(codon, descriptor.Parameter, descriptor.Conditions) {
						ItemsSource = new object[1],
						SetEnabled = true
					};
					var subItems = CreateUnexpandedMenuItems(context, descriptor.SubItems);
					item.SubmenuOpened += (sender, args) => {
						item.ItemsSource = ExpandMenuBuilders(subItems, true);
						args.Handled = true;
					};
					if (context.ImmediatelyExpandMenuBuildersForShortcuts)
						ExpandMenuBuilders(subItems, false);
					return item;
				case "Builder":
					IMenuItemBuilder builder = codon.AddIn.CreateObject(codon.Properties["class"]) as IMenuItemBuilder;
					if (builder == null)
						throw new NotSupportedException("Menu item builder " + codon.Properties["class"] + " does not implement IMenuItemBuilder");
					return new MenuItemBuilderPlaceholder(builder, descriptor.Codon, descriptor.Parameter);
				default:
					throw new NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		/// <summary>
		/// Converts from the Windows-Forms style label format (accessor key marked with '&amp;')
		/// to a WPF label format (accessor key marked with '_').
		/// </summary>
		public static string ConvertLabel(string label)
		{
			return label.Replace("_", "__").Replace("&", "_");
		}
		
		/// <summary>
		/// Creates an KeyGesture for a shortcut.
		/// </summary>
		public static KeyGesture ParseShortcut(string text)
		{
			return (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString(text.Replace('|', '+'));
		}
		
		public static string GetDisplayStringForShortcut(KeyGesture kg)
		{
			string old = kg.GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
			string text = KeyCodeConversion.KeyToUnicode(kg.Key.ToKeys());
			if (text != null && !text.Any(ch => char.IsWhiteSpace(ch))) {
				if ((kg.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
					text = "Alt+" + text;
				if ((kg.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
					text = "Shift+" + text;
				if ((kg.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
					text = "Ctrl+" + text;
				if ((kg.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
					text = "Win+" + text;
				return text;
			}
			return old;
		}
	}
	
	static class KeyCodeConversion
	{
		[DllImport("user32.dll")]
		static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte []
		                              lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
		                              int cchBuff, uint wFlags, IntPtr dwhkl);

		[DllImport("user32.dll")]
		static extern bool GetKeyboardState(byte[] pbKeyState);
		
		[DllImport("user32.dll")]
		static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

		[DllImport("user32.dll")]
		static extern IntPtr GetKeyboardLayout(uint idThread);

		/// <remarks>Only works with Windows.Forms.Keys. The WPF Key enum seems to be horribly distorted!</remarks>
		public static string KeyToUnicode(WinForms.Keys key)
		{
			StringBuilder sb = new StringBuilder(256);
			IntPtr hkl = GetKeyboardLayout(0);
			
			uint scanCode = MapVirtualKeyEx((uint)key, 0, hkl);
			if (scanCode < 1) return null;
			
			ClearKeyboardBuffer(hkl);
			int len = ToUnicodeEx((uint)key, scanCode, new byte[256], sb, sb.Capacity, 0, hkl);
			if (len > 0)
				return sb.ToString(0, len).ToUpper();
			
			ClearKeyboardBuffer(hkl);
			return null;
		}
		
		static void ClearKeyboardBuffer(IntPtr hkl)
		{
			StringBuilder sb = new StringBuilder(10);
			uint key = (uint)WinForms.Keys.Space;
			int rc;
			do {
				rc = ToUnicodeEx(key, MapVirtualKeyEx(key, 0, hkl), new byte[256], sb, sb.Capacity, 0, hkl);
			} while(rc < 0);
		}
		
		public static WinForms.Keys ToKeys(this Key key)
		{
			WinForms.Keys result;
			if (Enum.TryParse(key.ToString(), out result))
				return result;
			return WinForms.Keys.None;
		}
	}
}
