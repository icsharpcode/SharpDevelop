// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the BaseTestMethod populates the various
	/// properties of the DefaultMethod class in its constructor.
	/// </summary>
	[TestFixture]
	public class BaseTestMethodTestFixture
	{
		MockClass mockClass;
		MockMethod mockMethod;
		BaseTestMember baseTestMethod;
		DomRegion mockMethodRegion;
		DomRegion mockMethodBodyRegion;
		DefaultReturnType returnType;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			mockClass = new MockClass("Tests.MyTestFixture");
			mockMethod = new MockMethod(mockClass, "MyMethod");
			
			mockMethodRegion = new DomRegion(0, 0, 0, 10);
			mockMethod.Region = mockMethodRegion;
			mockMethodBodyRegion = new DomRegion(1, 0, 2, 5);
			mockMethod.BodyRegion = mockMethodBodyRegion;
			mockMethod.Modifiers = ModifierEnum.Public;
		
			MockClass returnTypeClass = new MockClass("Tests.ReturnType");
			returnType = new DefaultReturnType(returnTypeClass);
			mockMethod.ReturnType = returnType;
			
			baseTestMethod = new BaseTestMember(mockClass, mockMethod);
		}
		
		[Test]
		public void MethodName()
		{
			Assert.AreEqual("MyTestFixture.MyMethod", baseTestMethod.Name);
		}
		
		[Test]
		public void DeclaringType()
		{
			Assert.AreEqual(mockClass, baseTestMethod.DeclaringType);
		}
		
		[Test]
		public void ActualMethod()
		{
			Assert.AreEqual(mockMethod, baseTestMethod.Member);
		}
		
		[Test]
		public void MethodRegion()
		{
			Assert.AreEqual(mockMethodRegion, baseTestMethod.Region);
		}

		[Test]
		public void MethodBodyRegion()
		{
			Assert.AreEqual(mockMethodBodyRegion, baseTestMethod.BodyRegion);
		}
		
		[Test]
		public void Modifiers()
		{
			Assert.AreEqual(ModifierEnum.Public, baseTestMethod.Modifiers);
		}
		
		[Test]
		public void ReturnType()
		{
			Assert.AreSame(returnType, baseTestMethod.ReturnType);
		}
	}
}
