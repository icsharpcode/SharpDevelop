// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2667$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// The combo box used to enter the event handler which an event is connected to.
	/// </summary>
	sealed class EventHandlerEditor : ComboBox
	{
		readonly IPropertyEditorDataEvent dataEvent;
		
		public EventHandlerEditor(IPropertyEditorDataEvent dataEvent)
		{
			this.dataEvent = dataEvent;
			this.IsEditable = true;
			
			Loaded += delegate {
				dataEvent.HandlerNameChanged += OnEventHandlerNameChanged;
				OnEventHandlerNameChanged(null, null);
			};
			Unloaded += delegate {
				dataEvent.HandlerNameChanged -= OnEventHandlerNameChanged;
			};
		}
		
		void OnEventHandlerNameChanged(object sender, EventArgs e)
		{
			this.Text = dataEvent.HandlerName;
		}
		
		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			base.OnKeyDown(e);
			
			if (e.Handled)
				return;
			if (e.Key == Key.Enter) {
				dataEvent.HandlerName = this.Text;
				if (!string.IsNullOrEmpty(dataEvent.HandlerName)) {
					dataEvent.GoToHandler();
				}
				e.Handled = true;
			} else if (e.Key == Key.Escape) {
				this.Text = dataEvent.HandlerName;
				e.Handled = true;
			}
		}
		
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);
			if (string.IsNullOrEmpty(this.Text))
				dataEvent.HandlerName = null;
			else
				this.Text = dataEvent.HandlerName;
		}
		
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			e.Handled = true;
			DoubleClick();
		}
		
		public void DoubleClick()
		{
			dataEvent.GoToHandler();
		}
	}
}
