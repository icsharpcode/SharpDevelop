// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// This class "parses" a decompiled type to provide the information required
	/// by the ParserService.
	/// </summary>
	public class ILSpyParser : IParser
	{
		public bool CanParse(string fileName)
		{
			return fileName != null && fileName.StartsWith("ilspy://", StringComparison.OrdinalIgnoreCase);
		}
		
		readonly static ITextSource EmptyFileContent = new StringTextSource("");
		
		public ITextSource GetFileContent(FileName fileName)
		{
			return EmptyFileContent;
		}
		
		public ParseInformation Parse(FileName fileName, ITextSource fileContent, bool fullParseInformationRequested, IProject parentProject, CancellationToken cancellationToken)
		{
			return ILSpyDecompilerService.DecompileType(DecompiledTypeReference.FromFileName(fileName), cancellationToken);
		}
		
		public ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			var decompiledParseInfo = parseInfo as ILSpyFullParseInformation;
			if (decompiledParseInfo == null)
				throw new ArgumentException("ParseInfo does not have SyntaxTree");
			return ResolveAtLocation.Resolve(compilation, null, decompiledParseInfo.SyntaxTree, location, cancellationToken);
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
			DecompiledTypeReference reference = DecompiledTypeReference.FromFileName(fileName);
			if (reference != null) {
				var model = SD.GetService<IClassBrowser>().FindAssemblyModel(reference.AssemblyFile);
				if (model == null)
					model = SD.AssemblyParserService.GetAssemblyModelSafe(reference.AssemblyFile, true);
				if (model != null)
					return model.Context.GetCompilation();
			}
			return new CSharpProjectContent()
				.AddOrUpdateFiles(unresolvedFile)
				.CreateCompilation();
		}
		
		public IReadOnlyList<string> TaskListTokens { get; set; }
	}
}
