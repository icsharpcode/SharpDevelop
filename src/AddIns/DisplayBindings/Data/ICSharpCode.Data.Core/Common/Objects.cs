// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
