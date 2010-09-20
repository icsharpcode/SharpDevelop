// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	/// <summary>
	/// Interaction logic for GeneralEditorOptions.xaml
	/// </summary>
	public partial class GeneralEditorOptions : OptionPanel
	{
		public GeneralEditorOptions()
		{
			InitializeComponent();
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			CodeEditorOptions options = CodeEditorOptions.Instance;
			fontSelectionPanel.CurrentFont = WinFormsResourceService.LoadFont(
				options.FontFamily, (int)Math.Round(options.FontSize * 72.0 / 96.0));
		}
		
		public override bool SaveOptions()
		{
			CodeEditorOptions options = CodeEditorOptions.Instance;
			var font = fontSelectionPanel.CurrentFont;
			if (font != null) {
				options.FontFamily = font.Name;
				options.FontSize = Math.Round(font.Size * 96.0 / 72.0);
			}
			return base.SaveOptions();
		}
	}
}
