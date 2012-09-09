// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTesting.Tests.Project;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class NUnitTestFrameworkIsTestClassTests : ProjectTestFixtureBase
	{
		NUnitTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new NUnitTestFramework();
		}
		
		[Test]
		public void IsTestClassReturnsFalseHasClassHasNoAttributes()
		{
			CreateProject(testFramework, Parse("class MyClass {}"));
			ITypeDefinition myClass = projectContent.CreateCompilation().MainAssembly.GetTypeDefinition("", "MyClass", 0);
			Assert.IsFalse(testFramework.IsTestClass(myClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttributeMissingAttributePart()
		{
			CreateProject(testFramework, Parse("using NUnit.Framework; [TestFixture] class MyClass {}"));
			ITypeDefinition myClass = projectContent.CreateCompilation().MainAssembly.GetTypeDefinition("", "MyClass", 0);
			Assert.IsTrue(testFramework.IsTestClass(myClass));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassHasTestFixtureAttributeAndIsAbstract() {
			CreateProject(testFramework, Parse("using NUnit.Framework; [TestFixtureAttribute] abstract class MyClass {}"));
			ITypeDefinition myClass = projectContent.CreateCompilation().MainAssembly.GetTypeDefinition("", "MyClass", 0);
			Assert.IsFalse(testFramework.IsTestClass(myClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttribute()
		{
			CreateProject(testFramework, Parse("using NUnit.Framework; [TestFixtureAttribute] class MyClass {}"));
			ITypeDefinition myClass = projectContent.CreateCompilation().MainAssembly.GetTypeDefinition("", "MyClass", 0);
			Assert.IsTrue(testFramework.IsTestClass(myClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasFullyQualifiedNUnitTestFixtureAttribute()
		{
			CreateProject(testFramework, Parse("[NUnit.Framework.TestFixtureAttribute] class MyClass {}"));
			ITypeDefinition myClass = projectContent.CreateCompilation().MainAssembly.GetTypeDefinition("", "MyClass", 0);
			Assert.IsTrue(testFramework.IsTestClass(myClass));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassIsNull()
		{
			Assert.IsFalse(testFramework.IsTestClass(null));
		}
	}
}
