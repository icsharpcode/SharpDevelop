// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Provides immutable empty list instances.
	/// </summary>
	static class Empty<T>
	{
		public static readonly T[] Array = new T[0];
		//public static readonly ReadOnlyCollection<T> ReadOnlyCollection = new ReadOnlyCollection<T>(Array);
	}
}
