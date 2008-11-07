// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Determines whether the document can be modified.
	/// </summary>
	public interface IReadOnlySectionProvider
	{
		/// <summary>
		/// Gets whether insertion is possible at the specified offset.
		/// </summary>
		bool CanInsert(int offset);
		
		/// <summary>
		/// Gets the deletable segments inside the given segment.
		/// </summary>
		IEnumerable<ISegment> GetDeletableSegments(ISegment segment);
	}
}
