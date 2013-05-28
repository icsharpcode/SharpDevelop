// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop.Editor.Search;
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
//		#region FindReferences
		static IEnumerable<IProject> GetProjectsThatCouldReferenceEntity(IEntity entity)
		{
			ISolution solution = ProjectService.OpenSolution;
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
		public static async Task FindReferencesAsync(IEntity entity, IProgressMonitor progressMonitor, Action<SearchedFile> callback)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			if (callback == null)
				throw new ArgumentNullException("callback");
			SD.MainThread.VerifyAccess();
			if (SD.ParserService.LoadSolutionProjectsThread.IsRunning) {
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
					await s.FindReferencesAsync(new SymbolSearchArgs(childProgressMonitor, parseableFileContentFinder), callback);
				}
				
				workDone += s.WorkAmount;
				progressMonitor.Progress = workDone / totalWorkAmount;
			}
		}
		
		public static IObservable<SearchedFile> FindReferences(IEntity entity, IProgressMonitor progressMonitor)
		{
			return ReactiveExtensions.CreateObservable<SearchedFile>(
				(monitor, callback) => FindReferencesAsync(entity, monitor, callback),
				progressMonitor);
		}
		
		public static Task FindReferencesToHierarchyAsync(IMember member, IProgressMonitor progressMonitor, Action<SearchedFile> callback)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			if (callback == null)
				throw new ArgumentNullException("callback");
			return Task.WhenAll(FindAllMembers(member).Distinct().Select(m => FindReferencesAsync(m, progressMonitor, callback)));
		}
		
		public static IEnumerable<IMember> FindAllMembers(IMember member)
		{
			yield return member;
			foreach (var ancestor in InheritanceHelper.GetBaseMembers(member, true)) {
				yield return ancestor;
				// TODO : produces duplicates
				foreach (var derived in FindDerivedMembers(ancestor))
					yield return derived;
			}
			foreach (var derived in FindDerivedMembers(member))
				yield return derived;
		}
		
		static IEnumerable<IMember> FindDerivedMembers(IMember member)
		{
			ITypeDefinition definition = member.DeclaringTypeDefinition;
			if (definition == null) yield break;
			var tree = BuildDerivedTypesGraph(definition).ConvertToDerivedTypeTree();
			var derivedMembers = TreeTraversal.PreOrder(tree, node => node.Children)
				.Select(n => FindDerivedMember(member, n));
			foreach (var otherMember in derivedMembers.Where(m => m != null))
				yield return otherMember;
		}
		
		static IMember FindDerivedMember(IMember member, ITreeNode<ITypeDefinition> node)
		{
//			if (endPoint != null && (node.Content.Equals(endPoint) || node.Content.GetAllBaseTypeDefinitions().Any(td => endPoint.Equals(td))))
//				return null;
			var importedMember = node.Content.Compilation.Import(member);
			if (importedMember == null) return null;
			return InheritanceHelper.GetDerivedMember(importedMember, node.Content);
		}
		
		public static IObservable<SearchedFile> FindReferencesToHierarchy(IMember member, IProgressMonitor progressMonitor)
		{
			return ReactiveExtensions.CreateObservable<SearchedFile>(
				(monitor, callback) => FindReferencesToHierarchyAsync(member, monitor, callback),
				progressMonitor);
		}
		
		/// <summary>
		/// Finds references to a local variable.
		/// </summary>
		public static async Task<SearchedFile> FindLocalReferencesAsync(IVariable variable, IProgressMonitor progressMonitor)
		{
			if (variable == null)
				throw new ArgumentNullException("variable");
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			var fileName = FileName.Create(variable.Region.FileName);
			List<Reference> references = new List<Reference>();
			await SD.ParserService.FindLocalReferencesAsync(
				fileName, variable,
				r => { lock (references) references.Add(r); },
				cancellationToken: progressMonitor.CancellationToken);
			return new SearchedFile(fileName, references);
		}
		
		public static IObservable<SearchedFile> FindLocalReferences(IVariable variable, IProgressMonitor progressMonitor)
		{
			return ReactiveExtensions.CreateObservable<SearchedFile>(
				(monitor, callback) => FindLocalReferencesAsync(variable, monitor).ContinueWith(t => callback(t.Result)),
				progressMonitor);
		}
