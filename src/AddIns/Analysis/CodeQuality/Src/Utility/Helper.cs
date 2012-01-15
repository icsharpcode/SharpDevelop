// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.TreeView;

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	/// <summary>
	/// Description of Helper.
	/// </summary>
	public static class Helper
	{
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		
		public static void FillTree(SharpTreeView tree, AssemblyNode module)
		{
			var root = CreateTreeItem(module);
			tree.Root = root;
			tree.ShowRoot = false;
		
			var sortedNameSpaces = new List<NamespaceNode>();
			sortedNameSpaces.AddRange (module.Namespaces);
			sortedNameSpaces.Sort((a, b) => String.Compare(a.Name, b.Name));
			
			foreach (var ns in sortedNameSpaces)
//			foreach (var ns in module.Namespaces)
			{
				var namespaceNode = CreateTreeItem(ns);
				tree.Root.Children.Add(namespaceNode);
				
				var sortedTypes = new List<TypeNode>();
				sortedTypes.AddRange(ns.Types);
				sortedTypes.Sort((a, b) => String.Compare(a.Name, b.Name));
				foreach (var type in sortedTypes)
				{
					var typeNode = CreateTreeItem(type);
					namespaceNode.Children.Add(typeNode);

					var sortedMethods = new List<MethodNode>();
					sortedMethods.AddRange(type.Methods);
					sortedMethods.Sort((a, b) => String.Compare(a.Name, b.Name) );
					foreach (var method in sortedMethods)
					{
						var methodName = CreateTreeItem(method);
						typeNode.Children.Add(methodName);
					}

					
					var sortedFields = new List<FieldNode>();
					sortedFields.AddRange(type.Fields);
					sortedFields.Sort((a, b) => String.Compare(a.Name, b.Name) );
					foreach (var field in sortedFields)
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
			var percent = .5f;
			var amountFrom = 1.0f - percent;

            return Color.FromArgb(
            (byte)(c1.A * amountFrom + c2.A * percent),
            (byte)(c1.R * amountFrom + c2.R * percent),
            (byte)(c1.G * amountFrom + c2.G * percent),
            (byte)(c1.B * amountFrom + c2.B * percent));
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
