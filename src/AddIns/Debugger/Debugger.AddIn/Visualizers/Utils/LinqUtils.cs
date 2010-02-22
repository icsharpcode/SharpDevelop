// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Description of LinqUtils.
	/// </summary>
	public static class LinqUtils
	{
		public static int MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
		{
			if (source.Count() == 0)
				return defaultValue;
			
			return source.Max(selector);
		}
	}
}
