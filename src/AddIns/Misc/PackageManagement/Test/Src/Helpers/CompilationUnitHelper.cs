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
//using ICSharpCode.SharpDevelop.Dom;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.Helpers
//{
//	public class CompilationUnitHelper
//	{
//		public ICompilationUnit CompilationUnit;
//		public FakeCodeGenerator FakeCodeGenerator = new FakeCodeGenerator();
//		public List<IClass> Classes = new List<IClass>();
//		public UsingScopeHelper UsingScopeHelper = new UsingScopeHelper();
//		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
//		
//		public CompilationUnitHelper()
//		{
//			CompilationUnit = MockRepository.GenerateStub<ICompilationUnit>();
//			LanguageProperties language = MockRepository.GenerateStub<LanguageProperties>(StringComparer.InvariantCultureIgnoreCase);
//			language.Stub(lang => lang.CodeGenerator).Return(FakeCodeGenerator);
//			CompilationUnit.Stub(unit => unit.Language).Return(language);
//			CompilationUnit.Stub(unit => unit.Classes).Return(Classes);
//			CompilationUnit.Stub(unit => unit.UsingScope).Return(UsingScopeHelper.UsingScope);
//			CompilationUnit.Stub(unit => unit.ProjectContent).Return(ProjectContentHelper.ProjectContent);
//		}
//		
//		public void SetFileName(string fileName)
//		{
//			CompilationUnit.FileName = fileName;
//		}
//		
//		public void AddClass(IClass c)
//		{
//			Classes.Add(c);
//		}
//		
//		public void AddNamespace(string name)
//		{
//			UsingScopeHelper.AddNamespace(name);
//		}
//		
//		public void AddNamespaceAlias(string alias, string namespaceName)
//		{
//			UsingScopeHelper.AddNamespaceAlias(alias, namespaceName);
//		}
//	}
//}
