// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
		CecilLoader loader = new CecilLoader(true) { IncludeInternalMembers = true };
		ICompilation compilation;
		internal Dictionary<IAssembly, AssemblyNode> assemblyMappings;
		internal Dictionary<string, NamespaceNode> namespaceMappings;
		internal Dictionary<ITypeDefinition, TypeNode> typeMappings;
		internal Dictionary<IMethod, MethodNode> methodMappings;
		internal Dictionary<IField, FieldNode> fieldMappings;
		internal Dictionary<MemberReference, IEntity> cecilMappings;
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
			fieldMappings = new Dictionary<IField, FieldNode>();
			methodMappings = new Dictionary<IMethod, MethodNode>();
			cecilMappings = new Dictionary<MemberReference, IEntity>();
			
			foreach (var type in compilation.GetAllTypeDefinitions()) {
				ReadType(type);
			}
			foreach (TypeNode type in typeMappings.Values) {
				foreach (var field in type.TypeDefinition.Fields) {
					var node = new FieldNode(field);
					fieldMappings.Add(field, node);
					try {
						var cecilObj = loader.GetCecilObject((IUnresolvedField)field.UnresolvedMember);
						cecilMappings[cecilObj] = field;
					} catch (InvalidOperationException) {}
					type.Children.Add(node);
				}
				
				foreach (var method in type.TypeDefinition.Methods) {
					var node = new MethodNode(method);
					methodMappings.Add(method, node);
					try {
						var cecilObj = loader.GetCecilObject((IUnresolvedMethod)method.UnresolvedMember);
						cecilMappings[cecilObj] = method;
					} catch (InvalidOperationException) {}
					type.Children.Add(node);
				}
			}
			ILAnalyzer analyzer = new ILAnalyzer(loadedAssemblies.Select(asm => loader.GetCecilObject(asm)).ToArray(), this);
			foreach (var element in methodMappings) {
				int cc;
				try {
					var cecilObj = loader.GetCecilObject((IUnresolvedMethod)element.Key.UnresolvedMember);
					analyzer.Analyze(cecilObj.Body, element.Value, out cc);
				} catch (InvalidOperationException) {}
			}
			return new ReadOnlyCollection<AssemblyNode>(assemblyMappings.Values.ToList());
		}
		
		IEnumerable<IUnresolvedAssembly> LoadAssemblies()
		{
			var resolver = new AssemblyResolver();
			List<AssemblyDefinition> assemblies = new List<AssemblyDefinition>();
			foreach (var file in fileNames.Distinct(StringComparer.OrdinalIgnoreCase))
				assemblies.Add(resolver.LoadAssemblyFile(file));
			foreach (var asm in assemblies.ToArray())
				assemblies.AddRange(asm.Modules.SelectMany(m => m.AssemblyReferences).Select(r => resolver.Resolve(r)));
			return assemblies.Distinct().Select(asm => loader.LoadAssembly(asm));
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
		
		void ReadType(ITypeDefinition type)
		{
			var asm = GetOrCreateAssembly(type.ParentAssembly);
			var ns = GetOrCreateNamespace(asm, type.Namespace);
			TypeNode parent;
			var node = new TypeNode(type);
			if (type.DeclaringTypeDefinition != null) {
				if (typeMappings.TryGetValue(type.DeclaringTypeDefinition, out parent))
					parent.Children.Add(node);
				else
					throw new Exception("TypeNode not found: " + type.DeclaringTypeDefinition.FullName);
			} else
				ns.Children.Add(node);
			cecilMappings[loader.GetCecilObject(type.Parts.First())] = type;
			typeMappings.Add(type, node);
		}
		
		class AssemblyResolver : DefaultAssemblyResolver
		{
			public AssemblyDefinition LoadAssemblyFile(string fileName)
			{
				var assembly = AssemblyDefinition.ReadAssembly(fileName, new ReaderParameters { AssemblyResolver = this });
				RegisterAssembly(assembly);
				return assembly;
			}
		}
	}
}
