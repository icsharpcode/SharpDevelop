// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests TestMethod.GetMethodName.
	/// </summary>
	[TestFixture]
	public class MethodNameTests
	{
		[Test]
		public void RootNamespaceClass()
		{
			string qualifiedName = "RootNamespace.TestFixture.Method";
			Assert.AreEqual("Method", TestMethod.GetMethodName(qualifiedName));
		}
		
		[Test]
		public void NullName()
		{
			Assert.IsNull(TestMethod.GetMethodName(null));
		}
		
		[Test]
		public void NoClassOrNamespace()
		{
			Assert.IsNull(TestMethod.GetMethodName("Method"));
		}
	}
}
