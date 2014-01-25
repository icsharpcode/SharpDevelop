// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
