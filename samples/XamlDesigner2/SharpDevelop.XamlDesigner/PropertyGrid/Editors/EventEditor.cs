using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDevelop.XamlDesigner.Controls;
using System.Windows.Input;
using System.Windows.Data;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors
{
	public class EventEditor : EnterTextBox
	{
		public PropertyNode PropertyNode
		{
			get { return DataContext as PropertyNode; }
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				Commit(false);
			}
			else if (e.Key == Key.Escape) {
				BindingOperations.GetBindingExpression(this, TextProperty).UpdateTarget();
			}
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			Commit(false);
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			Commit(true);
		}

		void Commit(bool force)
		{
			var newText = Text.Trim();
			if (newText.Length == 0) {
				newText = null;
			}

			if (newText != PropertyNode.ValueString) {
				if (newText == null) {
					PropertyNode.Reset();
					return;
				}
				else {
					PropertyNode.ValueString = Text;
				}
				CreateHandler();
			}
			else {
				if (force) {
					CreateHandler();
				}
			}
		}

		void CreateHandler()
		{
			//DesignEnvironment.Instance.CreateEventHandler(PropertyModel);
		}
	}
}
