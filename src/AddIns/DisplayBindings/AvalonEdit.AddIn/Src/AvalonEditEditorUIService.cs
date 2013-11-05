// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.ContextActions;
using ICSharpCode.AvalonEdit.AddIn.ContextActions;

namespace ICSharpCode.AvalonEdit.AddIn
{
	sealed class AvalonEditEditorUIService : IEditorUIService
	{
		TextView textView;
		
		public AvalonEditEditorUIService(TextView textView)
		{
			this.textView = textView;
		}	
		
		/// <inheritdoc />
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
		
		/// <inheritdoc />
		public Point GetScreenPosition(int line, int column)
		{
			var positionInPixels = textView.PointToScreen(
				textView.GetVisualPosition(new TextViewPosition(line, column), VisualYPosition.LineBottom) - textView.ScrollOffset);
			return positionInPixels.TransformFromDevice(textView);
		}
		
		/// <inheritdoc />
		public void ShowContextActionsPopup(ContextActionsPopupViewModel viewModel)
		{
			new ContextActionsPopup { Actions = viewModel }.OpenAtCursorAndFocus();
		}
	}
}
