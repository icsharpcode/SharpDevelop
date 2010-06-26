// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
	/// Interaction logic for ContextActionsControl.xaml
	/// </summary>
	public partial class ContextActionsControl : UserControl
	{
		public ContextActionsControl()
		{
			InitializeComponent();
		}
		
		public new void Focus()
		{
			var firstButton = SearchVisualTree<Button>(this);
			if (firstButton == null)
				return;
			firstButton.Focus();
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
	}
}