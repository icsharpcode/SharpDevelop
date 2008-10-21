using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public class EnterTextBox : TextBox
	{
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				var b = BindingOperations.GetBindingExpressionBase(this, TextProperty);
				if (b != null) {
					b.UpdateSource();
				}
				SelectAll();
			}
			else if (e.Key == Key.Escape) {
				var b = BindingOperations.GetBindingExpressionBase(this, TextProperty);
				if (b != null) {
					b.UpdateTarget();
				}
			}
		}
	}
}
