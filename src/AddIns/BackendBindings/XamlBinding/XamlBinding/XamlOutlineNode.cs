// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.TreeView;

namespace ICSharpCode.XamlBinding
{
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
		
		public override bool CanDrag(SharpTreeNode[] nodes)
		{
			return false; //nodes.All(node => node.Parent != null);
		}
		
		public override DropEffect CanDrop(IDataObject data, DropEffect requestedEffect)
		{
			return  DropEffect.None; //DropEffect.Move;
		}
		
		public override bool CanCopy(SharpTreeNode[] nodes)
		{
			return true;
		}
		
		public string GetMarkupText()
		{
			return Editor.Document.GetText(Marker.Offset, EndMarker.Offset - Marker.Offset);
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
				Editor.Document.Insert(offset, insertText);
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
				node.Editor.Document.Remove(node.Marker.Offset, node.EndMarker.Offset - node.Marker.Offset);
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
