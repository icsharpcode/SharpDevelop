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

namespace ICSharpCode.WpfDesign.PropertyGrid.Editors
{
	public partial class TextBoxEditor
	{
		/// <summary>
		/// Creates a new TextBoxEditor instance.
		/// </summary>
		public TextBoxEditor()
		{
			InitializeComponent();
		}
		
		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				BindingOperations.GetBindingExpressionBase(this, TextProperty).UpdateSource();
				SelectAll();
			} else if (e.Key == Key.Escape) {
				BindingOperations.GetBindingExpression(this, TextProperty).UpdateTarget();
			}
		}
	}
}
