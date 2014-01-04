// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeDelegateTests : CodeModelTestBase
	{
		CodeDelegate codeDelegate;
		
		void CreateDelegate(string code)
		{
			AddCodeFile("delegate.cs", code);
			codeDelegate = new CodeDelegate(
				codeModelContext,
				assemblyModel.TopLevelTypeDefinitions.First().Resolve());
		}

		[Test]
		public void Access_PublicDelegate_ReturnsPublic()
		{
			CreateDelegate("public delegate void MyDelegate(string param1);");
			
			global::EnvDTE.vsCMAccess access = codeDelegate.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateDelegate_ReturnsPrivate()
		{
			CreateDelegate("delegate void MyDelegate(string param1);");
			
			global::EnvDTE.vsCMAccess access = codeDelegate.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void Kind_PublicDelegate_ReturnsDelegate()
		{
			CreateDelegate("public delegate void MyDelegate(string param1);");
			
			global::EnvDTE.vsCMElement kind = codeDelegate.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementDelegate, kind);
		}
	}
}
