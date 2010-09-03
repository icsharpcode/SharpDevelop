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

namespace ICSharpCode.SharpDevelop.Refactoring
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
			var firstButton = SearchVisualTree<Button>(this);
			if (firstButton == null)
				return;
			firstButton.Focus();
		}
		
		void ActionButtonClick(object sender, RoutedEventArgs e)
		{
			if (ActionExecuted != null)
				ActionExecuted(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Returns the first occurence of object of type <paramref name="T" /> in the visual tree of <paramref name="dependencyObject" />.
		/// </summary>
		public static T SearchVisualTree<T>(DependencyObject root) where T : DependencyObject
		{
			if (root is T)
				return (T)root;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
			{
				var foundChild = SearchVisualTree<T>(VisualTreeHelper.GetChild(root, i));
				if (foundChild != null)
					return foundChild;
			}
			return null;
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
