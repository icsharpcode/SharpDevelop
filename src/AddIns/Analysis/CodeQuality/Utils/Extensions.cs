// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.CodeQuality.Gui;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeQuality
{
	/// <summary>
	/// Description of Extensions.
	/// </summary>
	public static class Extensions
	{
		public static void FillTree(SharpTreeView tree, IEnumerable<NodeBase> rootNodes)
		{
			tree.Root = new SharpTreeNode();
			if (rootNodes != null)
				CreateItems(rootNodes, tree.Root);
		}
		
		static void CreateItems(IEnumerable<NodeBase> nodes, SharpTreeNode parent)
		{
			foreach (NodeBase node in nodes) {
				var item = new MatrixTreeNode(node);
				parent.Children.Add(item);
				if (node.Children != null)
					CreateItems(node.Children, item);
			}
		}
		
		public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			// depth-first Search
			int count = VisualTreeHelper.GetChildrenCount(obj);
			for (int i = 0; i < count; i++) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);

				if (child is T)
					return (T)child;
				else {
					T childOfChild = FindVisualChild<T>(child);

					if (childOfChild != null)
						return childOfChild;
				}
			}

			return null;
		}
		
		public static TContainer GetParent<TContainer>(this DependencyObject obj) where TContainer : DependencyObject
		{
			if (obj == null)
				return null;
			while (VisualTreeHelper.GetParent(obj) != null && !(obj is TContainer))
				obj = VisualTreeHelper.GetParent(obj);
			return obj as TContainer;
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
		
		static readonly IAmbience amb = new CSharpAmbience() { ConversionFlags = ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames | ConversionFlags.ShowReturnType | ConversionFlags.ShowTypeParameterList };
		
		public static string PrintFullName(this IEntity entity)
		{
			return amb.ConvertSymbol(entity);
		}
	}
}