// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of Helper.
	/// </summary>
	public static class Helper
	{
		public static void FillTree(ICSharpCode.TreeView.SharpTreeView tree,Module module)
		{
			var root = CreateTreeItem(module);
			tree.Root = root;
			tree.ShowRoot = false;
		
			foreach (var ns in module.Namespaces)
			{
				var namespaceNode = CreateTreeItem(ns);
				tree.Root.Children.Add(namespaceNode);
				
				foreach (var type in ns.Types)
				{
					var typeNode = CreateTreeItem(type);
					namespaceNode.Children.Add(typeNode);

					foreach (var method in type.Methods)
					{
						var methodName = CreateTreeItem(method);
						typeNode.Children.Add(methodName);
					}

					foreach (var field in type.Fields)
					{
						var fieldNode = CreateTreeItem(field);
						typeNode.Children.Add(fieldNode);
					}
				}
			}
		}
			
		private static DependecyTreeNode CreateTreeItem (INode node)
		{
			DependecyTreeNode dtn = new DependecyTreeNode(node);
			return dtn;
		}
		
		public static Color MixedWith(this Color c1, Color c2)
		{
			int r = Math.Min((c1.R + c2.R), 255);
			int g = Math.Min((c1.G + c2.G), 255);
			int b = Math.Min((c1.B + c2.B), 255);

			return new Color
			{
				R = Convert.ToByte(r),
				G = Convert.ToByte(g),
				B = Convert.ToByte(b)
			};
		}
		
		public static T FindVisualChild<T>( DependencyObject obj )
			where T : DependencyObject
		{
			// Search immediate children first (breadth-first)

			for( int i = 0; i < VisualTreeHelper.GetChildrenCount( obj ); i++ )
			{
				DependencyObject child = VisualTreeHelper.GetChild( obj, i );

				if( child != null && child is T )
				{
					return ( T )child;
				}
				else
				{
					T childOfChild = FindVisualChild<T>( child );

					if( childOfChild != null )
					{
						return childOfChild;
					}
				}
			}

			return null;
		}
	}
}
