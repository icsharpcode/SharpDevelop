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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.NRefactory;


namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Interaction logic for XamlOutlineContentHost.xaml
	/// </summary>
	public partial class XamlOutlineContentHost : DockPanel, IOutlineContentHost, IDisposable
	{
		ITextEditor editor;
		DispatcherTimer updateTreeTimer = new DispatcherTimer();
		const double updateDelayMilliseconds = 500;
		DispatcherTimer scrollToNodeTimer = new DispatcherTimer();
		const double scrollDelayMilliseconds = 200;
		AXmlDocument document;
		TextLocation? lastCaretLocation;
		XamlOutlineNode selectedNode = null;		
		const bool optionSelectActiveTreeNode = true;
		const bool optionSelectRange = false;
		
		public XamlOutlineContentHost(ITextEditor editor)
		{
			this.editor = editor;
			this.editor.Caret.LocationChanged += CaretLocationChanged;
			
			InitializeComponent();
			
			SD.ParserService.ParseInformationUpdated += ParseInfoUpdated;
			
			this.updateTreeTimer.Interval = TimeSpan.FromMilliseconds(updateDelayMilliseconds);
			this.updateTreeTimer.Tick += this.UpdateTreeTimer_Tick;

			this.scrollToNodeTimer.Interval = TimeSpan.FromMilliseconds(scrollDelayMilliseconds);
			this.scrollToNodeTimer.Tick += this.ScrollToNodeTimer_Tick;	
		}

		void ParseInfoUpdated(object sender, ParseInformationEventArgs e)
		{
			if (this.editor == null || !FileUtility.IsEqualFileName(this.editor.FileName, e.FileName))
				return;
			
			var parseInfo = e.NewParseInformation as XamlFullParseInformation;
			if (parseInfo != null && parseInfo.Document != null) {
				this.updateTreeTimer.Stop();
				this.document = parseInfo.Document;
				this.updateTreeTimer.Start();
			}	
		}
		
		void CaretLocationChanged(object sender, EventArgs e)
		{
			SelectActiveTreeNode();
		}
		
		void SelectActiveTreeNode() {
			if (!optionSelectActiveTreeNode)
				return;
			// prevent unnecessary looping, when both CaretLocationChanged and ParseUpdateChanged are fired.
			if (this.lastCaretLocation.HasValue && this.lastCaretLocation == this.editor.Caret.Location)
				return;
			
			this.lastCaretLocation = this.editor.Caret.Location;
			selectedNode = null;
			FindNodeFromLocation(this.editor.Caret.Location, treeView.Root as XamlOutlineNode);			
			if (selectedNode != null && treeView.SelectedItem != selectedNode) {
				treeView.SelectedItem = selectedNode;				
				if (!scrollToNodeTimer.IsEnabled) {				
					scrollToNodeTimer.Start();		
				}		
			}
		}
		
		bool IsRangeInside(TextLocation outerStartLocation, TextLocation outerEndLocation,
						   TextLocation innerStartLocation, TextLocation innerEndLocation) {
			if (outerStartLocation.IsEmpty || outerStartLocation.IsInfinite() ||
				outerEndLocation.IsEmpty   || outerEndLocation.IsInfinite()   ||
				innerStartLocation.IsEmpty || innerStartLocation.IsInfinite() ||
				innerEndLocation.IsEmpty   || innerEndLocation.IsInfinite())
				return false;
			
			const int virtualLineLength = 200;
			var outerRange = (outerEndLocation.Line - outerStartLocation.Line) * virtualLineLength - outerStartLocation.Column + outerEndLocation.Column;
			var innerRange = (innerEndLocation.Line - innerStartLocation.Line) * virtualLineLength - innerStartLocation.Column + innerEndLocation.Column;
			return innerRange < outerRange;
		}
		
		void FindNodeFromLocation(TextLocation location, XamlOutlineNode node) {
			if (node == null)
				return;
			if (node.StartMarker.IsDeleted || node.EndMarker.IsDeleted)
				return;
			
			if (location.IsInside(node.StartMarker.Location, node.EndMarker.Location)
				&& (selectedNode == null || IsRangeInside(selectedNode.StartLocation, selectedNode.EndLocation,
														 node.StartLocation, node.EndLocation))) {
				selectedNode = node;
			}
			
			foreach(var child in node.Children) {
				FindNodeFromLocation(location, child  as XamlOutlineNode);
			}
		}
		
		void UpdateTreeTimer_Tick(Object sender, EventArgs args) {
			this.updateTreeTimer.Stop();
			this.UpdateTree(this.document);
			this.SelectActiveTreeNode(); 			
		}
		
		void ScrollToNodeTimer_Tick(Object sender, EventArgs args) {
			this.scrollToNodeTimer.Stop();
			if (selectedNode != null) {
				treeView.ScrollIntoView(selectedNode);			
			}
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
			int textLength = Math.Max(editor.Document.TextLength - 1,0);
			if (dataNode is AXmlElement) {
				var item = (AXmlElement)dataNode;
				node.Name = item.GetAttributeValue("Name") ?? item.GetAttributeValue(XamlConst.XamlNamespace, "Name");
				node.ElementName = item.Name;
			}
			node.StartMarker = editor.Document.CreateAnchor(Utils.MinMax(dataNode.StartOffset, 0, textLength));
			node.EndMarker = editor.Document.CreateAnchor(Utils.MinMax(dataNode.EndOffset, 0, textLength));
			node.StartLocation = new TextLocation(node.StartMarker.Line, node.StartMarker.Column);
			node.EndLocation = new TextLocation(node.EndMarker.Line, node.EndMarker.Column);
			
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
				StartMarker = editor.Document.CreateAnchor(Utils.MinMax(item.StartOffset, 0, editor.Document.TextLength - 1)),
				EndMarker = editor.Document.CreateAnchor(Utils.MinMax(item.EndOffset, 0, editor.Document.TextLength - 1)),
				Editor = editor,
				XmlNodeItem = item
			};
			
			node.StartLocation = new TextLocation(node.StartMarker.Line, node.StartMarker.Column);
			node.EndLocation = new TextLocation(node.EndMarker.Line, node.EndMarker.Column);
			node.IsExpanded = true;
			
			foreach (var child in item.Children.OfType<AXmlElement>())
				node.Children.Add(BuildNode(child));
			
			return node;
		}
		
		void TreeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var node = treeView.SelectedItem as XamlOutlineNode;
			if (node == null)
				return;
			
			if (optionSelectRange)
				editor.Select(node.StartMarker.Offset, node.EndMarker.Offset - node.StartMarker.Offset);			
			FileService.JumpToFilePosition(this.editor.FileName, node.StartMarker.Line, node.StartMarker.Column);		
		}
		
		void TreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
//			XamlOutlineNode node = treeView.SelectedItem as XamlOutlineNode;
//			if (node == null) return;
//			editor.Select(node.StartMarker.Offset, node.EndMarker.Offset - node.StartMarker.Offset);
		}
		
		public object OutlineContent {
			get { return this; }
		}
		
		public void Dispose() {
			SD.ParserService.ParseInformationUpdated -= ParseInfoUpdated;
			
			if (this.editor != null) {
				if (this.editor.Caret != null)
					this.editor.Caret.LocationChanged -= CaretLocationChanged;
				this.editor = null;
			}
			
			this.document = null;
			this.lastCaretLocation = null;
			this.selectedNode = null;
			
			if (this.updateTreeTimer != null) {
				this.updateTreeTimer.Stop();
				this.updateTreeTimer.Tick -= this.UpdateTreeTimer_Tick;
				this.updateTreeTimer = null;
			}

			if (this.scrollToNodeTimer != null) {
				this.scrollToNodeTimer.Stop();
				this.scrollToNodeTimer.Tick -= this.ScrollToNodeTimer_Tick;
				this.scrollToNodeTimer = null;
			}	
		}
	}
}
