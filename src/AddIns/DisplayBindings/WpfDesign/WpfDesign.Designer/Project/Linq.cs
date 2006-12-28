// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.Designer
{
	static class Linq
	{
		public static T[] ToArray<T>(ICollection<T> collection)
		{
			T[] arr = new T[collection.Count];
			collection.CopyTo(arr, 0);
			return arr;
		}
	}
}
