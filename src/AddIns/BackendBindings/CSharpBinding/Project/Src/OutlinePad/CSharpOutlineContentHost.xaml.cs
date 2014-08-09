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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using CSharpBinding.Parser;

namespace CSharpBinding.OutlinePad
{
	/// <summary>
	/// Interaction logic for CSharpOutlineContentHost.xaml
	/// </summary>
	public partial class CSharpOutlineContentHost : DockPanel, IOutlineContentHost, IDisposable
	{
		ITextEditor editor;
		DispatcherTimer updateTreeTimer = new DispatcherTimer();
		const double updateDelayMilliseconds = 1000;
		DispatcherTimer scrollToNodeTimer = new DispatcherTimer();
		const double scrollDelayMilliseconds = 200;
		SyntaxTree syntaxTree;
		TextLocation? lastCaretLocation;
		CSharpOutlineNode selectedNode = null;		
		const bool optionSelectActiveTreeNode = true;
		const bool optionSelectRange = false;
		
		public CSharpOutlineContentHost(ITextEditor editor) {
			this.editor = editor;
			this.editor.Caret.LocationChanged += CaretLocationChanged;
			
			InitializeComponent();
			
			SD.ParserService.ParseInformationUpdated += ParseInfoUpdated;
			
			this.updateTreeTimer.Interval = TimeSpan.FromMilliseconds(updateDelayMilliseconds);
			this.updateTreeTimer.Tick += this.UpdateTreeTimer_Tick;

			this.scrollToNodeTimer.Interval = TimeSpan.FromMilliseconds(scrollDelayMilliseconds);
			this.scrollToNodeTimer.Tick += this.ScrollToNodeTimer_Tick;				
		}		
		
