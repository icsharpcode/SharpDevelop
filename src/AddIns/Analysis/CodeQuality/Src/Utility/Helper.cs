/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.09.2011
 * Time: 23:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of Helper.
	/// </summary>
	public class Helper
	{
		public static  void FillTree (ICSharpCode.TreeView.SharpTreeView tree,Module module)
		{
			tree.ShowRoot = false;
			var root = CreateTreeItem(module);
			tree.Root = root;
		
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
						namespaceNode.Children.Add(methodName);
					}

					foreach (var field in type.Fields)
					{
						var fieldNode = CreateTreeItem(field);
						namespaceNode.Children.Add(fieldNode);
					}
				}
			}
		}
			
			
		private static DependecyTreeNode CreateTreeItem (INode node)
		{
			DependecyTreeNode dtn = new DependecyTreeNode(node);
			return dtn;
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
