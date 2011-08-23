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
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Interaction logic for DiffControl.xaml
	/// </summary>
	public partial class DiffControl : UserControl
	{
		public DiffControl()
		{
			InitializeComponent();
			
			revertButton.Content = PresentationResourceService.GetImage("Icons.16x16.UndoIcon");
			copyButton.Content = PresentationResourceService.GetImage("Icons.16x16.CopyIcon");
		}
		
		void CopyButtonClick(object sender, RoutedEventArgs e)
		{
			if (editor.SelectionLength == 0) {
				int offset = editor.CaretOffset;
				editor.SelectAll();
				editor.Copy();
				editor.Select(offset, 0);
			} else {
				editor.Copy();
			}
		}
	}
}