// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using ICSharpCode.CodeCoverage;
using ICSharpCode.CodeCoverage.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class CodeCoveragePropertyTestFixture
	{
		CodeCoverageMethod getter;
		CodeCoverageMethod setter;
		
		[SetUp]
		public void Init()
		{
			XElement getterElement = CreateGetterElement("System.Int32 MyTest::get_Count()");
			getter = new CodeCoverageMethod("MyTest", getterElement);
			XElement setterElement = CreateSetterElement("System.Void MyTest::set_Count(System.Int32)");
			setter = new CodeCoverageMethod("MyTest", setterElement);
		}
		
		XElement CreateGetterElement(string methodSignature)
		{
			return CodeCoverageMethodXElementBuilder.CreateGetterMethod(methodSignature);
		}
		
		XElement CreateSetterElement(string methodSignature)
		{
			return CodeCoverageMethodXElementBuilder.CreateSetterMethod(methodSignature);
		}
		
		[Test]
		public void CodeCoveragePropertyNameFromGetter()
		{
			Assert.AreEqual("Count", CodeCoverageProperty.GetPropertyName(getter));
		}
		
		[Test]
		public void CodeCoveragePropertyNameFromSetter()
		{
			Assert.AreEqual("Count", CodeCoverageProperty.GetPropertyName(setter));
		}
		
		[Test]
		public void PropertyNameIsEmptyStringWhenMethodIsNotGetter()
		{
			CodeCoverageMethod method = new CodeCoverageMethod("Count", "MyTest");
			Assert.AreEqual(String.Empty, CodeCoverageProperty.GetPropertyName(method));
		}
		
		[Test]
		public void GetterMethodIsGetterProperty()
		{
			Assert.IsTrue(CodeCoverageProperty.IsGetter(getter));
		}

		[Test]
		public void SetterMethodIsSetterProperty()
		{
			Assert.IsTrue(CodeCoverageProperty.IsSetter(setter));
		}
		
		[Test]
		public void PropertyNameFromGetter()
		{
			CodeCoverageProperty property = new CodeCoverageProperty(getter);
			Assert.AreEqual("Count", property.Name);
		}
		
		[Test]
		public void PropertyNameFromSetter()
		{
			CodeCoverageProperty property = new CodeCoverageProperty(setter);
			Assert.AreEqual("Count", property.Name);
		}
		
		[Test]
		public void SingleCodeCoverageMethodWhenOnlyGetter()
		{
			CodeCoverageProperty property = new CodeCoverageProperty(getter);
			List<CodeCoverageMethod> expectedMethods = new List<CodeCoverageMethod>();
			expectedMethods.Add(getter);
			Assert.AreEqual(expectedMethods, property.GetMethods());
		}

		[Test]
		public void SingleCodeCoverageMethodWhenOnlySetter()
		{
			CodeCoverageProperty property = new CodeCoverageProperty(setter);
			List<CodeCoverageMethod> expectedMethods = new List<CodeCoverageMethod>();
			expectedMethods.Add(setter);
			Assert.AreEqual(expectedMethods, property.GetMethods());
		}
		
		[Test]
		public void TwoCodeCoverageMethodsWhenSetterAndGetterAdded()
		{
			CodeCoverageProperty property = new CodeCoverageProperty(getter);
			property.AddMethod(setter);
			List<CodeCoverageMethod> expectedMethods = new List<CodeCoverageMethod>();
			expectedMethods.Add(getter);
			expectedMethods.Add(setter);
			Assert.AreEqual(expectedMethods, property.GetMethods());
		}
	}
}
