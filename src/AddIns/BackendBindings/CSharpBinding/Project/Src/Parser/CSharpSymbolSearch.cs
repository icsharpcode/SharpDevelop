// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.Core;
using ICSharpCode.Editor;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
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
		IList<FindReferences.SearchScope> searchScopes;
		IList<string>[] interestingFileNames;
		int workAmount;
		double workAmountInverse;
		Action<Reference> callback;
		FileName currentFileName;
		
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
						var allTypesInPC = TreeTraversal.PreOrder(ctx.GetTypes().Where(t => t.ProjectContent == pc), t => t.NestedTypes);
						interestingFileNames[i] = fr.GetInterestingFileNames(searchScopes[i], allTypesInPC, ctx);
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
			fr.ReferenceFound += OnReferenceFound;
			
			for (int i = 0; i < searchScopes.Count; i++) {
				FindReferences.SearchScope searchScope = searchScopes[i];
				foreach (string file in interestingFileNames[i]) {
					currentFileName = FileName.Create(file);
					FindReferencesInCurrentFile(args, searchScope);
					args.ProgressMonitor.Progress += workAmountInverse;
				}
			}
		}
		
		void OnReferenceFound(AstNode node, ResolveResult result)
		{
			callback(new Reference(new DomRegion(currentFileName, node.StartLocation, node.EndLocation), result));
		}
		
		void FindReferencesInCurrentFile(SymbolSearchArgs args, FindReferences.SearchScope searchScope)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(currentFileName);
			if (textSource == null)
				return;
			if (searchScope.SearchTerm != null) {
				// TODO: do a fast check with IndexOf()
			}
			
			ParseInformation parseInfo = ParserService.Parse(currentFileName, textSource);
			if (parseInfo == null)
				return;
			ParsedFile parsedFile = parseInfo.ParsedFile as ParsedFile;
			CompilationUnit cu = parseInfo.Annotation<CompilationUnit>();
			if (parsedFile == null || cu == null)
				return;
			fr.FindReferencesInFile(searchScope, parsedFile, cu, project.TypeResolveContext);
		}
	}
}
