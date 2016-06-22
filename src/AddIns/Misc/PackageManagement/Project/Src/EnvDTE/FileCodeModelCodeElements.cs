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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ICSharpCode.SharpDevelop.Dom;
//using ICSharpCode.SharpDevelop.Project;
//
//namespace ICSharpCode.PackageManagement.EnvDTE
//{
//	public class FileCodeModelCodeElements : CodeElementsList
//	{
//		ICompilationUnit compilationUnit;
//		List<FileCodeModelCodeNamespace> fileCodeModelNamespaces = new List<FileCodeModelCodeNamespace>();
//		
//		public FileCodeModelCodeElements(ICompilationUnit compilationUnit)
//		{
//			this.compilationUnit = compilationUnit;
//			GetCodeElements();
//		}
//		
//		void GetCodeElements()
//		{
//			foreach (IUsing namespaceUsing in GetNamespaceImports()) {
//				AddNamespaceImport(namespaceUsing);
//			}
//			AddClasses();
//		}
//		
//		IList<IUsing> GetNamespaceImports()
//		{
//			return compilationUnit
//				.UsingScope
//				.Usings;
//		}
//		
//		void AddNamespaceImport(IUsing import)
//		{
//			AddCodeElement(new CodeImport(import));
//		}
//		
//		void AddClasses()
//		{
//			foreach (IClass c in compilationUnit.Classes) {
//				FileCodeModelCodeNamespace codeNamespace = GetOrCreateFileCodeModelNamespace(c);
//				codeNamespace.AddClass(compilationUnit.ProjectContent, c);
//			}
//		}
//		
//		FileCodeModelCodeNamespace GetOrCreateFileCodeModelNamespace(IClass c)
//		{
//			var codeNamespace = FindFileCodeModelNamespace(c);
//			if (codeNamespace != null) {
//				return codeNamespace;
//			}
//			return CreateFileCodeModelNamespace(c);
//		}
//		
//		FileCodeModelCodeNamespace FindFileCodeModelNamespace(IClass c)
//		{
//			return fileCodeModelNamespaces.FirstOrDefault(ns => ns.FullName == c.Namespace);
//		}
//		
//		FileCodeModelCodeNamespace CreateFileCodeModelNamespace(IClass c)
//		{
//			var codeNamespace = new FileCodeModelCodeNamespace(compilationUnit.ProjectContent, c.Namespace);
//			AddCodeElement(codeNamespace);
//			fileCodeModelNamespaces.Add(codeNamespace);
//			return codeNamespace;
//		}
//	}
//}
