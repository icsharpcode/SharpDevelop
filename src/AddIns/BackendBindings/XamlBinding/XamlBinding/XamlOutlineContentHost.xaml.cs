// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
