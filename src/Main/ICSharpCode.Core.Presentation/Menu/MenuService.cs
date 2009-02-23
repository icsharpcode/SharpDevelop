// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Creates WPF menu controls from the AddIn Tree.
	/// </summary>
	public static class MenuService
	{
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
		
		public static ContextMenu CreateContextMenu(object owner, string addInTreePath)
		{
			ContextMenu menu = new ContextMenu();
			menu.ItemsSource = CreateMenuItems(menu, owner, addInTreePath);
			return menu;
		}
		
		internal static ContextMenu CreateContextMenu(IList subItems)
		{
			var contextMenu = new ContextMenu() {
				ItemsSource = new object[1]
			};
			contextMenu.Opened += (sender, args) => {
				contextMenu.ItemsSource = ExpandMenuBuilders(subItems);
				args.Handled = true;
			};
			return contextMenu;
		}
		
		public static IList CreateMenuItems(UIElement inputBindingOwner, object owner, string addInTreePath)
		{
			return CreateMenuItems(inputBindingOwner, AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
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
			
			public ICollection BuildItems()
			{
				return builder.BuildItems(codon, caller);
			}
		}
		
		internal static IList CreateMenuItems(UIElement inputBindingOwner, IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			foreach (MenuItemDescriptor descriptor in descriptors) {
				result.Add(CreateMenuItemFromDescriptor(inputBindingOwner, descriptor));
			}
			return result;
		}
		
		internal static IList ExpandMenuBuilders(ICollection input)
		{
			ArrayList result = new ArrayList(input.Count);
			foreach (object o in input) {
				MenuItemBuilderPlaceholder p = o as MenuItemBuilderPlaceholder;
				if (p != null) {
					ICollection c = p.BuildItems();
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
			return result;
		}
		
		static object CreateMenuItemFromDescriptor(UIElement inputBindingOwner, MenuItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Command";
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new ConditionalSeparator(codon, descriptor.Caller, false);
				case "CheckBox":
					return "CheckBox";
					//return new MenuCheckBox(codon, descriptor.Caller);
				case "Item":
				case "Command":
					return new MenuCommand(inputBindingOwner, codon, descriptor.Caller, createCommand);
				case "Menu":
					var item = new CoreMenuItem(codon, descriptor.Caller) {
						ItemsSource = new object[1]
					};
					var subItems = CreateMenuItems(inputBindingOwner, descriptor.SubItems);
					item.SubmenuOpened += (sender, args) => {
						item.ItemsSource = ExpandMenuBuilders(subItems);
						args.Handled = true;
					};
					return item;
				case "Builder":
					IMenuItemBuilder builder = codon.AddIn.CreateObject(codon.Properties["class"]) as IMenuItemBuilder;
					if (builder == null)
						throw new NotSupportedException("Menu item builder " + codon.Properties["class"] + " does not implement IMenuItemBuilder");
					return new MenuItemBuilderPlaceholder(builder, descriptor.Codon, descriptor.Caller);
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		public static string ConvertLabel(string label)
		{
			return label.Replace("_", "__").Replace("&", "_");
		}
		
		// HACK: find a better way to allow the host app to process link commands
		public static Converter<string, ICommand> LinkCommandCreator;
		
		/// <summary>
		/// Creates an KeyGesture for a shortcut.
		/// </summary>
		public static KeyGesture ParseShortcut(string text)
		{
			return (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString(text.Replace(',', '+').Replace('|', '+'));
		}
	}
}
