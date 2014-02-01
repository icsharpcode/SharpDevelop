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
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core.Presentation;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// ContextActionsControl accepts IEnumerable&lt;ContextActionViewModel&gt; as its DataContext.
	/// </summary>
	public partial class ContextActionsControl : UserControl
	{
		public ContextActionsControl()
		{
			InitializeComponent();
		}
		
		public event EventHandler ActionExecuted;
		public event RoutedEventHandler ActionSelected;
		public event RoutedEventHandler ActionUnselected;
		
		public new void Focus()
		{
			var firstButton = WpfTreeNavigation.TryFindChild<Button>(this);
			if (firstButton != null)
				firstButton.Focus();
			else
				TreeView.Focus();
		}
		
		void TreeViewLoaded(object sender, RoutedEventArgs e)
		{
			if (TreeView.IsVisible) {
				this.Focus();
			}
		}

		void ActionButtonClick(object sender, RoutedEventArgs e)
		{
			if (ActionExecuted != null)
				ActionExecuted(this, EventArgs.Empty);
		}
		
		void ActionGotFocus(object sender, RoutedEventArgs e)
		{
			if (ActionSelected != null)
				ActionSelected(this, e);
		}
		
		void ActionLostFocus(object sender, RoutedEventArgs e)
		{
			if (ActionUnselected != null)
				ActionUnselected(this, e);
		}
		
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ContextActionsControl),
			                            new FrameworkPropertyMetadata());
		
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
	}
}
