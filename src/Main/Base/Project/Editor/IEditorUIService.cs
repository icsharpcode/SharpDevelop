// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		/// <summary>
		/// Gets the absolute screen position of given position in the document.
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
}
