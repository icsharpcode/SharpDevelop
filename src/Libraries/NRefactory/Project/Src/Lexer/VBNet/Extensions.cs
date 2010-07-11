// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.NRefactory.Parser.VB
{
	public static class Extensions
	{
		public static bool IsElement<T>(this IEnumerable<T> items, Func<T, bool> check)
		{
			T item = items.FirstOrDefault();
			
			if (item != null)
				return check(item);
			return false;
		}
	}
}
