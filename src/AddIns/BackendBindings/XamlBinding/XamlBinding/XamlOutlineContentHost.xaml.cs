// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TreeView;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Interaction logic for XamlOutlineContentHost.xaml
	/// </summary>
	public partial class XamlOutlineContentHost : DockPanel, IOutlineContentHost
	{
		ITextEditor editor;
		Task updateTask;
		DispatcherTimer timer;
		
		public XamlOutlineContentHost(ITextEditor editor)
		{
			this.editor = editor;
			
			InitializeComponent();
			
			this.timer = new DispatcherTimer(DispatcherPriority.Background, this.Dispatcher);
			this.timer.Tick += new EventHandler(XamlOutlineContentHostTick);

			this.timer.Interval = new TimeSpan(0, 0, 2);
			this.timer.Start();
		}

		void XamlOutlineContentHostTick(object sender, EventArgs e)
		{
			if (updateTask != null && updateTask.Status == TaskStatus.Running)
				updateTask.Wait();
			updateTask = new Task(UpdateTask);
			updateTask.Start();
		}
		
		void UpdateTask()
		{
			string content = WorkbenchSingleton.SafeThreadFunction(() => editor.Document.Text);
			Stack<NodeWrapper> nodes = new Stack<NodeWrapper>();
			NodeWrapper root = null;
			
			using (XmlTextReader reader = new XmlTextReader(new StringReader(content))) {
				try {
					while (reader.Read()) {
						switch (reader.NodeType) {
							case XmlNodeType.Element:
								NodeWrapper node = new NodeWrapper() {
									ElementName = reader.LocalName,
									Line = reader.LineNumber,
									Column = reader.LinePosition,
									EndColumn = -1,
									EndLine = -1,
									Children = new List<NodeWrapper>()
								};
								if (reader.HasAttributes) {
									string name = reader.GetAttribute("Name");
									if (name == null)
										name = reader.GetAttribute("Name", CompletionDataHelper.XamlNamespace);
									if (name != null)
										node.Name = name;
								}
								if (root == null) {
									root = node;
									nodes.Push(root);
								} else {
									if (nodes.Count > 0)
										nodes.Peek().Children.Add(node);
									if (!reader.IsEmptyElement)
										nodes.Push(node);
								}
								break;
							case XmlNodeType.EndElement:
								if (nodes.Count > 1) {
									NodeWrapper n = nodes.Pop();
									n.EndLine = reader.LineNumber;
									n.EndColumn = reader.LinePosition;
								}
								break;
						}
					}
				} catch (XmlException) {
					return;
				}
				
				WorkbenchSingleton.SafeThreadCall(() => UpdateTree(root));
			}
		}
		
		void UpdateTree(NodeWrapper root)
		{
			if (this.treeView.Root == null)
				this.treeView.Root = BuildNode(root);
			else {
				UpdateNode(this.treeView.Root as XamlOutlineNode, root);
			}
		}
		
		void UpdateNode(XamlOutlineNode node, NodeWrapper dataNode)
		{
			if (dataNode != null && node != null) {
				node.Name = dataNode.Name;
				node.ElementName = dataNode.ElementName;
				node.Marker = editor.Document.CreateAnchor(editor.Document.PositionToOffset(dataNode.Line, dataNode.Column));
				
				ITextAnchor marker = null;
				
				if (dataNode.EndLine != -1 && dataNode.EndColumn != -1) {
					marker = editor.Document.CreateAnchor(editor.Document.PositionToOffset(dataNode.EndLine, dataNode.EndColumn));
				}
				
				node.EndMarker = marker;
				
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
			ITextAnchor marker = null;
			
			if (item.EndLine != -1 && item.EndColumn != -1) {
				marker = editor.Document.CreateAnchor(editor.Document.PositionToOffset(item.EndLine, item.EndColumn));
			}
			
			XamlOutlineNode node = new XamlOutlineNode() {
				Name = item.Name,
				ElementName = item.ElementName,
				ShowIcon = false,
				Marker = editor.Document.CreateAnchor(editor.Document.PositionToOffset(item.Line, item.Column)),
				EndMarker = marker,
				Editor = editor
			};
			
			foreach (var child in item.Children)
				node.Children.Add(BuildNode(child));
			
			return node;
		}
		
		void TreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			XamlOutlineNode node = treeView.SelectedItem as XamlOutlineNode;
			int offset = editor.Document.PositionToOffset(node.Marker.Line, node.Marker.Column);
			int endOffset = node.GetEndOffset();
			editor.Select(offset - 1, endOffset - offset);
		}
		
		public object OutlineContent {
			get {
				return this;
			}
		}
	}
	
	class NodeWrapper {
		public string ElementName { get; set; }
		public string Name { get; set; }
		
		public int Line { get; set; }
		public int Column { get; set; }
		
		public int EndLine { get; set; }
		public int EndColumn { get; set; }
		
		public IList<NodeWrapper> Children { get; set; }
	}
	
	class XamlOutlineNode : SharpTreeNode {
		string elementName, name;
		
		public string ElementName {
			get { return elementName; }
			set {
				this.elementName = value;
				this.RaisePropertyChanged("Text");
			}
		}
		
		public string Name {
			get { return name; }
			set {
				this.name = value;
				this.RaisePropertyChanged("Text");
			}
		}
		
		public ITextAnchor Marker { get; set; }
		public ITextAnchor EndMarker { get; set; }
		public ITextEditor Editor { get; set; }
		
		public XamlOutlineNode Successor {
			get {
				if (this.Parent == null)
					return null;
				int index = this.Parent.Children.IndexOf(this);
				if (index + 1 < this.Parent.Children.Count)
					return this.Parent.Children[index + 1] as XamlOutlineNode;
				else
					return null;
			}
		}
		
		public override bool CanDrag(SharpTreeNode[] nodes)
		{
			return nodes.All(node => node.Parent != null);
		}
		
		public override DropEffect CanDrop(IDataObject data, DropEffect requestedEffect)
		{
			return DropEffect.Move;
		}
		
		public override bool CanCopy(SharpTreeNode[] nodes)
		{
			return true;
		}
		
		public int GetEndOffset()
		{
			if (EndMarker != null) {
				return EndMarker.Offset + ElementName.Length + 2;
			} else {
				XamlOutlineNode successor = Successor;
				if (successor != null) {
					return successor.Marker.Offset;
				} else {
					XamlOutlineNode parent = Parent as XamlOutlineNode;
					if (parent != null)
						return parent.EndMarker.Offset - 1;
				}
			}
			
			return Editor.Document.TextLength + 1;
		}
		
		public string GetMarkupText()
		{
			int offset = Editor.Document.PositionToOffset(Marker.Line, Marker.Column);

			return Editor.Document.GetText(offset - 1, GetEndOffset() - offset);
		}
		
		public override IDataObject Copy(SharpTreeNode[] nodes)
		{
			string[] data = nodes
				.OfType<XamlOutlineNode>()
				.Select(item => item.GetMarkupText())
				.ToArray();
			var dataObject = new DataObject();
			dataObject.SetData(typeof(string[]), data);
			
			return dataObject;
		}
		
		public override bool CanDelete(SharpTreeNode[] nodes)
		{
			return nodes.All(node => node.Parent != null);
		}
		
		public override void Drop(IDataObject data, int index, DropEffect finalEffect)
		{
			try {
				string insertText = (data.GetData(typeof(string[])) as string[])
					.Aggregate((text, part) => text += part);
				ITextAnchor marker;
				int length = 0;
				if (index == this.Children.Count) {
					if (index == 0)
						marker = null;
					else
						marker = (this.Children[index - 1] as XamlOutlineNode).EndMarker;
					if (marker == null) {
						marker = this.EndMarker;
						length = -1; // move backwards
					} else {
						length = 2 + (this.Children[index - 1] as XamlOutlineNode).elementName.Length;
					}
				} else
					marker = (this.Children[index] as XamlOutlineNode).Marker;
				
				int offset = marker.Offset + length;
				Editor.Document.Insert(offset - 1, insertText);
			} catch (Exception ex) {
				throw ex;
			}
		}
		
		public override void Delete(SharpTreeNode[] nodes)
		{
			DeleteCore(nodes);
		}
		
		public override void DeleteCore(SharpTreeNode[] nodes)
		{
			foreach (XamlOutlineNode node in nodes.OfType<XamlOutlineNode>()) {
				node.Editor.Document.Remove(node.Marker.Offset - 1, node.GetEndOffset() - node.Marker.Offset);
			}
		}
		
		ContextMenu menu;

		public override ContextMenu GetContextMenu()
		{
			if (menu == null) {
				menu = new ContextMenu();
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut });
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy });
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste });
				menu.Items.Add(new Separator());
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Delete });
			}
			return menu;
		}
		
		public override object Text {
			get { return (!string.IsNullOrEmpty(Name) ? ElementName + " (" + Name + ")" : ElementName); }
		}
	}
}