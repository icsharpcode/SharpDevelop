// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class DestructorDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpDestructorDeclarationTest()
		{
			DestructorDeclaration dd = ParseUtilCSharp.ParseTypeMember<DestructorDeclaration>("~MyClass() {}");
		}
		
		[Test]
		public void CSharpExternDestructorDeclarationTest()
		{
			DestructorDeclaration dd = ParseUtilCSharp.ParseTypeMember<DestructorDeclaration>("extern ~MyClass();");
			Assert.AreEqual(Modifiers.Extern, dd.Modifier);
		}
		
		[Test]
		public void CSharpUnsafeDestructorDeclarationTest()
		{
			DestructorDeclaration dd = ParseUtilCSharp.ParseTypeMember<DestructorDeclaration>("unsafe ~MyClass() {}");
			Assert.AreEqual(Modifiers.Unsafe, dd.Modifier);
		}
		#endregion
		
		#region VB.NET
		// No VB.NET representation
		#endregion
	}
}
