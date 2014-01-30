// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
