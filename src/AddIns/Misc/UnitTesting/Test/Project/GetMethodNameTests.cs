// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
