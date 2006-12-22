// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Util
{
	public class Lists
	{
		public static List<T> MergeLists<T>(T a, IEnumerable<T> b)
		{
			return MergeLists(new T[] {a}, b);
		}
		
		public static List<T> MergeLists<T>(IEnumerable<T> a, T b)
		{
			return MergeLists(a, new T[] {b});
		}
		
		public static List<T> MergeLists<T>(IEnumerable<T> a, IEnumerable<T> b)
		{
			List<T> newList = new List<T>();
			if (a != null) newList.AddRange(a);
			if (b != null) newList.AddRange(b);
			return newList;
		}
	}
}
