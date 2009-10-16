// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
			options.FontFamily = fontSelectionPanel.CurrentFont.Name;
			options.FontSize = Math.Round(fontSelectionPanel.CurrentFont.Size * 96.0 / 72.0);
			return base.SaveOptions();
		}
	}
}