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

using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using HexEditor.Util;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace HexEditor.View
{
	/// <summary>
	/// Interaction logic for HexEditOptionsPanel.xaml
	/// </summary>
	public partial class HexEditOptionsPanel : OptionPanel
	{
		public HexEditOptionsPanel()
		{
			InitializeComponent();
			viewModes.ItemsSource = new[] {
				new { Value = ViewMode.Hexadecimal,
					Text = StringParser.Parse("${res:AddIns.HexEditor.NumeralSystem.Hexadecimal}") },
				new { Value = ViewMode.Octal,
					Text = StringParser.Parse("${res:AddIns.HexEditor.NumeralSystem.Octal}") },
				new { Value = ViewMode.Decimal,
					Text = StringParser.Parse("${res:AddIns.HexEditor.NumeralSystem.Decimal}") }
			};
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			bytesPerLine.Value = Settings.BytesPerLine;
			offsetFontButton.Content = offsetFont = Settings.OffsetFont;
			dataFontButton.Content = dataFont = Settings.DataFont;
			SetPreview(offsetPreview, offsetFont);
			SetPreview(dataPreview, dataFont);
		}
		
		public override bool SaveOptions()
		{
			Settings.BytesPerLine = (int)bytesPerLine.Value;
			Settings.OffsetFont = offsetFont;
			Settings.DataFont = dataFont;
			return base.SaveOptions();
		}
		
		System.Drawing.Font offsetFont, dataFont;
		
		void FontChooserClick(object sender, RoutedEventArgs e)
		{
			var chooser = new System.Windows.Forms.FontDialog();
			if (sender == offsetFontButton) {
				chooser.Font = offsetFont;
			}
			if (sender == dataFontButton) {
				chooser.Font = dataFont;
			}
			if (chooser.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				return;
			if (sender == offsetFontButton) {
				offsetFont = chooser.Font;
				SetPreview(offsetPreview, offsetFont);
				offsetFontButton.Content = offsetFont;
			}
			if (sender == dataFontButton) {
				dataFont = chooser.Font;
				SetPreview(dataPreview, dataFont);
				dataFontButton.Content = dataFont;
			}
		}

		void SetPreview(TextBlock preview, System.Drawing.Font font)
		{
			preview.FontFamily = new FontFamily(font.Name);
			preview.FontSize = font.Size / 72.0 * 96.0;
			if (font.Italic)
				preview.FontStyle = FontStyles.Italic;
			else
				preview.FontStyle = FontStyles.Normal;
			if (font.Bold)
				preview.FontWeight = FontWeights.Bold;
			else
				preview.FontWeight = FontWeights.Normal;
		}
	}
}
