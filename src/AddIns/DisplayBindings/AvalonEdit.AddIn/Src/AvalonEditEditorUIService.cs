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
		public IOverlayUIElement CreateOverlayUIElement(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			CodeEditor codeEditor = textView.GetService<CodeEditor>();
			if (codeEditor == null)
				throw new NotSupportedException("This feature is not supported!");
			var groupBox = new OverlayUIElementContainer(codeEditor);
			groupBox.Content = element;
			codeEditor.Children.Add(groupBox);
			System.Windows.Controls.Grid.SetRow(groupBox, 1);
			return groupBox;
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
