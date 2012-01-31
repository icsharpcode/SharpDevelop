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
	/// Language-independent finds references implementation.
	/// This call will call into the individual language bindings to perform the job.
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
		
		/// <summary>
		/// Finds references to a local variable.
		/// </summary>
		public static void FindReferences(IVariable variable, Action<Reference> callback)
		{
			if (variable == null)
				throw new ArgumentNullException("variable");
			if (callback == null)
				throw new ArgumentNullException("callback");
			var fileName = FileName.Create(variable.Region.FileName);
			IParser parser = ParserService.GetParser(fileName);
			ParseInformation pi = ParserService.Parse(fileName);
			if (pi == null || parser == null)
				return;
			var compilation = ParserService.GetCompilationForFile(fileName);
			parser.FindLocalReferences(pi, variable, compilation, callback, CancellationToken.None);
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
			
			var solutionSnapshot = GetSolutionSnapshot(baseType.Compilation);
			foreach (IProject project in GetProjectsThatCouldReferenceEntity(baseType)) {
				var compilation = solutionSnapshot.GetCompilation(project);
				var importedBaseType = compilation.Import(baseType);
				if (importedBaseType == null)
					continue;
				foreach (ITypeDefinition typeDef in compilation.MainAssembly.GetAllTypeDefinitions()) {
					bool isDerived;
					if (directDerivationOnly) {
						isDerived = typeDef.DirectBaseTypes.Select(t => t.GetDefinition()).Contains(importedBaseType);
					} else {
						isDerived = typeDef.IsDerivedFrom(importedBaseType);
					}
					if (isDerived)
						results.Add(typeDef);
				}
			}
			return results;
		}
		
		static SharpDevelopSolutionSnapshot GetSolutionSnapshot(ICompilation compilation)
		{
			var snapshot = compilation.SolutionSnapshot as SharpDevelopSolutionSnapshot;
			return snapshot ?? ParserService.GetCurrentSolutionSnapshot();
		}
		
		
		/// <summary>
		/// Builds a graph of derived type definitions.
		/// </summary>
		public static TypeGraphNode BuildDerivedTypesGraph(ITypeDefinition baseType)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			var solutionSnapshot = GetSolutionSnapshot(baseType.Compilation);
			var compilations = GetProjectsThatCouldReferenceEntity(baseType).Select(p => solutionSnapshot.GetCompilation(p));
			var graph = BuildTypeInheritanceGraph(compilations);
			TypeGraphNode node;
			if (graph.TryGetValue(new TypeName(baseType), out node)) {
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
		static Dictionary<TypeName, TypeGraphNode> BuildTypeInheritanceGraph(IEnumerable<ICompilation> compilations)
		{
			if (compilations == null)
				throw new ArgumentNullException("projectContents");
			Dictionary<TypeName, TypeGraphNode> dict = new Dictionary<TypeName, TypeGraphNode>();
			foreach (ICompilation compilation in compilations) {
				foreach (ITypeDefinition typeDef in compilation.MainAssembly.GetAllTypeDefinitions()) {
					dict.Add(new TypeName(typeDef), new TypeGraphNode(typeDef));
				}
			}
			foreach (ICompilation compilation in compilations) {
				foreach (ITypeDefinition typeDef in compilation.MainAssembly.GetAllTypeDefinitions()) {
					TypeGraphNode typeNode = dict[new TypeName(typeDef)];
					foreach (IType baseType in typeDef.DirectBaseTypes) {
						ITypeDefinition baseTypeDef = baseType.GetDefinition();
						if (baseTypeDef != null) {
							TypeGraphNode baseTypeNode;
							if (dict.TryGetValue(new TypeName(baseTypeDef), out baseTypeNode)) {
								typeNode.BaseTypes.Add(baseTypeNode);
								baseTypeNode.DerivedTypes.Add(typeNode);
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
