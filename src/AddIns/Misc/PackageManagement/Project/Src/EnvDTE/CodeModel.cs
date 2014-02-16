// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
