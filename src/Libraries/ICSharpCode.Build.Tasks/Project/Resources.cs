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
using System.Reflection;

namespace ICSharpCode.Build.Tasks
{
	internal static class Resources
	{		
		public static string RunningCodeAnalysis {
			get { return GetTranslation("${res:ICSharpCode.Build.RunningCodeAnalysis}")
					?? "Running Code Analysis..."; }
		}
		
		public static string CannotFindFxCop {
			get { return GetTranslation("${res:ICSharpCode.Build.CannotFindFxCop}")
					?? "SharpDevelop cannot find FxCop. Please specify the MSBuild property 'FxCopDir'."; }
		}
		
		public static string CannotReadFxCopLogFile {
			get { return GetTranslation("${res:ICSharpCode.Build.CannotReadFxCopLogFile}")
					?? "Cannot read FxCop log file:"; }
		}
		
		static bool initialized;
		static Type resourceService;
		
		static string GetTranslation(string key)
		{
			if (!initialized) {
				initialized = true;
				try {
					resourceService = Type.GetType("ICSharpCode.Core.StringParser, ICSharpCode.Core");
				} catch {}
			}
			if (resourceService != null) {
				const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
				string translation = (string)resourceService.InvokeMember("Parse", flags, null, null, new object[] { key });
				if (translation != null && translation.StartsWith("${res")) {
					return null;
				}
				return translation;
			} else {
				return null;
			}
		}
	}
}
