// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding
{
	/// <summary>
	/// C# backend implementation for 'find references'.
	/// </summary>
	public class CSharpSymbolSearch : ISymbolSearch
	{
		IProject project;
		FindReferences fr = new FindReferences();
		IList<IFindReferenceSearchScope> searchScopes;
		IList<string>[] interestingFileNames;
		int workAmount;
		double workAmountInverse;
		Action<Reference> callback;
		
		public CSharpSymbolSearch(IProject project, IEntity entity)
		{
			this.project = project;
			searchScopes = fr.GetSearchScopes(entity);
			interestingFileNames = new IList<string>[searchScopes.Count];
			using (var ctx = project.TypeResolveContext.Synchronize()) {
				IProjectContent pc = project.ProjectContent;
				for (int i = 0; i < searchScopes.Count; i++) {
					// Check whether this project can reference the entity at all
					bool canReferenceEntity;
					switch (searchScopes[i].Accessibility) {
						case Accessibility.None:
						case Accessibility.Private:
							canReferenceEntity = (pc == entity.ProjectContent);
							break;
						case Accessibility.Internal:
						case Accessibility.ProtectedAndInternal:
							canReferenceEntity = entity.ProjectContent.InternalsVisibleTo(pc, ctx);
							break;
						default:
							ITypeDefinition typeDef = searchScopes[i].TopLevelTypeDefinition;
							if (typeDef != null) {
								ITypeDefinition typeDefInContext = ctx.GetTypeDefinition(typeDef.Namespace, typeDef.Name, typeDef.TypeParameterCount, StringComparer.Ordinal);
								canReferenceEntity = (typeDefInContext == typeDef.GetDefinition());
							} else {
								canReferenceEntity = true;
							}
							break;
					}
					
					if (canReferenceEntity) {
						interestingFileNames[i] = fr.GetInterestingFileNames(searchScopes[i], pc.GetAllTypes(), ctx);
						workAmount += interestingFileNames[i].Count;
					} else {
						interestingFileNames[i] = new string[0];
					}
				}
			}
			workAmountInverse = 1.0 / workAmount;
		}
		
		public double WorkAmount {
			get { return workAmount; }
		}
		
		public void FindReferences(SymbolSearchArgs args, Action<Reference> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (this.callback != null)
				throw new InvalidOperationException("Cannot call FindReferences() twice");
			this.callback = callback;
			
			for (int i = 0; i < searchScopes.Count; i++) {
				IFindReferenceSearchScope searchScope = searchScopes[i];
				object progressLock = new object();
				Parallel.ForEach(
					interestingFileNames[i],
					new ParallelOptions {
						MaxDegreeOfParallelism = Environment.ProcessorCount
					},
					delegate (string file) {
						FindReferencesInFile(args, searchScope, FileName.Create(file));
						lock (progressLock)
							args.ProgressMonitor.Progress += workAmountInverse;
					});
			}
		}
		
		void FindReferencesInFile(SymbolSearchArgs args, IFindReferenceSearchScope searchScope, FileName fileName)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			if (searchScope.SearchTerm != null) {
				if (textSource.IndexOf(searchScope.SearchTerm, 0, textSource.TextLength, StringComparison.Ordinal) < 0)
					return;
			}
			
			ParseInformation parseInfo = ParserService.Parse(fileName, textSource);
			if (parseInfo == null)
				return;
			CSharpParsedFile parsedFile = parseInfo.ParsedFile as CSharpParsedFile;
			CompilationUnit cu = parseInfo.Annotation<CompilationUnit>();
			if (parsedFile == null || cu == null)
				return;
			fr.FindReferencesInFile(
				searchScope, parsedFile, cu, project.TypeResolveContext,
				delegate (AstNode node, ResolveResult result) {
					var region = new DomRegion(fileName, node.StartLocation, node.EndLocation);
					callback(new Reference(region, result));
				});
		}
	}
}
