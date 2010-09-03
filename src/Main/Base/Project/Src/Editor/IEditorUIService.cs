// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Editor
{
	public interface IEditorUIService
	{
		IInlineUIElement CreateInlineUIElement(ITextAnchor position, UIElement element);
		/// <summary>
		/// Gets the absolute screen position of given position in the document.
		/// </summary>
		Point GetScreenPosition(int line, int column);
	}

	public interface IInlineUIElement
	{
		void Remove();
	}
}
