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
	/// Tests TestMethod.GetQualifiedClassName.
	/// </summary>
	[TestFixture]
	public class GetQualifiedClassNameTests
	{
		[Test]
		public void GetClassName()
		{
			string name = "RootNamespace.ClassName.Method";
			Assert.AreEqual("RootNamespace.ClassName", TestMethod.GetQualifiedClassName(name));
		}
		
		[Test]
		public void NoRootNamespace()
		{
			string name = "ClassName.Method";
			Assert.AreEqual("ClassName", TestMethod.GetQualifiedClassName(name));
		}
		
		[Test]
		public void OnlyMethod()
		{
			string name = "Method";
			Assert.IsNull(TestMethod.GetQualifiedClassName(name));
		}
	}
}
