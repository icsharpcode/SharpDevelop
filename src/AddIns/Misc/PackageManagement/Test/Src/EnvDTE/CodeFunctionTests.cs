// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeFunctionTests
	{
		CodeFunction codeFunction;
		IMethod method;
		ProjectContentHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateMSBuildMethod(string name)
		{
			method = MockRepository.GenerateMock<IMethod, IEntity>();
			method.Stub(m => m.ProjectContent).Return(helper.FakeProjectContent);
		}
		
		void CreateMSBuildPublicMethod(string name)
		{
			CreateMSBuildMethod(name);
			method.Stub(m => m.IsPublic).Return(true);
		}
		
		void CreateMSBuildPrivateMethod(string name)
		{
			CreateMSBuildMethod(name);
			method.Stub(m => m.IsPublic).Return(false);
			method.Stub(m => m.IsPrivate).Return(true);
		}
		
		void CreatePublicFunction(string name)
		{
			CreateMSBuildPublicMethod(name);
			CreateFunction();
		}
		
		void CreatePrivateFunction(string name)
		{
			CreateMSBuildPrivateMethod(name);
			CreateFunction();
		}
		
		void CreateFunction()
		{
			codeFunction = new CodeFunction(method);
		}
		
		[Test]
		public void Access_PublicFunction_ReturnsPublic()
		{
			CreatePublicFunction("MyFunction");
			
			vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateFunction_ReturnsPrivate()
		{
			CreatePrivateFunction("MyFunction");
			
			vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
	}
}
