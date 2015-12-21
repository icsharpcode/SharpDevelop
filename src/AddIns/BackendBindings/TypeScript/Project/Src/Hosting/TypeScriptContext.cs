// 
// TypeScriptContext.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Noesis.Javascript;

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class TypeScriptContext : IDisposable
	{
		IJavaScriptContext context;
		LanguageServiceShimHost host;
		TypeScriptProject project;
		IScriptLoader scriptLoader;
		bool runInitialization = true;
		
		public TypeScriptContext(IJavaScriptContext context, IScriptLoader scriptLoader, ILogger logger)
		{
			this.context = context;
			this.scriptLoader = scriptLoader;
			host = new LanguageServiceShimHost(logger);
			host.AddDefaultLibScript(new FileName(scriptLoader.LibScriptFileName), scriptLoader.GetLibScript());
			
			if (context != null) {
				context.SetParameter("host", host);
				context.Run(scriptLoader.GetTypeScriptServicesScript());
			}
		}
		
		public void Dispose()
		{
			if (context != null) {
				context.Dispose();
			}
		}
		
		public void AddFile(FileName fileName, string text)
		{
			host.AddFile(fileName, text);
		}
		
		public void RunInitialisationScript()
		{
			if (runInitialization) {
				runInitialization = false;
				if (context != null) {
					context.Run(scriptLoader.GetMainScript());
				}
			}
		}
		
		public void GetCompletionItemsForTheFirstTime()
		{
			// HACK - run completion on first file so the user does not have to wait about 
			// 1-2 seconds for the completion list to appear the first time it is triggered.
			string fileName = host.GetFileNames().FirstOrDefault();
			if (fileName != null) {
				GetCompletionItems(new FileName(fileName), 1, String.Empty, false);
			}
		}
		
		public void UpdateFile(FileName fileName, string text)
		{
			host.UpdateFile(fileName, text);
		}
		
		public CompletionInfo GetCompletionItems(FileName fileName, int offset, string text, bool memberCompletion)
		{
			if (context == null)
				return new CompletionInfo();
			
			UpdateCompilerSettings();
			host.position = offset;
			host.UpdateFileName(fileName);
			host.isMemberCompletion = memberCompletion;
			
			context.Run(scriptLoader.GetMemberCompletionScript());
			
			return host.CompletionResult.result;
		}
		
		public CompletionEntryDetails GetCompletionEntryDetails(FileName fileName, int offset, string entryName)
		{
			if (context == null)
				return new CompletionEntryDetails();
			
			UpdateCompilerSettings();
			host.position = offset;
			host.UpdateFileName(fileName);
			host.completionEntry = entryName;
			
			context.Run(scriptLoader.GetCompletionDetailsScript());
			
			return host.CompletionEntryDetailsResult.result;
		}
		
		public SignatureHelpItems GetSignature(FileName fileName, int offset)
		{
			if (context == null)
				return new SignatureHelpItems();
			
			UpdateCompilerSettings();
			host.position = offset;
			host.UpdateFileName(fileName);
			
			context.Run(scriptLoader.GetFunctionSignatureScript());
			
			return host.SignatureResult.result;
		}
		
		public ReferenceEntry[] FindReferences(FileName fileName, int offset)
		{
			if (context == null)
				return new ReferenceEntry[0];
			
			UpdateCompilerSettings();
			host.position = offset;
			host.UpdateFileName(fileName);
			
			context.Run(scriptLoader.GetFindReferencesScript());
			
			return host.ReferencesResult.result;
		}
		
		public DefinitionInfo[] GetDefinition(FileName fileName, int offset)
		{
			if (context == null)
				return new DefinitionInfo[0];
			
			UpdateCompilerSettings();
			host.position = offset;
			host.UpdateFileName(fileName);
			
			context.Run(scriptLoader.GetDefinitionScript());
			
			return host.DefinitionResult.result;
		}
		
		public NavigationBarItem[] GetNavigationInfo(FileName fileName)
		{
			if (context == null)
				return new NavigationBarItem[0];
			
			UpdateCompilerSettings();
			host.UpdateFileName(fileName);
			context.Run(scriptLoader.GetNavigationScript());
			
			return host.NavigationResult.result;
		}
		
		public void RemoveFile(FileName fileName)
		{
			host.RemoveFile(fileName);
		}
		
		public EmitOutput Compile(FileName fileName, ITypeScriptOptions options)
		{
			if (context == null)
				return new EmitOutput();
			
			host.UpdateCompilerSettings(options);
			host.UpdateFileName(fileName);
			context.Run(scriptLoader.GetLanguageServicesCompileScript());
			
			return host.CompilerResult.result;
		}
		
		public Diagnostic[] GetDiagnostics(FileName fileName, ITypeScriptOptions options)
		{
			if (context == null)
				return new [] { GetMicrosoftRuntimeNotInstalledDiagnostic() };
			
			host.UpdateCompilerSettings(options);
			host.UpdateFileName(fileName);
			context.Run(scriptLoader.GetDiagnosticsScript());
			
			return host.SemanticDiagnosticsResult.result.Concat(
				host.SyntacticDiagnosticsResult.result)
				.ToArray();
		}
		
		public void AddFiles(IEnumerable<TypeScriptFile> files)
		{
			foreach (TypeScriptFile file in files) {
				AddFile(file.FileName, file.Text);
			}
		}
		
		public void UseProjectForOptions(TypeScriptProject project)
		{
			this.project = project;
		}

		void UpdateCompilerSettings()
		{
			if (project != null) {
				host.UpdateCompilerSettings(project);
			}
		}
		
		Diagnostic GetMicrosoftRuntimeNotInstalledDiagnostic()
		{
			return new Diagnostic {
				category = DiagnosticCategory.Error,
				message = "Microsoft Visual C++ 2010 Redistributable Package is not installed. https://www.microsoft.com/en-us/download/details.aspx?id=5555"
			};
		}
	}
}
