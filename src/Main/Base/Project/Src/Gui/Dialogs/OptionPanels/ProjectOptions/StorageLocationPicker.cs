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
using System.Windows.Media;

using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Allows choosing the storage location for an MSBuild property.
	/// </summary>
	public class StorageLocationPicker : Control
	{
		static StorageLocationPicker()
		{
			// Style is defined in ProjectOptionPanel.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(StorageLocationPicker), new FrameworkPropertyMetadata(typeof(StorageLocationPicker)));
		}
		
		public static readonly DependencyProperty LocationProperty =
			DependencyProperty.Register(
				"Location", typeof(PropertyStorageLocations), typeof(StorageLocationPicker),
				new FrameworkPropertyMetadata(PropertyStorageLocations.Base, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				                              OnLocationChanged, CoerceLocation));
		
		public PropertyStorageLocations Location {
			get { return (PropertyStorageLocations)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		
		internal static readonly DependencyProperty EllipseBackgroundProperty =
			DependencyProperty.Register("EllipseBackground", typeof(Brush), typeof(StorageLocationPicker),
			                            new FrameworkPropertyMetadata(Brushes.Black));
		
		internal Brush EllipseBackground {
			get { return (Brush)GetValue(EllipseBackgroundProperty); }
			set { SetValue(EllipseBackgroundProperty, value); }
		}
		
		internal static readonly DependencyProperty IsUserFileProperty =
			DependencyProperty.Register("IsUserFile", typeof(bool), typeof(StorageLocationPicker),
			                            new FrameworkPropertyMetadata(false));
		
		internal bool IsUserFile {
			get { return (bool)GetValue(IsUserFileProperty); }
			set { SetValue(IsUserFileProperty, value); }
		}
		
		static void OnLocationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			StorageLocationPicker p = o as StorageLocationPicker;
			if (p != null) {
				PropertyStorageLocations location = (PropertyStorageLocations)e.NewValue;
				p.IsUserFile = (location & PropertyStorageLocations.UserFile) == PropertyStorageLocations.UserFile;
				switch (location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
					case PropertyStorageLocations.ConfigurationSpecific:
						p.EllipseBackground = Brushes.Blue;
						break;
					case PropertyStorageLocations.PlatformSpecific:
						p.EllipseBackground = Brushes.Red;
						break;
					case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
						p.EllipseBackground = Brushes.Violet;
						break;
					default:
						p.EllipseBackground = Brushes.Black;
						break;
				}
				if (p.contextMenu != null) {
					p.SetIsCheckedOnMenu(location);
				}
			}
		}
		
		static object CoerceLocation(DependencyObject d, object baseValue)
		{
			PropertyStorageLocations location = (PropertyStorageLocations)baseValue;
			if ((location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) != 0) {
				// remove 'Base' flag if any of the specific flags is set
				location &= ~PropertyStorageLocations.Base;
			} else {
				// otherwise, add 'Base' flag
				location |= PropertyStorageLocations.Base;
			}
			return location;
		}
		
		ContextMenu contextMenu;
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			Button button = Template.FindName("PART_button", this) as Button;
			if (button != null) {
				button.Click += button_Click;
				
				contextMenu = new ContextMenu();
				TextOptions.SetTextFormattingMode(contextMenu, TextFormattingMode.Display);
				contextMenu.Items.Add(CreateMenuItem("${res:Dialog.ProjectOptions.ConfigurationSpecific}", PropertyStorageLocations.ConfigurationSpecific));
				contextMenu.Items.Add(CreateMenuItem("${res:Dialog.ProjectOptions.PlatformSpecific}", PropertyStorageLocations.PlatformSpecific));
				contextMenu.Items.Add(CreateMenuItem("${res:Dialog.ProjectOptions.StoreInUserFile}", PropertyStorageLocations.UserFile));
				contextMenu.Items.Add(new Separator());
				
				MenuItem helpButton = new MenuItem();
				helpButton.SetValueToExtension(MenuItem.HeaderProperty, new LocalizeExtension("Global.HelpButtonText"));
				helpButton.Click += delegate(object sender, RoutedEventArgs e) {
					e.Handled = true;
					Core.MessageService.ShowMessage("${res:Dialog.ProjectOptions.StorageLocationHelp}");
				};
				contextMenu.Items.Add(helpButton);
				
				button.ContextMenu = contextMenu;
				SetIsCheckedOnMenu(this.Location);
			}
		}
		
		void SetIsCheckedOnMenu(PropertyStorageLocations location)
		{
			((MenuItem)contextMenu.Items[0]).IsChecked = (location & PropertyStorageLocations.ConfigurationSpecific) != 0;
			((MenuItem)contextMenu.Items[1]).IsChecked = (location & PropertyStorageLocations.PlatformSpecific) != 0;
			((MenuItem)contextMenu.Items[2]).IsChecked = (location & PropertyStorageLocations.UserFile) != 0;
		}
		
		MenuItem CreateMenuItem(string text, PropertyStorageLocations location)
		{
			MenuItem item = new MenuItem();
			item.SetValueToExtension(MenuItem.HeaderProperty, new StringParseExtension(text));
			item.Click += delegate(object sender, RoutedEventArgs e) {
				if ((this.Location & location) == 0) {
					this.Location |= location;
				} else {
					this.Location &= ~location;
				}
				e.Handled = true;
			};
			return item;
		}
		
		void button_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			contextMenu.IsOpen = true;
		}
	}
}
