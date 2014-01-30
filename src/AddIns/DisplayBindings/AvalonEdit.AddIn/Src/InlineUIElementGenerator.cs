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
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	sealed class InlineUIElementGenerator : VisualLineElementGenerator, IInlineUIElement
	{
		ITextAnchor anchor;
		UIElement element;
		TextView textView;
		
		public InlineUIElementGenerator(TextView textView, UIElement element, ITextAnchor anchor)
		{
			this.textView = textView;
			this.element = element;
			this.anchor = anchor;
			this.anchor.Deleted += delegate { Remove(); };
		}
		
		public override int GetFirstInterestedOffset(int startOffset)
		{
			if (anchor.Offset >= startOffset)
				return anchor.Offset;
			
			return -1;
		}
		
		public override VisualLineElement ConstructElement(int offset)
		{
			if (this.anchor.Offset == offset)
				return new InlineObjectElement(0, element);
			
			return null;
		}
		
		public void Remove()
		{
			this.textView.ElementGenerators.Remove(this);
		}
	}
	
	sealed class OverlayUIElementContainer : GroupBox, IOverlayUIElement
	{
		CodeEditor codeEditor;
		
		public OverlayUIElementContainer(CodeEditor codeEditor)
		{
			if (codeEditor == null)
				throw new ArgumentNullException("codeEditor");
			this.codeEditor = codeEditor;
			
			Background = SystemColors.WindowBrush;
			Foreground = SystemColors.WindowTextBrush;
			HorizontalAlignment = HorizontalAlignment.Right;
			VerticalAlignment = VerticalAlignment.Bottom;
			MaxWidth = 300;
			Margin = new Thickness(0, 0, 20, 20);
		}
		
		public void Remove()
		{
			codeEditor.Children.Remove(this);
		}

		public string Title {
			get { return Header.ToString(); }
			set { Header = value; }
		}
	}
}
