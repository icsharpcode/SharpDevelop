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
