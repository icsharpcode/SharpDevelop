// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// <see cref="IReadOnlySectionProvider"/> that has no read-only sections; all text is editable.
	/// </summary>
	sealed class NoReadOnlySections : IReadOnlySectionProvider
	{
		public static readonly NoReadOnlySections Instance = new NoReadOnlySections();
		
		public bool CanInsert(int offset)
		{
			return true;
		}
		
		public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");
			// the segment is always deletable
			return ExtensionMethods.Sequence(segment);
		}
	}
}
