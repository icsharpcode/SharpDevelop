// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface IEditable
	{
		/// <summary>
		/// Creates a snapshot of the editor content.
		/// This method is thread-safe.
		/// </summary>
		ITextBuffer CreateSnapshot();
		
		/// <summary>
		/// Thread-safe access to the text in the view content.
		/// </summary>
		string Text {
			get;
		}
	}
}
