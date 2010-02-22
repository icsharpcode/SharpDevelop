// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Folding
{
	/// <summary>
	/// Base class for folding strategies.
	/// </summary>
	public abstract class AbstractFoldingStrategy
	{
		/// <summary>
		/// Create <see cref="NewFolding"/>s for the specified document and updates the folding manager with them.
		/// </summary>
		public void UpdateFoldings(FoldingManager manager, TextDocument document)
		{
			int firstErrorOffset;
			IEnumerable<NewFolding> foldings = CreateNewFoldings(document, out firstErrorOffset);
			manager.UpdateFoldings(foldings, firstErrorOffset);
		}
		
		/// <summary>
		/// Create <see cref="NewFolding"/>s for the specified document.
		/// </summary>
		public abstract IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset);
	}
}
