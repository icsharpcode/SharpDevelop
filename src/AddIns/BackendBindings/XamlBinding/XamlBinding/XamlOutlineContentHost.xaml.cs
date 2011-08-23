// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Interaction logic for XamlOutlineContentHost.xaml
	/// </summary>
	public partial class XamlOutlineContentHost : DockPanel, IOutlineContentHost, IDisposable
	{
		ITextEditor editor;
		
		public XamlOutlineContentHost(ITextEditor editor)
		{
			this.editor = editor;
			
			InitializeComponent();
			
			ParserService.ParseInformationUpdated += ParseInfoUpdated;
		}

		void ParseInfoUpdated(object sender, ParseInformationEventArgs e)
		{
			if (this.editor == null || !FileUtility.IsEqualFileName(this.editor.FileName, e.FileName))
				return;
			if (!e.IsPrimaryParseInfoForFile)
				return;
			
			var cu = e.NewCompilationUnit as XamlCompilationUnit;
			
			if (cu != null && cu.TreeRootNode != null)
				UpdateTree(cu.TreeRootNode);
		}
		
		void UpdateTree(NodeWrapper root)
		{
			if (this.treeView.Root == null)
				this.treeView.Root = BuildNode(root);
			else
				UpdateNode(this.treeView.Root as XamlOutlineNode, root);
		}
		
		void UpdateNode(XamlOutlineNode node, NodeWrapper dataNode)
		{
			if (dataNode != null && node != null) {
				node.Name = dataNode.Name;
				node.ElementName = dataNode.ElementName;
				node.Marker = editor.Document.CreateAnchor(Utils.MinMax(dataNode.StartOffset, 0, editor.Document.TextLength));
				node.EndMarker = editor.Document.CreateAnchor(Utils.MinMax(dataNode.EndOffset, 0, editor.Document.TextLength));
				
				int childrenCount = node.Children.Count;
				int dataCount = dataNode.Children.Count;
				
				for (int i = 0; i < Math.Max(childrenCount, dataCount); i++) {
					if (i >= childrenCount) {
						node.Children.Add(BuildNode(dataNode.Children[i]));
					} else if (i >= dataCount) {
						while (node.Children.Count > dataCount)
							node.Children.RemoveAt(dataCount);
					} else {
						UpdateNode(node.Children[i] as XamlOutlineNode, dataNode.Children[i]);
					}
				}
			}
		}
		
		XamlOutlineNode BuildNode(NodeWrapper item)
		{
			XamlOutlineNode node = new XamlOutlineNode() {
				Name = item.Name,
				ElementName = item.ElementName,
				Marker = editor.Document.CreateAnchor(Utils.MinMax(item.StartOffset, 0, editor.Document.TextLength - 1)),
				EndMarker = editor.Document.CreateAnchor(Utils.MinMax(item.EndOffset, 0, editor.Document.TextLength - 1)),
				Editor = editor
			};
			
			foreach (var child in item.Children)
				node.Children.Add(BuildNode(child));
			
			return node;
		}
		
		void TreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			XamlOutlineNode node = treeView.SelectedItem as XamlOutlineNode;
			editor.Select(node.Marker.Offset, node.EndMarker.Offset - node.Marker.Offset);
		}
		
		public object OutlineContent {
			get {
				return this;
			}
		}
		
		public void Dispose()
		{
			ParserService.ParseInformationUpdated -= ParseInfoUpdated;
		}
	}
}
