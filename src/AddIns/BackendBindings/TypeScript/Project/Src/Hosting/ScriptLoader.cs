// 
// ScriptLoader.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class ScriptLoader : IScriptLoader
	{
		string root;
		string typeScriptServicesFileName;
		string mainScriptFileName;
		string memberCompletionScriptFileName;
		string typeScriptCompilerFileName;
		string signatureScriptFileName;
		string libScriptFileName;
		string findReferencesScriptFileName;
		string definitionScriptFileName;
		string navigationScriptFileName;
		string completionDetailsScriptFileName;
		string languageServicesCompileScriptFileName;
		string diagnosticsScriptFileName;
		
		public ScriptLoader()
		{
			root = Path.Combine(StringParser.Parse("${addinpath:ICSharpCode.TypeScriptBinding}"), "Scripts");
			root = Path.GetFullPath(root);
			
			typeScriptServicesFileName = GetFullPath("typescriptServices.js");
			mainScriptFileName = GetFullPath("main.js");
			
			memberCompletionScriptFileName = GetFullPath("completion.js");
			signatureScriptFileName = GetFullPath("signature.js");
			findReferencesScriptFileName = GetFullPath("references.js");
			definitionScriptFileName = GetFullPath("definition.js");
			navigationScriptFileName = GetFullPath("navigation.js");
			completionDetailsScriptFileName = GetFullPath("completionDetails.js");
			
			languageServicesCompileScriptFileName = GetFullPath("compile.js");
			diagnosticsScriptFileName = GetFullPath("diagnostics.js");
			
			typeScriptCompilerFileName = GetFullPath("tsc.js");
			libScriptFileName = GetFullPath("lib.d.ts");
		}
		
		public string RootFolder {
			get { return root; }
		}
		
		public string TypeScriptCompilerFileName {
			get { return typeScriptCompilerFileName; }
		}
		
		public string LibScriptFileName {
			get { return libScriptFileName; }
		}
		
		string GetFullPath(string fileName)
		{
			return Path.Combine(root, fileName);
		}
		
		public string GetTypeScriptServicesScript()
		{
			return ReadScript(typeScriptServicesFileName);
		}
		
		string ReadScript(string fileName)
		{
			return File.ReadAllText(fileName);
		}
		
		public string GetMainScript()
		{
			return ReadScript(mainScriptFileName);
		}
		
		public string GetMemberCompletionScript()
		{
			return ReadScript(memberCompletionScriptFileName);
		}
		
		public string GetTypeScriptCompilerScript()
		{
			return ReadScript(typeScriptCompilerFileName);
		}
		
		public string GetFunctionSignatureScript()
		{
			return ReadScript(signatureScriptFileName);
		}
		
		public string GetLibScript()
		{
			return ReadScript(libScriptFileName);
		}
		
		public string GetFindReferencesScript()
		{
			return ReadScript(findReferencesScriptFileName);
		}
		
		public string GetDefinitionScript()
		{
			return ReadScript(definitionScriptFileName);
		}
		
		public string GetNavigationScript()
		{
			return ReadScript(navigationScriptFileName);
		}
		
		public string GetCompletionDetailsScript()
		{
			return ReadScript(completionDetailsScriptFileName);
		}
		
		public string GetLanguageServicesCompileScript()
		{
			return ReadScript(languageServicesCompileScriptFileName);
		}
		
		public string GetDiagnosticsScript()
		{
			return ReadScript(diagnosticsScriptFileName);
		}
	}
}
