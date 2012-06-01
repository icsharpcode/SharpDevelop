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
		MethodHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new MethodHelper();
		}
		
		void CreatePublicFunction(string name)
		{
			helper.CreatePublicMethod(name);
			CreateFunction();
		}
		
		void CreatePrivateFunction(string name)
		{
			helper.CreatePrivateMethod(name);
			CreateFunction();
		}
		
		void CreateFunction()
		{
			codeFunction = new CodeFunction(helper.Method);
		}
		
		[Test]
		public void Access_PublicFunction_ReturnsPublic()
		{
			CreatePublicFunction("Class1.MyFunction");
			
			vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateFunction_ReturnsPrivate()
		{
			CreatePrivateFunction("Class1.MyFunction");
			
			vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
	}
}
