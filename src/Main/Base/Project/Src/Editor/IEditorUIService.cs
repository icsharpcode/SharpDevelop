// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
