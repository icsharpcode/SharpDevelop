// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModel2 : MarshalByRefObject, global::EnvDTE.FileCodeModel2
	{
		CodeModelContext context;
		Project project;
		CodeElementsList<CodeElement> codeElements;
		Dictionary<string, FileCodeModelCodeNamespace> namespaces = new Dictionary<string, FileCodeModelCodeNamespace>();
		
		public FileCodeModel2(CodeModelContext context, Project project)
		{
			if (context == null || context.FilteredFileName == null) {
				throw new ArgumentException("context must be restricted to a file");
			}
			
			this.context = context;
			this.project = project;
		}
		
		public global::EnvDTE.CodeElements CodeElements {
			get {
				if (codeElements == null) {
					codeElements = new CodeElementsList<CodeElement>();
					AddCodeElements();
				}
				return codeElements;
			}
		}
		
		void AddCodeElements()
		{
			ICompilation compilation = project.GetCompilationUnit();
			
			var projectContent = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			if (projectContent != null) {
				IUnresolvedFile file = projectContent.GetFile(context.FilteredFileName);
				if (file != null) {
					var csharpFile = file as CSharpUnresolvedFile;
					if (csharpFile != null) {
						AddUsings(csharpFile.RootUsingScope, compilation);
					}
					
					var resolveContext = new SimpleTypeResolveContext(compilation.MainAssembly);
					AddTypes(
						file.TopLevelTypeDefinitions
						.Select(td => td.Resolve(resolveContext) as ITypeDefinition)
						.Where(td => td != null)
						.Distinct());
				}
			}
		}
		
		void AddTypes(IEnumerable<ITypeDefinition> types)
		{
			foreach (ITypeDefinition typeDefinition in types) {
				CodeType codeType = CodeType.Create(context, typeDefinition);
				if (string.IsNullOrEmpty(typeDefinition.Namespace)) {
					codeElements.Add(codeType);
				} else {
					GetNamespace(typeDefinition.Namespace).AddMember(codeType);
				}
			}
		}
		
		public void AddUsings(UsingScope usingScope, ICompilation compilation)
		{
			foreach (KeyValuePair<string, TypeOrNamespaceReference> alias in usingScope.UsingAliases) {
				AddCodeImport(alias.Value.ToString());
			}
			
			foreach (TypeOrNamespaceReference typeOrNamespace in usingScope.Usings) {
				AddCodeImport(typeOrNamespace.ToString());
			}
		}
		
		void AddCodeImport(string namespaceName)
		{
			codeElements.Add(new CodeImport(namespaceName));
		}
		
		public void AddImport(string name, object position = null, string alias = null)
		{
			context.CodeGenerator.AddImport(FileName.Create(context.FilteredFileName), name);
		}
		
		internal FileCodeModelCodeNamespace GetNamespace(string namespaceName)
		{
			FileCodeModelCodeNamespace ns;
			if (!namespaces.TryGetValue(namespaceName, out ns)) {
				ns = new FileCodeModelCodeNamespace(context, namespaceName);
				namespaces.Add(namespaceName, ns);
				codeElements.Add(ns);
			}
			return ns;
		}
	}
}
