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
