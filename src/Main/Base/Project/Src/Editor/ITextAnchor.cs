// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory;
using System;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Represents an anchored location inside an <see cref="IDocument"/>.
	/// </summary>
	public interface ITextAnchor
	{
		/// <summary>
		/// Gets the text location of this anchor.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
		Location Location { get; }
		
		/// <summary>
		/// Gets the offset of the text anchor.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
		int Offset { get; }
		
		/// <summary>
		/// Controls how the anchor moves.
		/// </summary>
		AnchorMovementType MovementType { get; set; }
		
		/// <summary>
		/// Specifies whether the anchor survives deletion of the text containing it.
		/// <c>false</c>: The anchor is deleted when the a selection that includes the anchor is deleted.
		/// <c>true</c>: The anchor is not deleted.
		/// </summary>
		bool SurviveDeletion { get; set; }
		
		/// <summary>
		/// Gets whether the anchor was deleted.
		/// </summary>
		bool IsDeleted { get; }
		
		/// <summary>
		/// Occurs after the anchor was deleted.
		/// </summary>
		event EventHandler Deleted;
		
		/// <summary>
		/// Gets the line number of the anchor.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
		int Line { get; }
		
		/// <summary>
		/// Gets the column number of this anchor.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
		int Column { get; }
	}
	
	/// <summary>
	/// Defines how a text anchor moves.
	/// </summary>
	public enum AnchorMovementType
	{
		/// <summary>
		/// When text is inserted at the anchor position, the type of the insertion
		/// determines where the caret moves to. For normal insertions, the anchor will stay
		/// behind the inserted text.
		/// </summary>
		Default = ICSharpCode.AvalonEdit.Document.AnchorMovementType.Default,
		/// <summary>
		/// Behaves like a start marker - when text is inserted at the anchor position, the anchor will stay
		/// before the inserted text.
		/// </summary>
		BeforeInsertion = ICSharpCode.AvalonEdit.Document.AnchorMovementType.BeforeInsertion,
		/// <summary>
		/// Behave like an end marker - when text is insered at the anchor position, the anchor will move
		/// after the inserted text.
		/// </summary>
		AfterInsertion = ICSharpCode.AvalonEdit.Document.AnchorMovementType.AfterInsertion
	}
}
