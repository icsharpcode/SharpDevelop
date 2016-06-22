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

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.PackageManagement
{
	public static class MessageServiceExtensions
	{
		public static void ShowNuGetConfigFileSaveError(string message)
		{
			MessageService.ShowError(
					String.Format("{0}{1}{1}{2}",
					message,
					Environment.NewLine,
					GetSaveNuGetConfigFileErrorMessage()));
		}
		
		/// <summary>
		/// Returns a non-Windows specific error message instead of the one NuGet returns.
		/// 
		/// NuGet returns a Windows specific error:
		/// 
		/// "DeleteSection" cannot be called on a NullSettings. This may be caused on account of 
		/// insufficient permissions to read or write to "%AppData%\NuGet\NuGet.config".
		/// </summary>
		static string GetSaveNuGetConfigFileErrorMessage()
		{
			string path = Path.Combine (
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"NuGet",
				"NuGet.config");
			return String.Format("Unable to read or write to \"{0}\".", path);
		}
	}
}
