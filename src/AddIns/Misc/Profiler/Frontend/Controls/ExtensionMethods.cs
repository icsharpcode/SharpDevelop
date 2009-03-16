// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
