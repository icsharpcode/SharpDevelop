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
			List<object> result = new List<object>();
			foreach (ToolbarItemDescriptor descriptor in descriptors) {
				object item = CreateToolBarItemFromDescriptor(inputBindingOwner, descriptor);
				IMenuItemBuilder submenuBuilder = item as IMenuItemBuilder;
				if (submenuBuilder != null) {
					result.AddRange(submenuBuilder.BuildItems(descriptor.Codon, descriptor.Parameter));
				} else {
					result.Add(item);
				}
			}
			return result;
		}
		
		static object CreateToolBarItemFromDescriptor(UIElement inputBindingOwner, ToolbarItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			object caller = descriptor.Parameter;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";
			
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new ConditionalSeparator(codon, caller, true, descriptor.Conditions);
				case "CheckBox":
					return new ToolBarCheckBox(codon, caller, descriptor.Conditions);
				case "Item":
					return new ToolBarButton(inputBindingOwner, codon, caller, createCommand, descriptor.Conditions);
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
					return codon.AddIn.CreateObject(codon.Properties["class"]);
				case "Custom":
					object result = codon.AddIn.CreateObject(codon.Properties["class"]);
					if (result is ComboBox)
						((ComboBox)result).SetResourceReference(FrameworkElement.StyleProperty, ToolBar.ComboBoxStyleKey);
					if (result is ICustomToolBarItem)
						((ICustomToolBarItem)result).Initialize(inputBindingOwner, codon, caller);
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
				image = new Image();
				image.Source = PresentationResourceService.GetBitmapSource(StringParser.Parse(codon.Properties["icon"]));
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

	public interface ICustomToolBarItem
	{
		void Initialize(UIElement inputBindingOwner, Codon codon, object owner);
	}
}
