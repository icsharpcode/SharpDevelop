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
	}

	public interface IInlineUIElement
	{
		void Remove();
	}
}
