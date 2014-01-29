// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	public class CodeModelTestBase : CSharpModelTestBase
	{
		protected CodeGenerator codeGenerator;
		protected CodeModelContext codeModelContext;
		protected string projectLanguage = "C#";
		
		public override void SetUp()
		{
			base.SetUp();
			project.Stub(p => p.Language).Return(null).WhenCalled(mi => mi.ReturnValue = projectLanguage);
			codeGenerator = MockRepository.GenerateStrictMock<CodeGenerator>();
			codeModelContext = new CodeModelContext {
				CodeGenerator = codeGenerator,
				CurrentProject = project
			};
		}
	}
}
