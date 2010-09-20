// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface IEditable
	{
		/// <summary>
		/// Creates a snapshot of the editor content.
		/// </summary>
		ITextBuffer CreateSnapshot();
		
		/// <summary>
		/// Gets the text in the view content.
		/// </summary>
		string Text {
			get;
		}
	}
}
