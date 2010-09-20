// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
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
}
