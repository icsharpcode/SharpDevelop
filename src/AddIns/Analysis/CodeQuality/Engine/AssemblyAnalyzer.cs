// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.CodeQuality.Engine
{
	/// <summary>
	/// Description of AssemblyAnalyzer.
	/// </summary>
	public class AssemblyAnalyzer
	{
		CecilLoader loader = new CecilLoader(true);
		ICompilation compilation;
		Dictionary<IAssembly, AssemblyNode> assemblyMappings;
		Dictionary<string, NamespaceNode> namespaceMappings;
		Dictionary<ITypeDefinition, TypeNode> typeMappings;
		Dictionary<IMethod, MethodNode> methodMappings;
		Dictionary<IField, FieldNode> fieldMappings;
		List<string> fileNames;
		
		public AssemblyAnalyzer()
		{
			fileNames = new List<string>();
		}
		
		public void AddAssemblyFiles(params string[] files)
		{
			fileNames.AddRange(files);
		}
		
		public ReadOnlyCollection<AssemblyNode> Analyze()
		{
			compilation = new SimpleCompilation(loader.LoadAssemblyFile(fileNames.First()), LoadOthorAssemblies());
			
			assemblyMappings = new Dictionary<IAssembly, AssemblyNode>();
			namespaceMappings = new Dictionary<string, NamespaceNode>();
			
			AssemblyNode mainAssembly = new AssemblyNode(compilation.MainAssembly);
			foreach (var type in compilation.GetAllTypeDefinitions()) {
				AnalyzeType(type);
			}
			return new ReadOnlyCollection<AssemblyNode>(assemblyMappings.Values.ToList());
		}
		
		IEnumerable<IUnresolvedAssembly> LoadOthorAssemblies()
		{
			foreach (var file in fileNames.Skip(1)) {
				yield return loader.LoadAssemblyFile(file);
			}
		}
		
		NamespaceNode GetOrCreateNamespace(string namespaceName)
		{
			NamespaceNode result;
			if (!namespaceMappings.TryGetValue(namespaceName, out result)) {
				result = new NamespaceNode(namespaceName);
				namespaceMappings.Add(namespaceName, result);
			}
			return result;
		}
		
		AssemblyNode GetOrCreateAssembly(IAssembly asm)
		{
			AssemblyNode result;
			if (!assemblyMappings.TryGetValue(asm, out result)) {
				result = new AssemblyNode(asm);
				assemblyMappings.Add(asm, result);
			}
			return result;
		}
		
		void AnalyzeType(ITypeDefinition type)
		{
			var asm = GetOrCreateAssembly(type.ParentAssembly);
			var ns = GetOrCreateNamespace(type.Namespace);
			if (!asm.namespaces.Contains(ns))
				asm.namespaces.Add(ns);
		}
	}
}
