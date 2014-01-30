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

#endregion

namespace ICSharpCode.Data.Core.Common
{
	/// <summary>
	/// ExtensionMethods for System.Object.
	/// </summary>
	public static class Objects
	{
		#region Public methods

		#region DoIfNull

		/// <summary>
		/// Does something if the object isn't null.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		/// <returns>Source object</returns>
		public static TSource DoIfNull<TSource>(this TSource source, Action action)
		{
			Helper.CheckIfParameterIsNull(action, "action");

			if (source == null)
				action();
			return source;
		}

		#endregion

		#region DoIfNotNull

		/// <summary>
		/// Does something if the object isn't null, otherwise the method returns null.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static TTarget DoIfNotNull<TSource, TTarget>(this TSource source, Func<TSource, TTarget> action)
		{
			Helper.CheckIfParameterIsNull(action, "action");

			return source == null ? default(TTarget) : action(source);
		}

		/// <summary>
		/// Does something if the object isn't null.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void DoIfNotNull<TSource>(this TSource source, Action<TSource> action)
		{
			Helper.CheckIfParameterIsNull(action, "action");

			if (source != null)
				action(source);
		}

		#endregion

		#endregion
	}
}
