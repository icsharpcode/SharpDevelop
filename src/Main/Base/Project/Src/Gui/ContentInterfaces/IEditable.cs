// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	[ViewContentService]
	public interface IEditable
	{
		/// <summary>
		/// Creates a snapshot of the editor content.
		/// </summary>
		ITextSource CreateSnapshot();
		
		/// <summary>
		/// Gets the text in the view content.
		/// </summary>
		string Text {
			get;
		}
	}
}
