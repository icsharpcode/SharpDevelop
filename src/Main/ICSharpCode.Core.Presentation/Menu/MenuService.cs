// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Creates WPF menu controls from the AddIn Tree.
	/// </summary>
	public static class MenuService
	{
		public static IList CreateMenuItems(object owner, string addInTreePath)
		{
			return CreateMenuItems(AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
		}
		
		static IList CreateMenuItems(IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			foreach (MenuItemDescriptor descriptor in descriptors) {
				object item = CreateMenuItemFromDescriptor(descriptor);
				if (item is IMenuItemBuilder) {
					IMenuItemBuilder submenuBuilder = (IMenuItemBuilder)item;
					result.AddRange(submenuBuilder.BuildItems(descriptor.Codon, descriptor.Caller));
				} else {
					result.Add(item);
				}
			}
			return result;
		}
		
		static object CreateMenuItemFromDescriptor(MenuItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Command";
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new Separator();
					//return new MenuSeparator(codon, descriptor.Caller);
				case "CheckBox":
					return "CheckBox";
					//return new MenuCheckBox(codon, descriptor.Caller);
				case "Item":
				case "Command":
					return new MenuCommand(codon, descriptor.Caller, createCommand);
				case "Menu":
					return new CoreMenuItem(codon, descriptor.Caller) {
						ItemsSource = CreateMenuItems(descriptor.SubItems)
					};
				case "Builder":
					return codon.AddIn.CreateObject(codon.Properties["class"]);
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
	}
}
