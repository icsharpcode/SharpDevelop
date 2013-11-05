// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that a class with base type nested inside test fixture is recognized
	/// </summary>
	[TestFixture]
	public class DoubleNestedTestClassWithBaseType : NUnitTestProjectFixtureBase
	{
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFileInNamespace("test.cs", @"
public abstract class BaseClass {
	[Test]
	public void BaseFoo() {
	}
}

public class A
{
	public class InnerATest
	{
		[Test]
		public void FooBar()
		{
		}

		[TestFixture]
		public class InnerTestLevel2 : BaseClass {
		}
	}
}");
		}
		
		[Test]
		public void DoubleNestedClassShouldHaveTestMemberImportedFromBaseClass() 
		{
			var compilation = SD.ParserService.GetCompilation(project);
			var nestedClass = compilation.MainAssembly.GetTypeDefinition(new FullTypeName("RootNamespace.A+InnerATest+InnerTestLevel2"));
			var nestedTestClass = testProject.GetTestsForEntity(nestedClass).Single();
			var testMethod = (NUnitTestMethod)nestedTestClass.NestedTests.Single();
			Assert.AreEqual("BaseClass.BaseFoo", testMethod.MethodNameWithDeclaringTypeForInheritedTests);
		}
	}
}
