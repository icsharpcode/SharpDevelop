// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	sealed class AvalonEditEditorUIService : IEditorUIService
	{
		TextView textView;
		
		public AvalonEditEditorUIService(TextView textView)
		{
			this.textView = textView;
		}	
		
		public IInlineUIElement CreateInlineUIElement(ITextAnchor position, UIElement element)
		{
			if (position == null)
				throw new ArgumentNullException("position");
			if (element == null)
				throw new ArgumentNullException("element");
			InlineUIElementGenerator inline = new InlineUIElementGenerator(textView, element, position);
			this.textView.ElementGenerators.Add(inline);
			return inline;
		}
	}
}
