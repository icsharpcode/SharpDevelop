// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Represents an active element that allows the snippet to stay interactive after insertion.
	/// </summary>
	public interface IActiveElement
	{
		/// <summary>
		/// Called when the all snippet elements have been inserted.
		/// </summary>
		void OnInsertionCompleted();
		
		/// <summary>
		/// Called when the interactive mode is deactivated.
		/// </summary>
		void Deactivate();
	}
}
