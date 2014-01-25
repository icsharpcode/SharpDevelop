// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
