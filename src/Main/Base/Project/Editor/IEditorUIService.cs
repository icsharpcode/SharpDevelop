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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.ContextActions;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor
{
	[TextEditorService]
	public interface IEditorUIService
	{
		IInlineUIElement CreateInlineUIElement(ITextAnchor position, UIElement element);
		
		IOverlayUIElement CreateOverlayUIElement(UIElement element);
		
		/// <summary>
		/// Gets the absolute screen position of given position in the document.
		/// If the position is outside of the currently visible portion of the document,
		/// the value is forced into the viewport bounds.
		/// </summary>
		Point GetScreenPosition(int line, int column);
		
		/// <summary>
		/// Shows a ContextActionsPopup created from a ViewModel.
		/// </summary>
		void ShowContextActionsPopup(ContextActionsPopupViewModel viewModel);
	}

	public interface IInlineUIElement
	{
		void Remove();
	}
	
	public interface IOverlayUIElement
	{
		void Remove();
		string Title { get; set; }
	}
}
