// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests TestMember.GetMemberName.
	/// </summary>
	[TestFixture]
	public class TestMemberGetMethodNameTests
	{
		[Test]
		public void RootNamespaceClass()
		{
			string qualifiedName = "RootNamespace.TestFixture.Method";
			Assert.AreEqual("Method", TestMember.GetMemberName(qualifiedName));
		}
		
		[Test]
		public void NullName()
		{
			Assert.IsNull(TestMember.GetMemberName(null));
		}
		
		[Test]
		public void NoClassOrNamespace()
		{
			Assert.IsNull(TestMember.GetMemberName("Method"));
		}
	}
}
