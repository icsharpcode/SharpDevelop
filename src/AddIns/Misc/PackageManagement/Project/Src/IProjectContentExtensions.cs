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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public static class NRefactoryExtensionsForEnvDTE
	{
		public static string GetCodeModelLanguage(this IProject project)
		{
			if (project != null && project.Language == "VB") {
				return global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB;
			}
			return global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp;
		}
		
		public static global::EnvDTE.vsCMAccess ToAccess(this Accessibility accessiblity)
		{
			if (accessiblity == Accessibility.Public) {
				return global::EnvDTE.vsCMAccess.vsCMAccessPublic;
			}
			return global::EnvDTE.vsCMAccess.vsCMAccessPrivate;
		}
		
		public static Accessibility ToAccessibility(this global::EnvDTE.vsCMAccess access)
		{
			switch (access) {
				case global::EnvDTE.vsCMAccess.vsCMAccessPublic:
					return Accessibility.Public;
				case global::EnvDTE.vsCMAccess.vsCMAccessPrivate:
					return Accessibility.Private;
				default:
					throw new Exception("Invalid value for vsCMAccess");
			}
		}
	}
}
