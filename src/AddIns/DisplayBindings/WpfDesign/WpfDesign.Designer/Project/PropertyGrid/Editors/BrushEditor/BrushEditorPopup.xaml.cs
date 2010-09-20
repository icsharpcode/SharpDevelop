// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors.BrushEditor
{
	public partial class BrushEditorPopup
	{
		public BrushEditorPopup()
		{
			InitializeComponent();
		}

		protected override void OnClosed(EventArgs e)
		{
		    base.OnClosed(e);
		    BrushEditorView.BrushEditor.Commit();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape) IsOpen = false;
		}
	}
}
