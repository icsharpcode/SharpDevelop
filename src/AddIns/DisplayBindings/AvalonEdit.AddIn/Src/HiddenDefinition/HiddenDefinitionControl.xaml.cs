// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.AddIn.Options;

namespace ICSharpCode.AvalonEdit.AddIn.HiddenDefinition
{
	public partial class HiddenDefinitionControl : UserControl
	{
		public HiddenDefinitionControl()
		{
			InitializeComponent();
			DefinitionTextBlock.FontFamily = new FontFamily(CodeEditorOptions.Instance.FontFamily);
			DefinitionTextBlock.FontSize = CodeEditorOptions.Instance.FontSize;
		}
		
		public string DefinitionText {
			get { return this.DefinitionTextBlock.Text; }
			set { this.DefinitionTextBlock.Text = value; }
		}
	}
}