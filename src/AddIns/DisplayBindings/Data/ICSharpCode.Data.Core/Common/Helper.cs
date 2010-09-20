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
	/// Helper class for the ExtensionMethods.
	/// </summary>
	internal static class Helper
	{
		/// <summary>
		/// Checks if a parameter is null and throws an exception if that's true.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="parameterName"></param>
		internal static void CheckIfParameterIsNull(object parameter, string parameterName)
		{
			if (parameter == null)
			{
				throw new Exception(string.Format("Der Parameter '{0}' darf nicht NULL sein.", parameterName));
			}
		}

		/// <summary>
		/// Trys to cast TSource to TTarget and throws an exception if that's not possible.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <param name="source"></param>
		/// <param name="castLogic"></param>
		internal static TTarget TryCast<TSource, TTarget>(TSource source, Func<TSource, TTarget> castLogic)
		{
			try
			{
				return castLogic(source);
			}
			catch (InvalidCastException)
			{
				throw new Exception(string.Format("Der SourceType '{0}' kann nicht auf den TargetType '{1}' gecastet werden.", typeof(TSource).ToString(), typeof(TTarget).ToString()));
			}
		}
	}
}
