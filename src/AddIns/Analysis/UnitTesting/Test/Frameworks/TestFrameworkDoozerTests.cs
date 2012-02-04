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
	public class TestFrameworkDoozerTests
	{
		Properties properties;
		TestFrameworkDescriptor descriptor;
		TestFrameworkDoozer doozer;
		MockTestFramework mockTestFramework;
		MockTestFrameworkFactory mockTestFrameworkFactory;
		
		void CreateDoozer()
		{
			properties = new Properties();
			properties["id"] = "Default";
			properties["class"] = "UnitTesting.Tests.Utils.MockTestFramework";
			Codon codon = new Codon(null, "TestFramework", properties, null);
			
			mockTestFrameworkFactory = new MockTestFrameworkFactory();
			mockTestFramework = mockTestFrameworkFactory.AddFakeTestFramework("UnitTesting.Tests.Utils.MockTestFramework");
			
			doozer = new TestFrameworkDoozer();
			descriptor = doozer.BuildItem(codon, mockTestFrameworkFactory);
		}
		
		[Test]
		public void Id_PropertiesIdIsDefault_ReturnsDefault()
		{
			CreateDoozer();
			properties["id"] = "Default";
			
			string id = descriptor.Id;
			
			Assert.AreEqual("Default", id);
		}
		
		[Test]
		public void IDoozer_TestFrameworkDoozer_ImplementsIDoozer()
		{
			CreateDoozer();
			IDoozer implementsDoozer = doozer as IDoozer;
			
			Assert.IsNotNull(implementsDoozer);
		}
		
		[Test]
		public void HandleConditions_TestFrameworkDoozerDoesNotHandleConditions_ReturnsFalse()
		{
			CreateDoozer();
			bool conditions = doozer.HandleConditions;
			
			Assert.IsFalse(conditions);
		}
		
		[Test]
		public void TestFramework_TestFrameworkDescriptorTestFrameworkProperty_ReturnsMockTestFrameworkObject()
		{
			CreateDoozer();
			bool result = descriptor.TestFramework is MockTestFramework;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void TestFramework_PropertyAccessTwice_OnlyOneObjectCreated()
		{
			CreateDoozer();
			ITestFramework testFramework = descriptor.TestFramework;
			testFramework = descriptor.TestFramework;
			
			List<string> expectedClassNames = new List<string>();
			expectedClassNames.Add("UnitTesting.Tests.Utils.MockTestFramework");
			
			List<string> classNamesCreated = mockTestFrameworkFactory.ClassNamesPassedToCreateMethod;
			
			Assert.AreEqual(expectedClassNames, classNamesCreated);
		}
	}
}
