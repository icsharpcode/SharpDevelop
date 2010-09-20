// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

#endregion

namespace ICSharpCode.Data.Core.Common
{
	/// <summary>
	/// Eigene Extension Methods für IEnumerable
	/// </summary>
	public static class Enumerable
	{
		#region ForEach

		/// <summary>
		/// Führt ein foreach auf IEnumerable T aus.
		/// WICHTIG: Hier gibt es kein break oder continue!
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
		{
			foreach (TSource item in source)
				action(item);
		}

		/// <summary>
		/// Führt ein foreach auf IEnumerable T  aus.
		/// WICHTIG: return false bei der Action löst ein break aus!
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> action)
		{
			foreach (TSource item in source)
				if (!action(item))
					break;
		}

		/// <summary>
		/// Creates an observable collection from an IEnumerable object.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static ObservableCollection<TSource> ToObservableCollection<TSource>(this IEnumerable<TSource> source)
		{
			ObservableCollection<TSource> dest = new ObservableCollection<TSource>();

			foreach (TSource item in source)
				dest.Add(item);

			return dest;
		}

		#endregion
	}
}
