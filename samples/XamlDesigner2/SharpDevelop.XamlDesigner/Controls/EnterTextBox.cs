using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class EnterTextBox : TextBox
	{
		public EnterTextBox()
		{
			SetResourceReference(StyleProperty, typeof(TextBox));
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				var expr = BindingOperations.GetBindingExpressionBase(this, TextProperty);
				if (expr != null) {
					expr.UpdateSource();
					if (expr.Status == BindingStatus.UpdateSourceError) {
						expr.UpdateTarget();
					}
				}
				SelectAll();
			}
			else {
				if (e.Key == Key.Escape) {
					var expr = BindingOperations.GetBindingExpressionBase(this, TextProperty);
					if (expr != null) {
						expr.UpdateTarget();
					}
				}
			}
		}
	}
}
