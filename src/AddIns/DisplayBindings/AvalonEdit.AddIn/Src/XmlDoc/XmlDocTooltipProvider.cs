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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn.XmlDoc
{
	public class XmlDocTooltipProvider : ITextAreaToolTipProvider
	{
		public void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			if (e.ResolveResult == null)
				return;
			TypeResolveResult trr = e.ResolveResult as TypeResolveResult;
			MemberResolveResult mrr = e.ResolveResult as MemberResolveResult;
			LocalResolveResult lrr = e.ResolveResult as LocalResolveResult;
			if (trr != null && trr.Type.GetDefinition() != null) {
				e.SetToolTip(CreateTooltip(trr.Type));
			} else if (mrr != null) {
				e.SetToolTip(CreateTooltip(mrr.Member));
			} else if (lrr != null) {
				e.SetToolTip(new FlowDocumentTooltip(XmlDocFormatter.CreateTooltip(lrr.Variable)));
			}
		}
		
		sealed class FlowDocumentTooltip : Popup, ITooltip
		{
			FlowDocumentScrollViewer viewer;
			
			public FlowDocumentTooltip(FlowDocument document)
			{
				TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
				viewer = new FlowDocumentScrollViewer();
				viewer.Document = document;
				Border border = new Border {
					Background = SystemColors.InfoBrush,
					BorderBrush = SystemColors.InfoTextBrush,
					BorderThickness = new Thickness(1),
					MaxHeight = 400,
					Child = viewer
				};
				this.Child = border;
				viewer.Foreground = SystemColors.InfoTextBrush;
				document.FontSize = CodeEditorOptions.Instance.FontSize;
			}
			
			public bool CloseWhenMouseMovesAway {
				get { return !this.IsKeyboardFocusWithin; }
			}
			
			protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
			{
				base.OnLostKeyboardFocus(e);
				this.IsOpen = false;
			}
			
			protected override void OnMouseLeave(MouseEventArgs e)
			{
				base.OnMouseLeave(e);
				// When the mouse is over the popup, it is possible for SharpDevelop to be minimized,
				// or moved into the background, and yet the popup stays open.
				// We don't have a good method here to check whether the mouse moved back into the text area
				// or somewhere else, so we'll just close the popup.
				if (CloseWhenMouseMovesAway)
					this.IsOpen = false;
			}
		}
		
		object CreateTooltip(IType type)
		{
			return new FlowDocumentTooltip(XmlDocFormatter.CreateTooltip(type));
		}
		
		object CreateTooltip(IEntity entity)
		{
			return new FlowDocumentTooltip(XmlDocFormatter.CreateTooltip(entity));
		}
	}
}
