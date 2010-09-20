// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// Description of ExtensionMethods.
	/// </summary>
	public static class ExtensionMethods
	{
		public static void AddRange<T>(this IList<T> coll, IEnumerable<T> items)
		{
			foreach (T item in items)
				coll.Add(item);
		}
		
		public static void AddRange(this IList coll, IEnumerable items)
		{
			foreach (object item in items)
				coll.Add(item);
		}
	}
}
