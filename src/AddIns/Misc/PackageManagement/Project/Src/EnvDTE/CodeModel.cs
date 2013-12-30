// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeModel : MarshalByRefObject, global::EnvDTE.CodeModel
	{
		Project project;
		CodeElementsInNamespace codeElements;
		CodeModelContext context;
		
		public CodeModel(CodeModelContext context, Project project)
		{
			this.context = context;
			this.project = project;
		}
		
		public global::EnvDTE.CodeElements CodeElements {
			get {
				if (codeElements == null) {
					codeElements = new CodeElementsInNamespace(context);
				}
				return codeElements;
			}
		}
		
		public global::EnvDTE.CodeType CodeTypeFromFullName(string name)
		{
			ITypeDefinition typeDefinition = GetTypeDefinition(name);
			if (typeDefinition != null) {
				return CreateCodeTypeForTypeDefinition(typeDefinition);
			}
			return null;
		}
		
		ITypeDefinition GetTypeDefinition(string name)
		{
			ICompilation compilation = project.GetCompilationUnit();
			var typeName = new TopLevelTypeName(name);
			
			foreach (IAssembly assembly in compilation.Assemblies) {
				ITypeDefinition typeDefinition = assembly.GetTypeDefinition(typeName);
				if (typeDefinition != null) {
					return typeDefinition;
				}
			}
			
			return null;
		}
		
		CodeType CreateCodeTypeForTypeDefinition(ITypeDefinition typeDefinition)
		{
			if (typeDefinition.Kind == TypeKind.Interface) {
				return new CodeInterface(context, typeDefinition);
			}
			return new CodeClass2(context, typeDefinition);
		}
		
		public string Language {
			get { return project.MSBuildProject.GetCodeModelLanguage(); }
		}
	}
}
