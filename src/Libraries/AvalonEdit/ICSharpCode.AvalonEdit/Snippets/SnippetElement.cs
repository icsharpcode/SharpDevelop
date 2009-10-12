// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// An element inside a snippet.
	/// </summary>
	[Serializable]
	public abstract class SnippetElement : IFreezable
	{
		// TODO: think about removing IFreezable, it may not be required after all
		
		#region IFreezable infrastructure
		bool isFrozen;
		
		/// <summary>
		/// Gets if this instance is frozen. Frozen instances are immutable and thus thread-safe.
		/// </summary>
		public bool IsFrozen {
			get { return isFrozen; }
		}
		
		/// <summary>
		/// Freezes this instance.
		/// </summary>
		public void Freeze()
		{
			if (!isFrozen) {
				FreezeInternal();
				isFrozen = true;
			}
		}
		
		/// <summary>
		/// Override this method to freeze child elements.
		/// </summary>
		protected virtual void FreezeInternal()
		{
		}
		
		/// <summary>
		/// Throws an exception if this instance is frozen.
		/// </summary>
		protected void CheckBeforeMutation()
		{
			if (isFrozen)
				throw new InvalidOperationException("Cannot mutate frozen " + GetType().Name);
		}
		#endregion
		
		/// <summary>
		/// Performs insertion of the snippet.
		/// </summary>
		public abstract void Insert(InsertionContext context);
	}
}
