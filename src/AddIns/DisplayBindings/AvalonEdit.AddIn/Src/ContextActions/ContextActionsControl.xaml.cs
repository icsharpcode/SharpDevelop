// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ContextActionsControl),
			                            new FrameworkPropertyMetadata());
		
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
	}
}
