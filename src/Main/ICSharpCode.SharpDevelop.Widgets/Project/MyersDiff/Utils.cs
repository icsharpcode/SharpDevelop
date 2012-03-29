// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.MyersDiff
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
	}
}
