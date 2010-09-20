// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockObjectCreatorTestFixture
	{
		MockTestFrameworkFactory factory;
		MockTestFramework myTestFramework;
		
		[SetUp]
		public void Init()
		{
			factory = new MockTestFrameworkFactory();
			myTestFramework = new MockTestFramework();
			factory.Add("MyClass", myTestFramework);
		}
		
		[Test]
		public void CreateMethodPassedMyClassNameReturnsMyTestFramework()
		{
			Assert.AreSame(myTestFramework, factory.Create("MyClass"));
		}
		
		[Test]
		public void CreateMethodPassedUnknownClassNameReturnNull()
		{
			Assert.IsNull(factory.Create("Unknown"));
		}
		
		[Test]
		public void ClassNamesPassedToCreateAreRecorded()
		{
			List<string> expectedClassNames = new List<string>();
			expectedClassNames.Add("a");
			expectedClassNames.Add("b");
			expectedClassNames.Add("c");
			
			factory.Create("a");
			factory.Create("b");
			factory.Create("c");
			
			Assert.AreEqual(expectedClassNames, factory.ClassNamesPassedToCreateMethod);
		}
	}
}
