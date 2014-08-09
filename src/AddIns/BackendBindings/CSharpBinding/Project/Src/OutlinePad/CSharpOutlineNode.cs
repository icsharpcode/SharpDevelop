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
using System.Windows.Media;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.TreeView;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.Core.Presentation;
using System.Windows.Controls;

namespace CSharpBinding.OutlinePad
{
	class CSharpOutlineNode : SharpTreeNode
	{
		string name, iconName;
		
		public string Name {
			get { return name; }
			set {
				if (this.name != value) {
					this.name = value;
					this.RaisePropertyChanged("Text");
				}
			}
		}
				
		public override object ToolTip {
			get { return this.GetSourceText(); }
		}
		
		public ITextAnchor StartMarker { get; set; }
		public ITextAnchor EndMarker { get; set; }
		public ITextEditor Editor { get; set; }
		public AstNode AstNodeItem { get; set; }		
		public new CSharpOutlineNode Parent { get; set; }
		
		public string IconName {
			get { return iconName; }
			set {
				if (iconName != value) {
					iconName = value;
					this.RaisePropertyChanged("Icon");
				}
			}
		}
		
		public string GetSourceText() {
			if (StartMarker.IsDeleted || EndMarker.IsDeleted)
				return string.Empty;
			
			return Editor.Document.GetText(StartMarker.Offset, EndMarker.Offset - StartMarker.Offset);
		}
		
		public override bool CanDelete(SharpTreeNode[] nodes) {
			return nodes.OfType<CSharpOutlineNode>().All(n => n.Parent != null);
		}
		
		public override void Delete(SharpTreeNode[] nodes) {
			DeleteWithoutConfirmation(nodes);
		}
		
		public override void DeleteWithoutConfirmation(SharpTreeNode[] nodes) {
			foreach (CSharpOutlineNode CSharpNode in nodes.OfType<CSharpOutlineNode>()) {
				CSharpNode.DeleteCore();
			}
		}
		
		void DeleteCore()
		{
			Editor.Document.Remove(StartMarker.Offset, EndMarker.Offset - StartMarker.Offset);
		}
		
		public override object Text {
			get { return Name; }
		}
		
		public override object Icon {
			get { return !string.IsNullOrEmpty(this.IconName) 
						? SD.ResourceService.GetImageSource(this.IconName)
						: null; }
		}
		
		public override Brush Foreground {
			get { return foregroundBrush ?? SystemColors.WindowTextBrush; }
		}
		
		Brush foregroundBrush;
		public Brush ForegroundBrush {
			get {
				return foregroundBrush;
			}
			set {
				foregroundBrush = value;
				RaisePropertyChanged("Foreground");
			}
		}
		
		FontWeight weight = FontWeights.Normal;
		public FontWeight Weight {
			get {
				return weight;
			}
			set {
				if (weight != value) {
					weight = value;
					RaisePropertyChanged("FontWeight");
				}
			}
		}
		
		FontStyle style = FontStyles.Normal;
		public FontStyle Style {
			get {
				return style;
			}
			set {
				if (style != value) {
					style = value;
					RaisePropertyChanged("FontStyle");
				}
			}
		}
		
		public override FontWeight FontWeight {
			get { return Weight; }
		}
		
		public override FontStyle FontStyle {
			get { return Style; }
		}
		
		public override void ShowContextMenu(ContextMenuEventArgs e)
		{
			MenuService.ShowContextMenu(null, this, "/SharpDevelop/Pads/OutlinePad/ContextMenu/NodeActions");
		}
		
		protected override void OnExpanding() {
			var cmd = new HandleFoldingCommand();
			if (cmd.CanExecute(this))
				cmd.Execute(this);
		}
		
		protected override void OnCollapsing() {
			var cmd = new HandleFoldingCommand();
			if (cmd.CanExecute(this))
				cmd.Execute(this);
		}
	}
}
