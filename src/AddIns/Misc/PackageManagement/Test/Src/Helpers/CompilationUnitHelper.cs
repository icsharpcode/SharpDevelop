// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class CompilationUnitHelper
	{
		public ICompilationUnit CompilationUnit;
		public FakeCodeGenerator FakeCodeGenerator = new FakeCodeGenerator();
		public List<IClass> Classes = new List<IClass>();
		
		public CompilationUnitHelper()
		{
			CompilationUnit = MockRepository.GenerateStub<ICompilationUnit>();
			LanguageProperties language = MockRepository.GenerateStub<LanguageProperties>(StringComparer.InvariantCultureIgnoreCase);
			language.Stub(lang => lang.CodeGenerator).Return(FakeCodeGenerator);
			CompilationUnit.Stub(unit => unit.Language).Return(language);
			CompilationUnit.Stub(unit => unit.Classes).Return(Classes);
		}
		
		public void SetFileName(string fileName)
		{
			CompilationUnit.FileName = fileName;
		}
		
		public void AddClass(IClass c)
		{
			Classes.Add(c);
		}
	}
}
