// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

namespace ICSharpCode.TextEditor.Undo
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
