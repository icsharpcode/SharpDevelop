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
	public class CodeVariableTests
	{
		CodeVariable codeVariable;
		IField field;
		ProjectContentHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateMSBuildField(string name)
		{
			field = MockRepository.GenerateMock<IField, IEntity>();
			field.Stub(f => f.ProjectContent).Return(helper.FakeProjectContent);
		}
		
		void CreateMSBuildPublicField(string name)
		{
			CreateMSBuildField(name);
			field.Stub(f => f.IsPublic).Return(true);
		}
		
		void CreateMSBuildPrivateField(string name)
		{
			CreateMSBuildField(name);
			field.Stub(f => f.IsPublic).Return(false);
			field.Stub(f => f.IsPrivate).Return(true);
		}
		
		void CreatePublicVariable(string name)
		{
			CreateMSBuildPublicField(name);
			CreateVariable();
		}
		
		void CreatePrivateVariable(string name)
		{
			CreateMSBuildPrivateField(name);
			CreateVariable();
		}
		
		void CreateVariable()
		{
			codeVariable = new CodeVariable(field);
		}
		
		[Test]
		public void Access_PublicVariable_ReturnsPublic()
		{
			CreatePublicVariable("MyVariable");
			
			vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateVariable_ReturnsPrivate()
		{
			CreatePrivateVariable("MyVariable");
			
			vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
	}
}
