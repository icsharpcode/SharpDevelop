// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.AddIn.MyersDiff
{
	public static class Utils
	{
		public static void Set<T>(this IList<T> instance, int index, T value)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			
			if (index == instance.Count)
				instance.Add(value);
			else
				instance[index] = value;
		}
		
		public static OffsetChangeMap ToOffsetChangeMap(this IEnumerable<Edit> edits)
		{
			var map = new OffsetChangeMap();
			int diff = 0;
			foreach (var edit in edits) {
				Debug.Assert(edit.EditType != ChangeType.None && edit.EditType != ChangeType.Unsaved);
				int offset = edit.BeginA + diff;
				int removalLength = edit.EndA - edit.BeginA;
				int insertionLength = edit.EndB - edit.BeginB;
				
				diff += (insertionLength - removalLength);
				map.Add(new OffsetChangeMapEntry(offset, removalLength, insertionLength));
			}
			return map;
		}
	}
}
