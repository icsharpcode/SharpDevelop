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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModel2 : MarshalByRefObject, global::EnvDTE.FileCodeModel2
	{
		readonly CodeModelContext context;
		CodeElementsList<CodeElement> codeElements = new CodeElementsList<CodeElement>();
		Dictionary<string, FileCodeModelCodeNamespace> namespaces = new Dictionary<string, FileCodeModelCodeNamespace>();
		
		public FileCodeModel2(CodeModelContext context)
		{
			if (context == null || context.FilteredFileName == null)
				throw new ArgumentException("context must be restricted to a file");
			this.context = context;
			var compilation = SD.ParserService.GetCompilation(context.CurrentProject);
			
			var projectContent = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			if (projectContent != null) {
				IUnresolvedFile file = projectContent.GetFile(context.FilteredFileName);
				if (file != null) {
					var csharpFile = file as CSharpUnresolvedFile;
					if (csharpFile != null)
						AddUsings(codeElements, csharpFile.RootUsingScope, compilation);
					
					var resolveContext = new SimpleTypeResolveContext(compilation.MainAssembly);
					AddTypes(file.TopLevelTypeDefinitions
					         .Select(td => td.Resolve(resolveContext) as ITypeDefinition)
					         .Where(td => td != null).Distinct());
				}
			}
		}
		
		public global::EnvDTE.CodeElements CodeElements {
			get { return codeElements; }
		}
		
		void AddTypes(IEnumerable<ITypeDefinition> types)
		{
			foreach (var td in types) {
				var model = td.GetModel();
				if (model == null)
					continue;
				var codeType = CodeType.Create(context, td);
				if (string.IsNullOrEmpty(td.Namespace))
					codeElements.Add(codeType);
				else
					GetNamespace(td.Namespace).AddMember(codeType);
			}
			codeElements.AddRange(types.Select(td => CodeType.Create(context, td)));
		}
		
		public static void AddUsings(CodeElementsList<CodeElement> codeElements, UsingScope usingScope, ICompilation compilation)
		{
			var resolvedUsingScope = usingScope.Resolve(compilation);
			foreach (var ns in resolvedUsingScope.Usings) {
				codeElements.Add(new CodeImport(ns.FullName));
			}
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
			return null;
		}
	}
}
