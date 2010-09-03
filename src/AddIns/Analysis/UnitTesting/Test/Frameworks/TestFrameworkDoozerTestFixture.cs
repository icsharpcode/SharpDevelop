// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class TestFrameworkDoozerTestFixture
	{
		TestFrameworkDescriptor descriptor;
		TestFrameworkDoozer doozer;
		MockTestFramework mockTestFramework;
		MockTestFrameworkFactory mockTestFrameworkFactory;
		
		[SetUp]
		public void Init()
		{
			Properties properties = new Properties();
			properties["id"] = "Default";
			properties["class"] = "UnitTesting.Tests.Utils.MockTestFramework";
			Codon codon = new Codon(null, "TestFramework", properties, null);
			
			mockTestFrameworkFactory = new MockTestFrameworkFactory();
			mockTestFramework = new MockTestFramework();
			mockTestFrameworkFactory.Add("UnitTesting.Tests.Utils.MockTestFramework", mockTestFramework);
			
			doozer = new TestFrameworkDoozer();
			descriptor = doozer.BuildItem(codon, mockTestFrameworkFactory);
		}
		
		[Test]
		public void TestFrameworkDescriptorIdIsDefault()
		{
			Assert.AreEqual("Default", descriptor.Id);
		}
		
		[Test]
		public void TestFrameworkDoozerImplementsIDoozer()
		{
			Assert.IsNotNull(doozer as IDoozer);
		}
		
		[Test]
		public void TestFrameworkDoozerDoesNotHandleConditions()
		{
			Assert.IsFalse(doozer.HandleConditions);
		}
		
		[Test]
		public void TestFrameworkDescriptorTestFrameworkPropertyReturnsMockTestFrameworkObject()
		{
			Assert.IsTrue(descriptor.TestFramework is MockTestFramework);
		}
		
		[Test]
		public void TestFrameworkDescriptorTestFrameworkPropertyOnlyCreatesOneObject()
		{
			ITestFramework testFramework = descriptor.TestFramework;
			testFramework = descriptor.TestFramework;
			
			List<string> expectedClassNames = new List<string>();
			expectedClassNames.Add("UnitTesting.Tests.Utils.MockTestFramework");
			
			Assert.AreEqual(expectedClassNames, mockTestFrameworkFactory.ClassNamesPassedToCreateMethod);
		}
	}
}
