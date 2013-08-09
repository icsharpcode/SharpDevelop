// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public class ClearableTextBox : EnterTextBox
	{
		private Button textRemoverButton;

		static ClearableTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (ClearableTextBox),
			                                         new FrameworkPropertyMetadata(typeof (ClearableTextBox)));
		}

		public ClearableTextBox()
		{
			this.GotFocus += this.TextBoxGotFocus;
			this.LostFocus += this.TextBoxLostFocus;
			this.TextChanged += this.TextBoxTextChanged;
			this.KeyUp += this.ClearableTextBox_KeyUp;
		}

		void ClearableTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				this.TextRemoverClick(sender, null);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.textRemoverButton = this.GetTemplateChild("TextRemover") as Button;
			if (null != this.textRemoverButton)
			{
				this.textRemoverButton.Click += this.TextRemoverClick;
			}

			this.UpdateState();
		}

		protected void UpdateState()
		{
			if (string.IsNullOrEmpty(this.Text))
			{
				VisualStateManager.GoToState(this, "TextRemoverHidden", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "TextRemoverVisible", true);
			}
		}

		private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateState();
		}

		private void TextRemoverClick(object sender, RoutedEventArgs e)
		{
			this.Text = string.Empty;
			this.Focus();
		}

		private void TextBoxGotFocus(object sender, RoutedEventArgs e)
		{
			this.UpdateState();
		}

		private void TextBoxLostFocus(object sender, RoutedEventArgs e)
		{
			this.UpdateState();
		}
	}
}
