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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.NRefactory.CSharp;
using CSharpBinding.OptionPanels;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// Offers an attached property to bind a formatting option to a ComboBox.
	/// </summary>
	internal static class FormattingOptionBinding
	{
		public static readonly DependencyProperty ContainerProperty =
			DependencyProperty.RegisterAttached("Container", typeof(CSharpFormattingOptionsContainer),
				typeof(FormattingOptionBinding),
				new FrameworkPropertyMetadata((o, e) => UpdateOptionBinding(o)));
		
		public static CSharpFormattingOptionsContainer GetContainer(Selector element)
		{
			return (CSharpFormattingOptionsContainer) element.GetValue(ContainerProperty);
		}
		
		public static void SetContainer(Selector element, CSharpFormattingOptionsContainer container)
		{
			element.SetValue(ContainerProperty, container);
		}
		
		public static readonly DependencyProperty FormattingOptionProperty =
			DependencyProperty.RegisterAttached("FormattingOption", typeof(FormattingOption),
				typeof(FormattingOptionBinding),
				new FrameworkPropertyMetadata((o, e) => UpdateOptionBinding(o)));
		
		public static FormattingOption GetFormattingOption(Selector element)
		{
			return (FormattingOption) element.GetValue(FormattingOptionProperty);
		}
		
		public static void SetFormattingOption(Selector element, FormattingOption container)
		{
			element.SetValue(FormattingOptionProperty, container);
		}
		
		static void UpdateOptionBinding(DependencyObject o)
		{
			ComboBox comboBox = o as ComboBox;
			CSharpFormattingOptionsContainer container = GetContainer(comboBox);
			FormattingOption option = GetFormattingOption(comboBox);
			if ((option != null) && (comboBox != null) && (container != null)) {
				if (container != null) {
					if (container.Parent != null) {
						// Add "default" entry in ComboBox
						comboBox.Items.Add(new ComboBoxItem {
							Content = (container.Parent ?? container).DefaultText,
							Tag = null
						});
						comboBox.SelectedIndex = 0;
					} else if (option.AlwaysAllowDefault) {
						// Also add "default" entry, but without changeable text by container
						comboBox.Items.Add(new ComboBoxItem {
							Content = "(default)",
							Tag = null
						});
						comboBox.SelectedIndex = 0;
					}
					
					Type optionType = container.GetOptionType(option.Option);
					FillComboValues(comboBox, optionType);
					UpdateComboBoxValue(container, option.Option, comboBox);
					
					comboBox.SelectionChanged += ComboBox_SelectionChanged;
					container.PropertyChanged += (sender, eventArgs) =>
					{
						if ((eventArgs.PropertyName == null) || (eventArgs.PropertyName == option.Option))
							UpdateComboBoxValue(container, option.Option, comboBox);
					};
				}
			}
		}
		
		static void UpdateComboBoxValue(CSharpFormattingOptionsContainer container, string option, ComboBox comboBox)
		{
			object currentValue = container.GetOption(option);
			comboBox.SelectedItem = comboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(item => object.Equals(currentValue, item.Tag));
		}

		static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			if (comboBox != null) {
				FormattingOption option = GetFormattingOption(comboBox);
				CSharpFormattingOptionsContainer container = GetContainer(comboBox);
				if ((container != null) && (option != null)) {
					ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
					if (selectedItem != null) {
						// Set option to appropriate value
						container.SetOption(option.Option, selectedItem.Tag);
					}
				}
			}
		}
		
		static void FillComboValues(ComboBox comboBox, Type type)
		{
			if (type == typeof(bool)) {
				FillBoolComboValues(comboBox);
			} else if (type == typeof(int)) {
				FillIntComboValues(comboBox);
			} else if (type == typeof(BraceStyle)) {
				FillBraceStyleComboValues(comboBox);
			} else if (type == typeof(PropertyFormatting)) {
				FillPropertyFormattingComboValues(comboBox);
			} else if (type == typeof(Wrapping)) {
				FillWrappingComboValues(comboBox);
			} else if (type == typeof(NewLinePlacement)) {
				FillNewLinePlacementComboValues(comboBox);
			} else if (type == typeof(UsingPlacement)) {
				FillUsingPlacementComboValues(comboBox);
			} else if (type == typeof(EmptyLineFormatting)) {
				FillEmptyLineFormattingComboValues(comboBox);
			}
		}
		
		static void FillBoolComboValues(ComboBox comboBox)
		{
			// TODO Add located resources!
			comboBox.Items.Add(new ComboBoxItem { Content = "Yes", Tag = true });
			comboBox.Items.Add(new ComboBoxItem { Content = "No", Tag = false });
		}
		
		static void FillIntComboValues(ComboBox comboBox)
		{
			for (int i = 0; i < 11; i++)
			{
				comboBox.Items.Add(new ComboBoxItem { Content = i.ToString(), Tag = i });
			}
		}
		
		static void FillBraceStyleComboValues(ComboBox comboBox)
		{
			// TODO Add located resources!
			comboBox.Items.Add(new ComboBoxItem { Content = "Do not change", Tag = BraceStyle.DoNotChange });
			comboBox.Items.Add(new ComboBoxItem { Content = "End of line", Tag = BraceStyle.EndOfLine });
			comboBox.Items.Add(new ComboBoxItem { Content = "End of line without space", Tag = BraceStyle.EndOfLineWithoutSpace });
			comboBox.Items.Add(new ComboBoxItem { Content = "Next line", Tag = BraceStyle.NextLine });
			comboBox.Items.Add(new ComboBoxItem { Content = "Next line shifted", Tag = BraceStyle.NextLineShifted });
			comboBox.Items.Add(new ComboBoxItem { Content = "Next line shifted 2", Tag = BraceStyle.NextLineShifted2 });
			comboBox.Items.Add(new ComboBoxItem { Content = "Banner style", Tag = BraceStyle.BannerStyle });
		}
		
		static void FillPropertyFormattingComboValues(ComboBox comboBox)
		{
			// TODO Add located resources!
			comboBox.Items.Add(new ComboBoxItem { Content = "Allow one line", Tag = PropertyFormatting.AllowOneLine });
			comboBox.Items.Add(new ComboBoxItem { Content = "Force one line", Tag = PropertyFormatting.ForceOneLine });
			comboBox.Items.Add(new ComboBoxItem { Content = "Force new line", Tag = PropertyFormatting.ForceNewLine });
		}
		
		static void FillWrappingComboValues(ComboBox comboBox)
		{
			// TODO Add located resources!
			comboBox.Items.Add(new ComboBoxItem { Content = "Do not change", Tag = Wrapping.DoNotChange });
			comboBox.Items.Add(new ComboBoxItem { Content = "Do not wrap", Tag = Wrapping.DoNotWrap });
			comboBox.Items.Add(new ComboBoxItem { Content = "Wrap always", Tag = Wrapping.WrapAlways });
			comboBox.Items.Add(new ComboBoxItem { Content = "Wrap if too long", Tag = Wrapping.WrapIfTooLong });
		}
		
		static void FillNewLinePlacementComboValues(ComboBox comboBox)
		{
			// TODO Add located resources!
			comboBox.Items.Add(new ComboBoxItem { Content = "Do not care", Tag = NewLinePlacement.DoNotCare });
			comboBox.Items.Add(new ComboBoxItem { Content = "New line", Tag = NewLinePlacement.NewLine });
			comboBox.Items.Add(new ComboBoxItem { Content = "Same line", Tag = NewLinePlacement.SameLine });
		}
		
		static void FillUsingPlacementComboValues(ComboBox comboBox)
		{
			// TODO Add located resources!
			comboBox.Items.Add(new ComboBoxItem { Content = "Top of file", Tag = UsingPlacement.TopOfFile });
			comboBox.Items.Add(new ComboBoxItem { Content = "Inside namespace", Tag = UsingPlacement.InsideNamespace });
		}
		
		static void FillEmptyLineFormattingComboValues(ComboBox comboBox)
		{
			// TODO Add located resources!
			comboBox.Items.Add(new ComboBoxItem { Content = "Do not change", Tag = EmptyLineFormatting.DoNotChange });
			comboBox.Items.Add(new ComboBoxItem { Content = "Indent", Tag = EmptyLineFormatting.Indent });
			comboBox.Items.Add(new ComboBoxItem { Content = "Do not indent", Tag = EmptyLineFormatting.DoNotIndent });
		}
	}
}
