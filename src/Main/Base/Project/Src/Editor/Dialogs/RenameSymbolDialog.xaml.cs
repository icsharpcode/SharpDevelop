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

namespace ICSharpCode.SharpDevelop.Editor.Dialogs
{
	/// <summary>
	/// Interaction logic for RenameSymbolDialog.xaml
	/// </summary>
	public partial class RenameSymbolDialog : Window
	{
		public static readonly DependencyProperty NewSymbolNameProperty =
			DependencyProperty.Register("NewSymbolName", typeof(string), typeof(RenameSymbolDialog),
			                            new FrameworkPropertyMetadata());
		public static readonly DependencyProperty IsValidSymbolNameProperty =
			DependencyProperty.Register("IsValidSymbolName", typeof(bool), typeof(RenameSymbolDialog),
			                            new FrameworkPropertyMetadata());
		public static readonly DependencyProperty IsErrorProperty =
			DependencyProperty.Register("IsError", typeof(bool), typeof(RenameSymbolDialog),
			                            new FrameworkPropertyMetadata());
		
		Predicate<string> symbolNameValidationPredicate = n => true; // Just to always have one
		
		public RenameSymbolDialog(Predicate<string> symbolNameValidator)
		{
			InitializeComponent();
			
			this.DataContext = this;
			
			if (symbolNameValidator != null) {
				symbolNameValidationPredicate = symbolNameValidator;
			}
			
			// Set focus into TextBox
			this.symbolNameTextBox.Focus();
			this.IsVisibleChanged += (sender, e) => this.symbolNameTextBox.SelectAll();
		}
		
		public string OldSymbolName
		{
			get;
			set;
		}
		
		public string NewSymbolName
		{
			get { return (string)GetValue(NewSymbolNameProperty); }
			set { SetValue(NewSymbolNameProperty, value); }
		}
		
		public bool IsValidSymbolName
		{
			get { return (bool)GetValue(IsValidSymbolNameProperty); }
			set { SetValue(IsValidSymbolNameProperty, value); }
		}
		
		public bool IsError
		{
			get { return (bool)GetValue(IsErrorProperty); }
			set { SetValue(IsErrorProperty, value); }
		}
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			
			if (e.Property.Name == "NewSymbolName")
				ValidateSymbolName();
		}
		
		void okButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		
		void ValidateSymbolName()
		{
			// Execute defined validator on input
			bool isValidSymbolName = symbolNameValidationPredicate(NewSymbolName);
			
			// Update error states
			IsValidSymbolName = isValidSymbolName && (OldSymbolName != NewSymbolName);
			IsError = !isValidSymbolName;
		}
	}
}
