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
		FieldHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new FieldHelper();
		}
				
		void CreatePublicVariable(string name)
		{
			helper.CreatePublicField(name);
			CreateVariable();
		}
		
		void CreatePrivateVariable(string name)
		{
			helper.CreatePrivateField(name);
			CreateVariable();
		}
		
		void CreateVariable()
		{
			codeVariable = new CodeVariable(helper.Field);
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