//		#endregion
		
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
		
		static ISolutionSnapshotWithProjectMapping GetSolutionSnapshot(ICompilation compilation)
		{
			var snapshot = compilation.SolutionSnapshot as ISolutionSnapshotWithProjectMapping;
			return snapshot ?? SD.ParserService.GetCurrentSolutionSnapshot();
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
			if (graph.TryGetValue(new AssemblyQualifiedTypeName(baseType), out node)) {
				// only derived types were requested, so don't return the base types
				// (this helps the GC to collect the unused parts of the graph more quickly)
				node.BaseTypes.Clear();
				return node;
			} else {
				return new TypeGraphNode(baseType);
			}
		}
		
		/// <summary>
		/// Builds a graph of type definitions.
		/// </summary>
		public static TypeGraphNode BuildFullTypeGraph(ITypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException("baseType");
			var solutionSnapshot = GetSolutionSnapshot(type.Compilation);
			var compilations = GetProjectsThatCouldReferenceEntity(type).Select(p => solutionSnapshot.GetCompilation(p));
			var graph = BuildTypeInheritanceGraph(compilations);
			TypeGraphNode node;
			if (graph.TryGetValue(new AssemblyQualifiedTypeName(type), out node)) {
				// only derived types were requested, so don't return the base types
				// (this helps the GC to collect the unused parts of the graph more quickly)
				return node;
			} else {
				return new TypeGraphNode(type);
			}
		}
		
		/// <summary>
		/// Builds a graph of all type definitions in the specified set of project contents.
		/// </summary>
		/// <remarks>The resulting graph may be cyclic if there are cyclic type definitions.</remarks>
		static Dictionary<AssemblyQualifiedTypeName, TypeGraphNode> BuildTypeInheritanceGraph(IEnumerable<ICompilation> compilations)
		{
			if (compilations == null)
				throw new ArgumentNullException("projectContents");
			Dictionary<AssemblyQualifiedTypeName, TypeGraphNode> dict = new Dictionary<AssemblyQualifiedTypeName, TypeGraphNode>();
			foreach (ICompilation compilation in compilations) {
				foreach (ITypeDefinition typeDef in compilation.MainAssembly.GetAllTypeDefinitions()) {
					// Overwrite previous entry - duplicates can occur if there are multiple versions of the
					// same project loaded in the solution (e.g. separate .csprojs for separate target frameworks)
					dict[new AssemblyQualifiedTypeName(typeDef)] = new TypeGraphNode(typeDef);
				}
			}
			foreach (ICompilation compilation in compilations) {
				foreach (ITypeDefinition typeDef in compilation.MainAssembly.GetAllTypeDefinitions()) {
					TypeGraphNode typeNode = dict[new AssemblyQualifiedTypeName(typeDef)];
					foreach (IType baseType in typeDef.DirectBaseTypes) {
						ITypeDefinition baseTypeDef = baseType.GetDefinition();
						if (baseTypeDef != null) {
							TypeGraphNode baseTypeNode;
							if (dict.TryGetValue(new AssemblyQualifiedTypeName(baseTypeDef), out baseTypeNode)) {
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
		
		Task FindReferencesAsync(SymbolSearchArgs searchArguments, Action<SearchedFile> callback);
	}
	
	public sealed class CompositeSymbolSearch : ISymbolSearch
	{
		IEnumerable<ISymbolSearch> symbolSearches;
		
		CompositeSymbolSearch(params ISymbolSearch[] symbolSearches)
		{
			this.symbolSearches = symbolSearches;
		}
		
		public static ISymbolSearch Create(ISymbolSearch symbolSearch1, ISymbolSearch symbolSearch2)
		{
			if (symbolSearch1 == null)
				return symbolSearch2;
			if (symbolSearch2 == null)
				return symbolSearch1;
			return new CompositeSymbolSearch(symbolSearch1, symbolSearch2);
		}
		
		public double WorkAmount {
			get { return symbolSearches.Sum(s => s.WorkAmount); }
		}
		
		public Task FindReferencesAsync(SymbolSearchArgs searchArguments, Action<SearchedFile> callback)
		{
			return Task.WhenAll(symbolSearches.Select(s => s.FindReferencesAsync(searchArguments, callback)));
		}
	}
}
