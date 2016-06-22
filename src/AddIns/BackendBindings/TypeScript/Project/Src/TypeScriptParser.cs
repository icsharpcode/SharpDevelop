// 
// TypeScriptParser.cs
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
using System.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptParser : IParser
	{
		ITypeScriptContextFactory contextFactory;
		
		public TypeScriptParser(ILogger logger)
			: this(new TypeScriptContextFactory(new ScriptLoader(), logger))
		{
		}
		
		public TypeScriptParser(ITypeScriptContextFactory contextFactory)
		{
			this.contextFactory = contextFactory;
		}
		
		public IReadOnlyList<string> TaskListTokens {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CanParse(string fileName)
		{
			return true;
		}
		
		public ParseInformation Parse(
			FileName fileName,
			ITextSource fileContent,
			bool fullParseInformationRequested,
			IProject parentProject,
			CancellationToken cancellationToken)
		{
			return Parse(fileName, fileContent, new TypeScriptProject(parentProject), new TypeScriptFile[0]);
		}
		
		public ParseInformation Parse(
			FileName fileName,
			ITextSource fileContent,
			TypeScriptProject project,
			IEnumerable<TypeScriptFile> files)
		{
			try {
				using (TypeScriptContext context = contextFactory.CreateContext()) {
					context.AddFile(fileName, fileContent.Text);
					context.RunInitialisationScript();
					
					NavigationBarItem[] navigation = context.GetNavigationInfo(fileName);
					var unresolvedFile = new TypeScriptUnresolvedFile(fileName);
					unresolvedFile.AddNavigation(navigation, fileContent);
					
					if (project != null) {
						context.AddFiles(files);
						var document = new TextDocument(fileContent);
						Diagnostic[] diagnostics = context.GetDiagnostics(fileName, project.GetOptions());
						TypeScriptService.TaskService.Update(diagnostics, fileName);
					}
					
					return new ParseInformation(unresolvedFile, fileContent.Version, true);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				LoggingService.Debug(ex.ToString());
			}
			
			return new ParseInformation(
				new TypeScriptUnresolvedFile(fileName),
				fileContent.Version,
				true);
		}
		
		public static bool IsTypeScriptFileName(FileName fileName)
		{
			if (fileName == null)
				return false;
			
			return String.Equals(".ts", fileName.GetExtension(), StringComparison.OrdinalIgnoreCase);
		}
		
		public ITextSource GetFileContent(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		public ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public ICodeContext ResolveContext(ParseInformation parseInfo, ICSharpCode.NRefactory.TextLocation location, ICSharpCode.NRefactory.TypeSystem.ICompilation compilation, System.Threading.CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public ResolveResult ResolveSnippet(ParseInformation parseInfo, TextLocation location, string codeSnippet, ICompilation compilation, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public void FindLocalReferences(ParseInformation parseInfo, ITextSource fileContent, IVariable variable, ICompilation compilation, Action<SearchResultMatch> callback, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public ICompilation CreateCompilationForSingleFile(FileName fileName, IUnresolvedFile unresolvedFile)
		{
			throw new NotImplementedException();
		}
	}
}