		void ParseInfoUpdated(object sender, ParseInformationEventArgs e) {
			if (this.editor == null || !FileUtility.IsEqualFileName(this.editor.FileName, e.FileName))
				return;
			
			var parseInfo = e.NewParseInformation as CSharpFullParseInformation;
			if (parseInfo != null && parseInfo.SyntaxTree != null) {
				this.updateTreeTimer.Stop();
				this.syntaxTree = parseInfo.SyntaxTree;
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
			FindNodeFromLocation(this.editor.Caret.Location, treeView.Root as CSharpOutlineNode);
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
		
		void FindNodeFromLocation(TextLocation location, CSharpOutlineNode node) {
			if (node == null)
				return;
			if (node.StartMarker.IsDeleted || node.EndMarker.IsDeleted)
				return;
			
			if (location.IsInside(node.StartMarker.Location, node.EndMarker.Location)
				&& (selectedNode == null || IsRangeInside(selectedNode.AstNodeItem.StartLocation, selectedNode.AstNodeItem.EndLocation,
														 node.AstNodeItem.StartLocation, node.AstNodeItem.EndLocation))) {
				selectedNode = node;
			}
			
			foreach(var child in node.Children) {
				FindNodeFromLocation(location, child  as CSharpOutlineNode);
			}
		}
		
		void UpdateTreeTimer_Tick(Object sender, EventArgs args) {
			this.updateTreeTimer.Stop();
			this.UpdateTree(this.syntaxTree);
			this.SelectActiveTreeNode(); 			
		}
		
		void ScrollToNodeTimer_Tick(Object sender, EventArgs args) {
			this.scrollToNodeTimer.Stop();
			if (selectedNode != null) {
				treeView.ScrollIntoView(selectedNode);			
			}
		}
			
		void UpdateTree(AstNode syntaxTree) {
			if (syntaxTree == null)
				return;
			
			if (treeView.Root == null) {			
				treeView.Root = new CSharpOutlineNode();
				SetNodeInfos(treeView.Root as CSharpOutlineNode, null, syntaxTree);
			}
			
			this.UpdateNode(treeView.Root as CSharpOutlineNode, syntaxTree);
		}
		
		void UpdateNode(CSharpOutlineNode node, AstNode dataNode) {
			if (dataNode == null || node == null)
				return;
			
			SetNodeInfos(node, null, dataNode);
			
			// Filter the children, for only the needed/wanted nodes
			var dataChildren = dataNode.Children.Where(childNode => AstNodeHelper.IsAllowedNode(childNode)).ToList();
			
			int childrenCount = node.Children.Count;
			int dataCount = dataChildren.Count;
			
			for (int i = 0; i < Math.Max(childrenCount, dataCount); i++) {
				if (i >= childrenCount) {
					node.Children.Add(BuildNode(node, dataChildren[i]));							

				} else if (i >= dataCount) {
					while (node.Children.Count > dataCount)	
						node.Children.RemoveAt(dataCount);
						
				} else {
					// Create new node, the old node doesn´t update (seems to be a bug)
					node.Children[i] = new CSharpOutlineNode();
					UpdateNode(node.Children[i] as CSharpOutlineNode, dataChildren[i]);				
				}
			}
		}
		
		CSharpOutlineNode BuildNode(CSharpOutlineNode parentNode, AstNode dataNode) {
						
			var node = new CSharpOutlineNode();
			SetNodeInfos(node, parentNode, dataNode);

			// Filter the children, for only the needed/wanted nodes
			var dataChildren = dataNode.Children.Where(v => AstNodeHelper.IsAllowedNode(v)).ToList();
			foreach (var child in dataChildren) {
				node.Children.Add(BuildNode(node, child));
			}
			return node;
		}
		
		void SetNodeInfos(CSharpOutlineNode node, CSharpOutlineNode parentNode, AstNode dataNode) {

			int startOffset = 0;
			int textLength = Math.Max(editor.Document.TextLength - 1,0);
			if (!dataNode.StartLocation.IsValid() || dataNode.StartLocation.Line > editor.Document.LineCount) 
				startOffset = 0;
			else	
				startOffset = editor.Document.GetOffset(dataNode.StartLocation);
						
			int endOffset = 0;
			if (!dataNode.EndLocation.IsValid() || dataNode.EndLocation.Line > editor.Document.LineCount) 
				endOffset = textLength;
			else	
				endOffset = editor.Document.GetOffset(dataNode.EndLocation);

			node.AstNodeItem = dataNode;        	
			node.StartMarker = editor.Document.CreateAnchor(MinMax(startOffset, 0, textLength));
			node.EndMarker = editor.Document.CreateAnchor(MinMax(endOffset, 0, textLength));
			node.Editor = editor;
			node.Parent = parentNode;		
				
			if (AstNodeHelper.IsDocument(dataNode))
               AstNodeHelper.SetDocumentNodeInfos(node, dataNode);
			
			if (AstNodeHelper.IsNameSpace(dataNode))
               AstNodeHelper.SetNameSpaceNodeInfos(node, dataNode);

            if (AstNodeHelper.IsRegionStart(dataNode))
                AstNodeHelper.SetRegionStartInfos(node, dataNode);

			if (AstNodeHelper.IsRegionEnd(dataNode))
                AstNodeHelper.SetRegionEndInfos(node, dataNode);
		
            if (AstNodeHelper.IsClass(dataNode))
                AstNodeHelper.SetClassNodeInfos(node, dataNode);

			if (AstNodeHelper.IsInterface(dataNode))
                AstNodeHelper.SetInterfaceNodeInfos(node, dataNode);
			
            if (AstNodeHelper.IsConstructor(dataNode))
                AstNodeHelper.SetConstructorNodeInfos(node, dataNode);

            if (AstNodeHelper.IsField(dataNode))
                AstNodeHelper.SetFieldNodeInfos(node, dataNode);

            if (AstNodeHelper.IsProperty(dataNode))
                AstNodeHelper.SetPropertyNodeInfos(node, dataNode);

            if (AstNodeHelper.IsMethod(dataNode))
                AstNodeHelper.SetMethodNodeInfos(node, dataNode);
			
			if (AstNodeHelper.IsEvent(dataNode))
                AstNodeHelper.SetEventNodeInfos(node, dataNode);
			
			if (AstNodeHelper.IsDelegate(dataNode))
                AstNodeHelper.SetDelegateNodeInfos(node, dataNode);
			
			if (AstNodeHelper.IsIndexer(dataNode))
                AstNodeHelper.SetIndexerNodeInfos(node, dataNode);
			
			if (AstNodeHelper.IsEnum(dataNode))
                AstNodeHelper.SetEnumNodeInfos(node, dataNode);
			
			if (AstNodeHelper.IsEnumMember(dataNode))
                AstNodeHelper.SetEnumMemberNodeInfos(node, dataNode);
			
			if (AstNodeHelper.IsStruct(dataNode))
                AstNodeHelper.SetStructNodeInfos(node, dataNode);			

			if (AstNodeHelper.IsOperator(dataNode))
                AstNodeHelper.SetOperatorNodeInfos(node, dataNode);			
		}
		
		static int MinMax(int value, int lower, int upper) {
			return Math.Min(Math.Max(value, lower), upper);
		}		
		
		void TreeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var node = treeView.SelectedItem as CSharpOutlineNode;
			if (node == null)
				return;
			
			if (optionSelectRange)
				editor.Select(node.StartMarker.Offset, node.EndMarker.Offset - node.StartMarker.Offset);			
			FileService.JumpToFilePosition(this.editor.FileName, node.AstNodeItem.StartLocation.Line, node.AstNodeItem.StartLocation.Column);		
		}
		
		void TreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e) {		
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
			
			this.syntaxTree = null;
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
