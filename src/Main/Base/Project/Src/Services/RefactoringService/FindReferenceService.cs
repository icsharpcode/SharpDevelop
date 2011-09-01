// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of FindReferenceService.
	/// </summary>
	public static class FindReferenceService
	{
		#region FindReferences
		static IEnumerable<IProject> GetProjectsThatCouldReferenceEntity(IEntity entity)
		{
			Solution solution = ProjectService.OpenSolution;
			if (solution == null)
				yield break;
			foreach (IProject project in solution.Projects) {
				IProjectContent pc = project.ProjectContent;
				if (pc == null)
					continue;
				yield return project;
			}
		}
		
		static List<ISymbolSearch> PrepareSymbolSearch(IEntity entity, CancellationToken cancellationToken, out double totalWorkAmount)
		{
			totalWorkAmount = 0;
			List<ISymbolSearch> symbolSearches = new List<ISymbolSearch>();
			foreach (IProject project in GetProjectsThatCouldReferenceEntity(entity)) {
				cancellationToken.ThrowIfCancellationRequested();
				ISymbolSearch symbolSearch = project.PrepareSymbolSearch(entity);
				if (symbolSearch != null) {
					symbolSearches.Add(symbolSearch);
					totalWorkAmount += symbolSearch.WorkAmount;
				}
			}
			if (totalWorkAmount < 1)
				totalWorkAmount = 1;
			return symbolSearches;
		}
		
		/// <summary>
		/// Finds all references to the specified entity.
		/// The results are reported using the callback.
		/// FindReferences may internally use parallelism, and may invoke the callback on multiple
		/// threads in parallel.
		/// </summary>
		public static void FindReferences(IEntity entity, IProgressMonitor progressMonitor, Action<Reference> callback)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				progressMonitor.ShowingDialog = true;
				MessageService.ShowMessage("${res:SharpDevelop.Refactoring.LoadSolutionProjectsThreadRunning}");
				progressMonitor.ShowingDialog = false;
				return;
			}
			double totalWorkAmount;
			List<ISymbolSearch> symbolSearches = PrepareSymbolSearch(entity, progressMonitor.CancellationToken, out totalWorkAmount);
			double workDone = 0;
			ParseableFileContentFinder parseableFileContentFinder = new ParseableFileContentFinder();
			foreach (ISymbolSearch s in symbolSearches) {
				progressMonitor.CancellationToken.ThrowIfCancellationRequested();
				using (var childProgressMonitor = progressMonitor.CreateSubTask(s.WorkAmount / totalWorkAmount)) {
					s.FindReferences(new SymbolSearchArgs(childProgressMonitor, parseableFileContentFinder), callback);
				}
				
				workDone += s.WorkAmount;
				progressMonitor.Progress = workDone / totalWorkAmount;
			}
		}
		#endregion
		
		#region FindDerivedTypes
		/// <summary>
		/// Finds all types that are derived from the given base type.
		/// </summary>
		public static IList<ITypeDefinition> FindDerivedTypes(ITypeDefinition baseType, bool directDerivationOnly)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			baseType = baseType.GetDefinition(); // ensure we use the compound class
			
			List<ITypeDefinition> results = new List<ITypeDefinition>();
			
			foreach (IProject project in GetProjectsThatCouldReferenceEntity(baseType)) {
				using (var ctx = project.TypeResolveContext.Synchronize()) {
					foreach (ITypeDefinition typeDef in project.ProjectContent.GetAllTypes()) {
						bool isDerived;
						if (directDerivationOnly) {
							isDerived = false;
							foreach (IType t in typeDef.GetBaseTypes(ctx)) {
								ITypeDefinition tdef = baseType.GetDefinition();
								if (tdef == baseType) {
									isDerived = true;
									break;
								}
							}
						} else {
							isDerived = typeDef.IsDerivedFrom(baseType, ctx);
						}
						if (isDerived)
							results.Add(typeDef);
					}
				}
			}
			return results;
		}
		
		
		/// <summary>
		/// Builds a graph of derived type definitions.
		/// </summary>
		public static TypeGraphNode BuildDerivedTypesGraph(ITypeDefinition baseType)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			IEnumerable<IProjectContent> projectContents = GetProjectsThatCouldReferenceEntity(baseType).Select(p => p.ProjectContent);
			projectContents = projectContents.Union(new [] { baseType.ProjectContent });
			var graph = BuildTypeInheritanceGraph(projectContents);
			TypeGraphNode node;
			if (graph.TryGetValue(baseType, out node)) {
				node.BaseTypes.Clear(); // only derived types were requested, so don't return the base types
				return node;
			} else {
				return new TypeGraphNode(baseType);
			}
		}
		
		/// <summary>
		/// Builds a graph of all type definitions in the specified set of project contents.
		/// </summary>
		/// <remarks>The resulting graph may be cyclic if there are cyclic type definitions.</remarks>
		public static Dictionary<ITypeDefinition, TypeGraphNode> BuildTypeInheritanceGraph(IEnumerable<IProjectContent> projectContents)
		{
			if (projectContents == null)
				throw new ArgumentNullException("projectContents");
			Dictionary<ITypeDefinition, TypeGraphNode> dict = new Dictionary<ITypeDefinition, TypeGraphNode>();
			using (var allContexts = new CompositeTypeResolveContext(projectContents).Synchronize()) { // lock all project contents
				foreach (ITypeDefinition typeDef in allContexts.GetAllTypes()) {
					dict.Add(typeDef, new TypeGraphNode(typeDef));
				}
				foreach (IProjectContent pc in projectContents) {
					using (var context = ParserService.GetTypeResolveContext(pc).Synchronize()) {
						foreach (ITypeDefinition typeDef in pc.GetAllTypes()) {
							TypeGraphNode typeNode = dict[typeDef];
							foreach (IType baseType in typeDef.GetBaseTypes(context)) {
								ITypeDefinition baseTypeDef = baseType.GetDefinition();
								if (baseTypeDef != null) {
									TypeGraphNode baseTypeNode;
									if (dict.TryGetValue(baseTypeDef, out baseTypeNode)) {
										typeNode.BaseTypes.Add(baseTypeNode);
										baseTypeNode.DerivedTypes.Add(typeNode);
									}
								}
							}
						}
					}
				}
			}
			return dict;
		}
		#endregion
	}
	
	public class SymbolSearchArgs
	{
		public IProgressMonitor ProgressMonitor { get; private set; }
		
		public CancellationToken CancellationToken {
			get { return this.ProgressMonitor.CancellationToken; }
		}
		
		public ParseableFileContentFinder ParseableFileContentFinder { get; private set; }
		
		public SymbolSearchArgs(IProgressMonitor progressMonitor, ParseableFileContentFinder parseableFileContentFinder)
		{
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			if (parseableFileContentFinder == null)
				throw new ArgumentNullException("parseableFileContentFinder");
			this.ProgressMonitor = progressMonitor;
			this.ParseableFileContentFinder = parseableFileContentFinder;
		}
	}
	
	public interface ISymbolSearch
	{
		double WorkAmount { get; }
		
		void FindReferences(SymbolSearchArgs searchArguments, Action<Reference> callback);
	}
}
