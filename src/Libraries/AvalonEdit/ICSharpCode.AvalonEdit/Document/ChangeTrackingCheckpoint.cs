// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// A checkpoint that allows tracking changes to a TextDocument.
	/// 
	/// Use <see cref="TextDocument.CreateSnapshot(out ChangeTrackingCheckpoint)"/> to create a checkpoint.
	/// </summary>
	public sealed class ChangeTrackingCheckpoint
	{
		readonly TextDocument document;
		// 'value' is the change from the previous checkpoint to this checkpoint
		readonly DocumentChangeEventArgs value;
		readonly int id;
		ChangeTrackingCheckpoint next;
		
		internal ChangeTrackingCheckpoint(TextDocument document)
		{
			this.document = document;
		}
		
		internal ChangeTrackingCheckpoint(TextDocument document, DocumentChangeEventArgs value, int id)
		{
			this.document = document;
			this.value = value;
			this.id = id;
		}
		
		internal ChangeTrackingCheckpoint Append(DocumentChangeEventArgs change)
		{
			Debug.Assert(this.next == null);
			this.next = new ChangeTrackingCheckpoint(this.document, change, unchecked( this.id + 1 ));
			return this.next;
		}
		
		/// <summary>
		/// Compares the age of this checkpoint to the other checkpoint.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
		/// <returns>-1 if this checkpoint is older than <paramref name="other"/>.
		/// 0 if <c>this</c>==<paramref name="other"/>.
		/// 1 if this checkpoint is newer than <paramref name="other"/>.</returns>
		public int CompareAge(ChangeTrackingCheckpoint other)
		{
			if (other == null)
				throw new ArgumentNullException("other");
			if (other.document != this.document)
				throw new ArgumentException("Checkpoints do not belong to the same document.");
			// We will allow overflows, but assume that the maximum distance between checkpoints is 2^31-1.
			// This is guaranteed on x86 because so many checkpoints don't fit into memory.
			return Math.Sign(unchecked( this.id - other.id ));
		}
		
		/// <summary>
		/// Gets the changes from this checkpoint to the other checkpoint.
		/// If 'other' is older than this checkpoint, reverse changes are calculated.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
		public IEnumerable<DocumentChangeEventArgs> GetChangesTo(ChangeTrackingCheckpoint other)
		{
			int result = CompareAge(other);
			if (result < 0)
				return GetForwardChanges(other);
			else if (result > 0)
				return other.GetForwardChanges(this).Reverse().Select(change => change.Invert());
			else
				return Empty<DocumentChangeEventArgs>.Array;
		}
		
		IEnumerable<DocumentChangeEventArgs> GetForwardChanges(ChangeTrackingCheckpoint other)
		{
			// Return changes from this(exclusive) to other(inclusive).
			ChangeTrackingCheckpoint node = this;
			do {
				node = node.next;
				yield return node.value;
			} while (node != other);
		}
	}
}
