// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.TreeView;
using System.Runtime.InteropServices;

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	/// <summary>
	/// Description of Helper.
	/// </summary>
	public static class Helper
	{
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		
		public static void FillTree(SharpTreeView tree, Module module)
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
			// depth-first Search

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
		
		
		public static ItemContainer GetParent<ItemContainer>(this DependencyObject obj)
			where ItemContainer : DependencyObject
		{
			if (obj == null)
				return null;
			while (VisualTreeHelper.GetParent(obj) != null && !(obj is ItemContainer))
			{
				obj = VisualTreeHelper.GetParent(obj);
			}
			
			// Will return null if not found
			return obj as ItemContainer;
		}
	}
}
