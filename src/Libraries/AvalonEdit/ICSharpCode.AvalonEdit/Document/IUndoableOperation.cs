// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// This Interface describes a the basic Undo/Redo operation
	/// all Undo Operations must implement this interface.
	/// </summary>
	public interface IUndoableOperation
	{
		/// <summary>
		/// Undo the last operation
		/// </summary>
		void Undo();
		
		/// <summary>
		/// Redo the last operation
		/// </summary>
		void Redo();
	}
}
