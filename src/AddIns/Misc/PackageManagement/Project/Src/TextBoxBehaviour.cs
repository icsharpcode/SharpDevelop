// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.PackageManagement
{
	public static class TextBoxBehaviour
	{
		public static readonly DependencyProperty EnterKeyCommandProperty
			= DependencyProperty.RegisterAttached(
				"EnterKeyCommand",
				typeof(ICommand),
				typeof(TextBoxBehaviour),
				new FrameworkPropertyMetadata(null, OnEnterKeyCommandPropertyChanged));
		
		public static ICommand GetEnterKeyCommand(DependencyObject dependencyObject)
		{
			return dependencyObject.GetValue(EnterKeyCommandProperty) as ICommand;
		}
		
		public static void SetEnterKeyCommand(DependencyObject dependencyObject, ICommand command)
		{
			dependencyObject.SetValue(EnterKeyCommandProperty, command);
		}
		
		static void OnEnterKeyCommandPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			TextBox textBox = dependencyObject as TextBox;
			if (textBox == null) {
				return;
			}
			
			ICommand command = e.NewValue as ICommand;
			if (command == null) {
				textBox.KeyDown -= TextBoxKeyDown;
			} else {
				textBox.KeyDown += TextBoxKeyDown;
			}
		}

		static void TextBoxKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox == null) {
				return;
			}
			
			if (e.Key == Key.Enter) {
				ICommand command = GetEnterKeyCommand(textBox);
				if (command != null) {
					command.Execute(null);
				}
			}
		}
	}
}
