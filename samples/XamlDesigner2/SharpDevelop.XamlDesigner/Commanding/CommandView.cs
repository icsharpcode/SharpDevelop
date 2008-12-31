using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Diagnostics;
using System.Windows.Input;

namespace SharpDevelop.XamlDesigner.Commanding
{
	public class CommandView : DependencyObject
	{
		public string CommandName { get; internal set; }

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(CommandView));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty ShortcutProperty =
			DependencyProperty.Register("Shortcut", typeof(SimpleKeyGesture), typeof(CommandView));

		public SimpleKeyGesture Shortcut
		{
			get { return (SimpleKeyGesture)GetValue(ShortcutProperty); }
			set { SetValue(ShortcutProperty, value); }
		}

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register("Icon", typeof(object), typeof(CommandView));

		public object Icon
		{
			get { return (object)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.Register("IsEnabled", typeof(bool), typeof(CommandView));

		public bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		public void InitializeFromRoutedCommand(RoutedCommand command)
		{
			var keyGesture = command.InputGestures.OfType<KeyGesture>().FirstOrDefault();
			if (keyGesture != null) {
				Shortcut = new SimpleKeyGesture() {
					Key = keyGesture.Key,
					Modifiers = keyGesture.Modifiers
				};
			}
			var uiCommand = command as RoutedUICommand;
			if (uiCommand != null) {
				Text = uiCommand.Text;
			}
		}
	}
}
