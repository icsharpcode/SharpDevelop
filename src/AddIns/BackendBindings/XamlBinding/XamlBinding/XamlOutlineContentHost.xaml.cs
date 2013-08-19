// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

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
			
			SD.ParserService.ParseInformationUpdated += ParseInfoUpdated;
		}

		void ParseInfoUpdated(object sender, ParseInformationEventArgs e)
		{
			if (this.editor == null || !FileUtility.IsEqualFileName(this.editor.FileName, e.FileName))
				return;
			
			var parseInfo = e.NewParseInformation as XamlFullParseInformation;
			if (parseInfo != null && parseInfo.Document != null)
				UpdateTree(parseInfo.Document);
		}
		
		void UpdateTree(AXmlDocument root)
		{
			if (treeView.Root == null) {
				treeView.Root = new XamlOutlineNode {
					ElementName = "Document Root",
					Name = Path.GetFileName(editor.FileName),
					Editor = editor
				};
			}
			
			UpdateNode(treeView.Root as XamlOutlineNode, root);
		}
		
		void UpdateNode(XamlOutlineNode node, AXmlObject dataNode)
		{
			if (dataNode == null || node == null)
				return;
			if (dataNode is AXmlElement) {
				var item = (AXmlElement)dataNode;
				node.Name = item.GetAttributeValue("Name") ?? item.GetAttributeValue(XamlConst.XamlNamespace, "Name");
				node.ElementName = item.Name;
			}
			node.Marker = editor.Document.CreateAnchor(Utils.MinMax(dataNode.StartOffset, 0, editor.Document.TextLength));
			node.EndMarker = editor.Document.CreateAnchor(Utils.MinMax(dataNode.EndOffset, 0, editor.Document.TextLength));
			
			var dataChildren = dataNode.Children.OfType<AXmlElement>().ToList();
			
			int childrenCount = node.Children.Count;
			int dataCount = dataChildren.Count;
			
			for (int i = 0; i < Math.Max(childrenCount, dataCount); i++) {
				if (i >= childrenCount) {
					node.Children.Add(BuildNode(dataChildren[i]));
				} else if (i >= dataCount) {
					while (node.Children.Count > dataCount)
						node.Children.RemoveAt(dataCount);
				} else {
					UpdateNode(node.Children[i] as XamlOutlineNode, dataChildren[i]);
				}
			}
		}
		
		XamlOutlineNode BuildNode(AXmlElement item)
		{
			XamlOutlineNode node = new XamlOutlineNode {
				Name = item.GetAttributeValue("Name") ?? item.GetAttributeValue(XamlConst.XamlNamespace, "Name"),
				ElementName = item.Name,
				Marker = editor.Document.CreateAnchor(Utils.MinMax(item.StartOffset, 0, editor.Document.TextLength - 1)),
				EndMarker = editor.Document.CreateAnchor(Utils.MinMax(item.EndOffset, 0, editor.Document.TextLength - 1)),
				Editor = editor
			};
			
			foreach (var child in item.Children.OfType<AXmlElement>())
				node.Children.Add(BuildNode(child));
			
			return node;
		}
		
		void TreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			XamlOutlineNode node = treeView.SelectedItem as XamlOutlineNode;
			editor.Select(node.Marker.Offset, node.EndMarker.Offset - node.Marker.Offset);
		}
		
		public object OutlineContent {
			get { return this; }
		}
		
		public void Dispose()
		{
			SD.ParserService.ParseInformationUpdated -= ParseInfoUpdated;
		}
	}
}
