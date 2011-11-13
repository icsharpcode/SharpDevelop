// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Creates WPF toolbars from the AddIn Tree.
	/// </summary>
	public static class ToolBarService
	{
		/// <summary>
		/// Style key used for toolbar images.
		/// </summary>
		public static readonly ResourceKey ImageStyleKey = new ComponentResourceKey(typeof(ToolBarService), "ImageStyle");
		
		public static void UpdateStatus(IEnumerable toolBarItems)
		{
			MenuService.UpdateStatus(toolBarItems);
		}
		
		public static IList CreateToolBarItems(UIElement inputBindingOwner, object owner, string addInTreePath)
		{
			return CreateToolBarItems(inputBindingOwner, AddInTree.BuildItems<ToolbarItemDescriptor>(addInTreePath, owner, false));
		}
		
		static IList CreateToolBarItems(UIElement inputBindingOwner, IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			foreach (ToolbarItemDescriptor descriptor in descriptors) {
				object item = CreateToolBarItemFromDescriptor(inputBindingOwner, descriptor);
				IMenuItemBuilder submenuBuilder = item as IMenuItemBuilder;
				if (submenuBuilder != null) {
					result.AddRange(submenuBuilder.BuildItems(descriptor.Codon, descriptor.Caller));
				} else {
					result.Add(item);
				}
			}
			return result;
		}
		
		static object CreateToolBarItemFromDescriptor(UIElement inputBindingOwner, ToolbarItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			object caller = descriptor.Caller;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";
			
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new ConditionalSeparator(codon, caller, true, descriptor.Conditions);
				case "CheckBox":
					return new ToolBarCheckBox(codon, caller, descriptor.Conditions);
				case "Item":
					return new ToolBarButton(inputBindingOwner, codon, caller, createCommand, descriptor.Conditions);
				case "ComboBox":
					return new ToolBarComboBox(codon, caller, descriptor.Conditions);
				case "TextBox":
					return "TextBox";
					//return new ToolBarTextBox(codon, caller);
				case "Label":
					return "Label";
					//return new ToolBarLabel(codon, caller);
				case "DropDownButton":
					return new ToolBarDropDownButton(
						codon, caller, MenuService.CreateUnexpandedMenuItems(
							new MenuService.MenuCreateContext { ActivationMethod = "ToolbarDropDownMenu" },
							descriptor.SubItems), descriptor.Conditions);
				case "SplitButton":
					return new ToolBarSplitButton(
						codon, caller, MenuService.CreateUnexpandedMenuItems(
							new MenuService.MenuCreateContext { ActivationMethod = "ToolbarDropDownMenu" },
							descriptor.SubItems), descriptor.Conditions);
				case "Builder":
					object result = codon.AddIn.CreateObject(codon.Properties["class"]);
					if (result is IToolBarItemBuilder)
						((IToolBarItemBuilder)result).Initialize(inputBindingOwner, codon, caller);
					return result;
				default:
					throw new System.NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		static ToolBar CreateToolBar(UIElement inputBindingOwner, object owner, AddInTreeNode treeNode)
		{
			ToolBar tb = new CoreToolBar();
			ToolBarTray.SetIsLocked(tb, true);
			tb.ItemsSource = CreateToolBarItems(inputBindingOwner, treeNode.BuildChildItems<ToolbarItemDescriptor>(owner));
			UpdateStatus(tb.ItemsSource); // setting Visible is only possible after the items have been added
			return tb;
		}
		
		sealed class CoreToolBar : ToolBar, IWeakEventListener
		{
			public CoreToolBar()
			{
				LanguageChangeWeakEventManager.AddListener(this);
			}
			
			bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
			{
				if (managerType == typeof(LanguageChangeWeakEventManager)) {
					MenuService.UpdateText(this.ItemsSource);
					return true;
				}
				return false;
			}
		}
		
		public static ToolBar CreateToolBar(UIElement inputBindingOwner, object owner, string addInTreePath)
		{
			return CreateToolBar(inputBindingOwner, owner, AddInTree.GetTreeNode(addInTreePath));
		}
		
		public static ToolBar[] CreateToolBars(UIElement inputBindingOwner, object owner, string addInTreePath)
		{
			AddInTreeNode treeNode;
			try {
				treeNode = AddInTree.GetTreeNode(addInTreePath);
			} catch (TreePathNotFoundException) {
				return null;
			}
			List<ToolBar> toolBars = new List<ToolBar>();
			foreach (AddInTreeNode childNode in treeNode.ChildNodes.Values) {
				toolBars.Add(CreateToolBar(inputBindingOwner, owner, childNode));
			}
			return toolBars.ToArray();
		}

		internal static object CreateToolBarItemContent(Codon codon)
		{
			object result = null;
			Image image = null;
			Label label = null;
			bool isImage = false;
			bool isLabel = false;
			if (codon.Properties.Contains("icon"))
			{
				image = PresentationResourceService.GetImage(StringParser.Parse(codon.Properties["icon"]));
				image.Height = 16;
				image.SetResourceReference(FrameworkElement.StyleProperty, ToolBarService.ImageStyleKey);
				isImage = true;
			}
			if (codon.Properties.Contains("label"))
			{
				label = new Label();
				label.Content = StringParser.Parse(codon.Properties["label"]);
				label.Padding = new Thickness(0);
				label.VerticalContentAlignment = VerticalAlignment.Center;
				isLabel = true;
			}

			if (isImage && isLabel)
			{
				StackPanel panel = new StackPanel();
				panel.Orientation = Orientation.Horizontal;
				image.Margin = new Thickness(0, 0, 5, 0);
				panel.Children.Add(image);
				panel.Children.Add(label);
				result = panel;
			}
			else
				if (isImage)
				{
					result = image;
				}
				else
					if (isLabel)
					{
						result = label;
					}
					else
					{
						result = codon.Id;
					}

			return result;
		}
	}

	public interface IToolBarItemBuilder
	{
		void Initialize(UIElement inputBindingOwner, Codon codon, object owner);
	}
}
