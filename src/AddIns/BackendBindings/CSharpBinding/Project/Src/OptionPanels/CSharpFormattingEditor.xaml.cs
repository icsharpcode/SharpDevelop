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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.OptionPanels
{
	/// <summary>
	/// Marker interface for group or option container.
	/// It doesn't need to have any members.
	/// </summary>
	internal interface IFormattingItemContainer
	{
	}
	
	/// <summary>
	/// Represents a container item for other container items in formatting editor list
	/// </summary>
	[ContentProperty("Children")]
	internal class FormattingGroupContainer : DependencyObject, IFormattingItemContainer
	{
		readonly ObservableCollection<IFormattingItemContainer> children = new ObservableCollection<IFormattingItemContainer>();
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(FormattingGroupContainer),
				new FrameworkPropertyMetadata());
		
		public string Text {
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		public ObservableCollection<IFormattingItemContainer> Children
		{
			get {
				return children;
			}
		}
	}
	
	/// <summary>
	/// Represents a container for formatting options.
	/// </summary>
	[ContentProperty("Children")]
	internal class FormattingOptionContainer : DependencyObject, IFormattingItemContainer
	{
		readonly ObservableCollection<FormattingOption> children = new ObservableCollection<FormattingOption>();
		
		public ObservableCollection<FormattingOption> Children
		{
			get {
				return children;
			}
		}
	}
	
	/// <summary>
	/// Represents a single formatting option in formatting editor.
	/// </summary>
	internal class FormattingOption : DependencyObject
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(FormattingOption),
				new FrameworkPropertyMetadata());
		public string Text {
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		public string Option
		{
			get;
			set;
		}
	}
	
	/// <summary>
	/// Interaction logic for CSharpFormattingEditor.xaml
	/// </summary>
	internal partial class CSharpFormattingEditor : UserControl
	{
		readonly ObservableCollection<ComboBoxItem> presetItems;
		readonly Dictionary<string, Func<CSharpFormattingOptions>> presets;
		
		public CSharpFormattingEditor()
		{
			presets = new Dictionary<string, Func<CSharpFormattingOptions>>();
			presetItems = new ObservableCollection<ComboBoxItem>();
			
			InitializeComponent();
			this.DataContext = this;
		}
		
		public static readonly DependencyProperty OptionsContainerProperty =
			DependencyProperty.Register("OptionsContainer", typeof(CSharpFormattingOptionsContainer), typeof(CSharpFormattingEditor),
				new FrameworkPropertyMetadata(OnOptionsContainerPropertyChanged));
		
		public CSharpFormattingOptionsContainer OptionsContainer {
			get { return (CSharpFormattingOptionsContainer)GetValue(OptionsContainerProperty); }
			set { SetValue(OptionsContainerProperty, value); }
		}
		
		public static readonly DependencyProperty AllowPresetsProperty =
			DependencyProperty.Register("AllowPresets", typeof(bool), typeof(CSharpFormattingEditor),
				new FrameworkPropertyMetadata());
		
		public bool AllowPresets {
			get { return (bool)GetValue(AllowPresetsProperty); }
			set { SetValue(AllowPresetsProperty, value); }
		}
		
		public static readonly DependencyProperty AutoFormattingProperty =
			DependencyProperty.Register("AutoFormatting", typeof(bool?), typeof(CSharpFormattingEditor),
				new FrameworkPropertyMetadata());
		
		public bool? AutoFormatting {
			get { return (bool?)GetValue(AutoFormattingProperty); }
			set { SetValue(AutoFormattingProperty, value); }
		}
		
		static void OnOptionsContainerPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var editor = o as CSharpFormattingEditor;
			if (editor != null) {
				var newContainer = e.NewValue as CSharpFormattingOptionsContainer;
				if (newContainer != null) {
					newContainer.PropertyChanged += (sender, eventArgs) => 
					{
						if (eventArgs.PropertyName == "AutoFormatting") {
							// Update AutoFormatting special option
							if (editor.AutoFormatting != newContainer.AutoFormatting)
								editor.AutoFormatting = newContainer.AutoFormatting;
						}
					};
					editor.autoFormattingCheckBox.IsThreeState = (newContainer.Parent != null);
					editor.AutoFormatting = newContainer.AutoFormatting;
					editor.FillPresetList(newContainer);
				}
			}
		}
		
		static void OnAutoFormattingPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var editor = o as CSharpFormattingEditor;
			if (editor != null) {
				var container = editor.OptionsContainer;
				if (container != null) {
					if (container.AutoFormatting != (bool?) e.NewValue)
						container.AutoFormatting = (bool?) e.NewValue;
				}
			}
		}
		
		private void FillPresetList(CSharpFormattingOptionsContainer container)
		{
			presets["(default)"] = () => null;
			presets["Empty"] = FormattingOptionsFactory.CreateEmpty;
			presets["SharpDevelop"] = FormattingOptionsFactory.CreateSharpDevelop;
			presets["Mono"] = FormattingOptionsFactory.CreateMono;
			presets["K&R"] = FormattingOptionsFactory.CreateKRStyle;
			presets["Allman"] = FormattingOptionsFactory.CreateAllman;
			presets["Whitesmiths"] = FormattingOptionsFactory.CreateWhitesmiths;
			presets["GNU"] = FormattingOptionsFactory.CreateGNU;
			
			// TODO Localize!
			if (container.Parent != null) {
				// Add a "default" preset
				presetItems.Add(new ComboBoxItem { Content = (container.Parent ?? container).DefaultText, Tag = "(default)" });
			}
			presetItems.Add(new ComboBoxItem { Content = "SharpDevelop", Tag = "SharpDevelop" });
			presetItems.Add(new ComboBoxItem { Content = "Mono", Tag = "Mono" });
			presetItems.Add(new ComboBoxItem { Content = "K&R", Tag = "K&R" });
			presetItems.Add(new ComboBoxItem { Content = "Allman", Tag = "Allman" });
			presetItems.Add(new ComboBoxItem { Content = "Whitesmiths", Tag = "Whitesmiths" });
			presetItems.Add(new ComboBoxItem { Content = "GNU", Tag = "GNU" });
			presetItems.Add(new ComboBoxItem { Content = "Empty", Tag = "Empty" });
			
			presetComboBox.SelectedIndex = 0;
		}
		
		public ObservableCollection<ComboBoxItem> Presets
		{
			get {
				return presetItems;
			}
		}
		
		void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			ComboBoxItem selectedPresetItem = presetComboBox.SelectedItem as ComboBoxItem;
			if (selectedPresetItem != null) {
				if (presets.ContainsKey((string) selectedPresetItem.Tag)) {
					var presetFunc = presets[(string) selectedPresetItem.Tag];
					
					// Ask user if he's sure to reset all previously defined settings
					if (presetFunc != null) {
						if (SD.MessageService.AskQuestion("${res:CSharpBinding.Formatting.PresetOverwriteQuestion}")) {
							OptionsContainer.Reset(presetFunc());
						}
					} else {
						SD.MessageService.ShowWarning("${res:CSharpBinding.Formatting.NoPresetSelectedMessage}");
					}
				}
			}
		}
		
		public ObservableCollection<IFormattingItemContainer> RootChildren
		{
			get {
				// rootEntries object is only the root container, its children should be shown directly
				var rootEntries = this.Resources["rootEntries"] as FormattingGroupContainer;
				if (rootEntries != null) {
					return rootEntries.Children;
				}
				
				return null;
			}
		}
	}
}