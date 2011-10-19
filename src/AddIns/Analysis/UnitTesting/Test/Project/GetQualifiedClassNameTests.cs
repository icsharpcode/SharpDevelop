// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests TestMethod.GetQualifiedClassName.
	/// </summary>
	[TestFixture]
	public class GetQualifiedClassNameTests
	{
		[Test]
		public void GetClassName()
		{
			string name = "RootNamespace.ClassName.Method";
			Assert.AreEqual("RootNamespace.ClassName", TestMember.GetQualifiedClassName(name));
		}
		
		[Test]
		public void NoRootNamespace()
		{
			string name = "ClassName.Method";
			Assert.AreEqual("ClassName", TestMember.GetQualifiedClassName(name));
		}
		
		[Test]
		public void OnlyMethod()
		{
			string name = "Method";
			Assert.IsNull(TestMember.GetQualifiedClassName(name));
		}
	}
}
