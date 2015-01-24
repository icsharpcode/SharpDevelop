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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Extends the Quick operation menu for the designer.
	/// </summary>
	[ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
	[ExtensionFor(typeof (FrameworkElement))]
	class QuickOperationMenuExtension : PrimarySelectionAdornerProvider
	{
		private QuickOperationMenu _menu;
		private KeyBinding _keyBinding;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			_menu = new QuickOperationMenu();
			_menu.Loaded += OnMenuLoaded;
			_menu.RenderTransform = ((Transform)this.ExtendedItem.GetCompleteAppliedTransformationToView().Inverse);
			var placement = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Top) {XOffset = 7, YOffset = 3.5};
			this.AddAdorners(placement, _menu);
			
			var kbs = this.ExtendedItem.Services.GetService(typeof (IKeyBindingService)) as IKeyBindingService;
			var command = new RelayCommand(delegate
			                                {
			                                	_menu.MainHeader.IsSubmenuOpen = true;
			                                	_menu.MainHeader.Focus();
			                                });
			_keyBinding=new KeyBinding(command, Key.Enter, ModifierKeys.Alt);
			if (kbs != null)
				kbs.RegisterBinding(_keyBinding);
		}

		private void OnMenuLoaded(object sender, EventArgs e)
		{
			if(_menu.MainHeader!=null)
				_menu.MainHeader.Click += MainHeaderClick;
			
			int menuItemsAdded = 0;
			var view = this.ExtendedItem.View;

			if (view != null) {
				string setValue;
				if(view is ItemsControl) {
					_menu.AddSubMenuInTheHeader(new MenuItem() {Header = "Edit Items"});
				}
				
				if(view is Grid) {
					_menu.AddSubMenuInTheHeader(new MenuItem() {Header = "Edit Rows"});
					_menu.AddSubMenuInTheHeader(new MenuItem() {Header = "Edit Columns"});
				}
				
				if (view is StackPanel) {
					var ch = new MenuItem() {Header = "Change Orientation"};
					_menu.AddSubMenuInTheHeader(ch);
					setValue = this.ExtendedItem.Properties[StackPanel.OrientationProperty].ValueOnInstance.ToString();
					_menu.AddSubMenuCheckable(ch, Enum.GetValues(typeof (Orientation)), Orientation.Vertical.ToString(), setValue);
					_menu.MainHeader.Items.Add(new Separator());
					menuItemsAdded++;
				}
				
				if(this.ExtendedItem.Parent!=null && this.ExtendedItem.Parent.View is DockPanel) {
					var sda = new MenuItem() {Header = "Set Dock to"};
					_menu.AddSubMenuInTheHeader(sda);
					setValue = this.ExtendedItem.Properties.GetAttachedProperty(DockPanel.DockProperty).ValueOnInstance.ToString();
					_menu.AddSubMenuCheckable(sda, Enum.GetValues(typeof (Dock)), Dock.Left.ToString(), setValue);
					_menu.MainHeader.Items.Add(new Separator());
					menuItemsAdded++;
				}

				var ha = new MenuItem() {Header = "Horizontal Alignment"};
				_menu.AddSubMenuInTheHeader(ha);
				setValue = this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance.ToString();
				_menu.AddSubMenuCheckable(ha, Enum.GetValues(typeof (HorizontalAlignment)), HorizontalAlignment.Stretch.ToString(), setValue);
				menuItemsAdded++;

				var va = new MenuItem() {Header = "Vertical Alignment"};
				_menu.AddSubMenuInTheHeader(va);
				setValue = this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance.ToString();
				_menu.AddSubMenuCheckable(va, Enum.GetValues(typeof (VerticalAlignment)), VerticalAlignment.Stretch.ToString(), setValue);
				menuItemsAdded++;
			}

			if (menuItemsAdded == 0) {
				OnRemove();
			}
		}

		private void MainHeaderClick(object sender, RoutedEventArgs e)
		{
			var clickedOn = e.Source as MenuItem;
			if (clickedOn != null) {
				var parent = clickedOn.Parent as MenuItem;
				if (parent != null) {
					
					if((string)clickedOn.Header=="Edit Items") {
						var editor = new CollectionEditor();
						var itemsControl=this.ExtendedItem.View as ItemsControl;
						if (itemsControl != null)
							editor.LoadItemsCollection(this.ExtendedItem);
						editor.Show();
					}
					
					if((string)clickedOn.Header=="Edit Rows") {
						var editor = new FlatCollectionEditor();
						var gd=this.ExtendedItem.View as Grid;
						if (gd != null)
							editor.LoadItemsCollection(this.ExtendedItem.Properties["RowDefinitions"]);
						editor.Show();
					}
					
					if((string)clickedOn.Header=="Edit Columns") {
						var editor = new FlatCollectionEditor();
						var gd=this.ExtendedItem.View as Grid;
						if (gd != null)
							editor.LoadItemsCollection(this.ExtendedItem.Properties["ColumnDefinitions"]);
						editor.Show();
					}
					
					if (parent.Header is string && (string) parent.Header == "Change Orientation") {
						var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
						if (value != null) {
							var orientation = Enum.Parse(typeof (Orientation), value);
							if (orientation != null)
								this.ExtendedItem.Properties[StackPanel.OrientationProperty].SetValue(orientation);
						}
					}
					if (parent.Header is string && (string)parent.Header == "Set Dock to") {
						var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
						if(value!=null) {
							var dock = Enum.Parse(typeof (Dock), value);
							if (dock != null)
								this.ExtendedItem.Properties.GetAttachedProperty(DockPanel.DockProperty).SetValue(dock);
						}
					}
					

					if (parent.Header is string && (string) parent.Header == "Horizontal Alignment") {
						var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
						if (value != null) {
							var ha = Enum.Parse(typeof (HorizontalAlignment), value);
							if (ha != null)
								this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(ha);
						}
					}

					if (parent.Header is string && (string) parent.Header == "Vertical Alignment") {
						var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
						if (value != null) {
							var va = Enum.Parse(typeof (VerticalAlignment), value);
							if (va != null)
								this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(va);
						}
					}
				}
			}
		}

		protected override void OnRemove()
		{
			base.OnRemove();
			_menu.Loaded -= OnMenuLoaded;
			var kbs = this.ExtendedItem.Services.GetService(typeof (IKeyBindingService)) as IKeyBindingService;
			if(kbs!=null)
				kbs.DeregisterBinding(_keyBinding);
		}
	}
}
