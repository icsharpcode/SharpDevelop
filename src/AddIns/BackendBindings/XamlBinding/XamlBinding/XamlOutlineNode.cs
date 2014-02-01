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
using System.Windows;

using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.TreeView;

namespace ICSharpCode.XamlBinding
{
	class XamlOutlineNode : SharpTreeNode
	{
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
		
		public string GetMarkupText()
		{
			return Editor.Document.GetText(Marker.Offset, EndMarker.Offset - Marker.Offset);
		}
		
		protected override IDataObject GetDataObject(SharpTreeNode[] nodes)
		{
			string[] data = nodes
				.OfType<XamlOutlineNode>()
				.Select(item => item.GetMarkupText())
				.ToArray();
			var dataObject = new DataObject();
			dataObject.SetData(typeof(string[]), data);
			
			return dataObject;
		}
		
//		public override void Drop(IDataObject data, int index, DropEffect finalEffect)
//		{
//			try {
//				string insertText = (data.GetData(typeof(string[])) as string[])
//					.Aggregate((text, part) => text += part);
//				ITextAnchor marker;
//				int length = 0;
//				if (index == this.Children.Count) {
//					if (index == 0)
//						marker = null;
//					else
//						marker = (this.Children[index - 1] as XamlOutlineNode).EndMarker;
//					if (marker == null) {
//						marker = this.EndMarker;
//						length = -1; // move backwards
//					} else {
//						length = 2 + (this.Children[index - 1] as XamlOutlineNode).elementName.Length;
//					}
//				} else
//					marker = (this.Children[index] as XamlOutlineNode).Marker;
//
//				int offset = marker.Offset + length;
//				Editor.Document.Insert(offset, insertText);
//			} catch (Exception ex) {
//				throw ex;
//			}
//		}
		
		public override bool CanDelete(SharpTreeNode[] nodes)
		{
			return nodes.OfType<XamlOutlineNode>().All(n => n.Parent != null);
		}
		
		public override void Delete(SharpTreeNode[] nodes)
		{
			DeleteWithoutConfirmation(nodes);
		}
		
		public override void DeleteWithoutConfirmation(SharpTreeNode[] nodes)
		{
			foreach (XamlOutlineNode xamlNode in nodes.OfType<XamlOutlineNode>()) {
				xamlNode.DeleteCore();
			}
		}
		
		void DeleteCore()
		{
			Editor.Document.Remove(Marker.Offset, EndMarker.Offset - Marker.Offset);
		}
		
		public override object Text {
			get { return (!string.IsNullOrEmpty(Name) ? ElementName + " (" + Name + ")" : ElementName); }
		}
		
		public override object Icon {
			get { return SD.ResourceService.GetImageSource("Icons.16x16.HtmlElements.Element"); }
		}
	}
}
