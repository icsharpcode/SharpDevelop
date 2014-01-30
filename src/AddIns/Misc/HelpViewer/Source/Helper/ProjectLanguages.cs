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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace MSHelpSystem.Helper
{
	internal sealed class ProjectLanguages
	{
		ProjectLanguages()
		{
		}

		#region Supported project languages

		static Dictionary<string, string> languages = InitializeLanguages();

		static Dictionary<string, string> InitializeLanguages()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			result.Add("C++", "C%2B%2B");
			result.Add("C#", "CSharp");
			result.Add("F#", "FSharp");
			result.Add("VB", "VB");
			return result;
		}

		#endregion

		public static string CurrentLanguage
		{
			get {
				string output = string.Empty;

				if (ProjectService.CurrentProject != null) {
					string devLang = ProjectService.CurrentProject.Language;
					if (string.IsNullOrEmpty(devLang)) {
						throw new ArgumentNullException("devLang");
					}
					if (!languages.ContainsKey(devLang) || !languages.TryGetValue(devLang, out output)) {
						output = devLang;
					}
					LoggingService.Debug(string.Format("HelpViewer: Project language \"{0}\" formatted to \"{1}\"", devLang, output));
				}
				return output.ToLower();
			}
		}

		public static string CurrentLanguageAsHttpParam
		{
			get {
				string devLang = CurrentLanguage;
				if (string.IsNullOrEmpty(devLang)) return string.Empty;
				else return string.Format("&category=DevLang%3a{0}", devLang);
			}
		}
	}
}
