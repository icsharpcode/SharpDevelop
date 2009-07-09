// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
		/// <param name="addIn">The addIn definition that defines the command class.</param>
		/// <param name="commandName">The name of the command, e.g. "Copy".</param>
		/// <returns>The WPF ICommand with the given name, or null if thecommand was not found.</returns>
		public static System.Windows.Input.ICommand GetRegisteredCommand(AddIn addIn, string commandName)
		{
			if (addIn == null)
				throw new ArgumentNullException("addIn");
			if (commandName == null)
				throw new ArgumentNullException("commandName");
			System.Windows.Input.ICommand command;
			lock (knownCommands) {
				if (knownCommands.TryGetValue(commandName, out command))
					return command;
			}
			int pos = commandName.LastIndexOf('.');
			if (pos > 0) {
				string className = commandName.Substring(0, pos);
				string propertyName = commandName.Substring(pos + 1);
				Type classType = addIn.FindType(className);
				if (classType != null) {
					PropertyInfo p = classType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
					if (p != null)
						return (System.Windows.Input.ICommand)p.GetValue(null, null);
					FieldInfo f = classType.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);
					if (f != null)
						return (System.Windows.Input.ICommand)f.GetValue(null);
				}
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
				contextMenu.ItemsSource = ExpandMenuBuilders(subItems, true);
				args.Handled = true;
			};
			return contextMenu;
		}
		
		public static IList CreateMenuItems(UIElement inputBindingOwner, object owner, string addInTreePath)
		{
			IList items = CreateUnexpandedMenuItems(inputBindingOwner, AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
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
			
			public ICollection BuildItems()
			{
				return builder.BuildItems(codon, caller);
			}
		}
		
		internal static IList CreateUnexpandedMenuItems(UIElement inputBindingOwner, IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			if (descriptors != null) {
				foreach (MenuItemDescriptor descriptor in descriptors) {
					result.Add(CreateMenuItemFromDescriptor(inputBindingOwner, descriptor));
				}
			}
			return result;
		}
		
		static IList ExpandMenuBuilders(ICollection input, bool addDummyEntryIfMenuEmpty)
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
			if (addDummyEntryIfMenuEmpty && result.Count == 0) {
				result.Add(new MenuItem { Header = "(empty menu)", IsEnabled = false });
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
						ItemsSource = new object[1],
						SetEnabled = true
					};
					var subItems = CreateUnexpandedMenuItems(inputBindingOwner, descriptor.SubItems);
					item.SubmenuOpened += (sender, args) => {
						item.ItemsSource = ExpandMenuBuilders(subItems, true);
						args.Handled = true;
					};
					return item;
				case "Builder":
					IMenuItemBuilder builder = codon.AddIn.CreateObject(codon.Properties["class"]) as IMenuItemBuilder;
					if (builder == null)
						throw new NotSupportedException("Menu item builder " + codon.Properties["class"] + " does not implement IMenuItemBuilder");
						
					var placeHolder = new MenuItemBuilderPlaceholder(builder, descriptor.Codon, descriptor.Caller);
					placeHolder.BuildItems(); // Build items to register key gestures
					
					return placeHolder;
				default:
					throw new NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		/// <summary>
		/// Converts from the Windows-Forms style label format (accessor key marked with '&')
		/// to a WPF label format (accessor key marked with '_').
		/// </summary>
		public static string ConvertLabel(string label)
		{
			return label.Replace("_", "__").Replace("&", "_");
		}
		
		// HACK: find a better way to allow the host app to process link commands
		public static Converter<string, ICommand> LinkCommandCreator { get; set; }
	}
}
