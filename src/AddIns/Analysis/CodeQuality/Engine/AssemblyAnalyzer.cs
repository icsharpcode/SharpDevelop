// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;

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
			var loadedAssemblies = LoadAssemblies().ToArray();
			compilation = new SimpleCompilation(loadedAssemblies.First(), loadedAssemblies.Skip(1));
			
			assemblyMappings = new Dictionary<IAssembly, AssemblyNode>();
			namespaceMappings = new Dictionary<string, NamespaceNode>();
			typeMappings = new Dictionary<ITypeDefinition, TypeNode>();
			
			foreach (var type in compilation.GetAllTypeDefinitions()) {
				AnalyzeType(type);
			}
			return new ReadOnlyCollection<AssemblyNode>(assemblyMappings.Values.ToList());
		}
		
		IEnumerable<IUnresolvedAssembly> LoadAssemblies()
		{
			var resolver = new DefaultAssemblyResolver();
			foreach (var file in fileNames) {
				var mainAsm = loader.LoadAssemblyFile(file);
				yield return mainAsm;
				var referencedAssemblies = loader.GetCecilObject(mainAsm).Modules
					.SelectMany(m => m.AssemblyReferences)
					.Select(r => resolver.Resolve(r));
				foreach (var asm in referencedAssemblies)
					yield return loader.LoadAssembly(asm);
			}
		}
		
		NamespaceNode GetOrCreateNamespace(AssemblyNode assembly, string namespaceName)
		{
			NamespaceNode result;
			var asmDef = loader.GetCecilObject(assembly.AssemblyInfo.UnresolvedAssembly);
			if (!namespaceMappings.TryGetValue(namespaceName + "," + asmDef.FullName, out result)) {
				result = new NamespaceNode(namespaceName);
				assembly.Children.Add(result);
				namespaceMappings.Add(namespaceName + "," + asmDef.FullName, result);
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
			var ns = GetOrCreateNamespace(asm, type.Namespace);
			var node = new TypeNode(type);
			typeMappings.Add(type, node);
			ns.Children.Add(node);
		}
	}
}
