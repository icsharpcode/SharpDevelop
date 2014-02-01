// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			XElement getterElement = CreateGetterElement("MyTest", "Count");
			getter = new CodeCoverageMethod("MyTest", getterElement);
			XElement setterElement = CreateSetterElement("MyTest", "Count");
			setter = new CodeCoverageMethod("MyTest", setterElement);
		}
		
		XElement CreateGetterElement(string className, string propertyName)
		{
			return CodeCoverageMethodXElementBuilder.CreateIntegerPropertyGetter(className, propertyName);
		}
		
		XElement CreateSetterElement(string className, string propertyName)
		{
			return CodeCoverageMethodXElementBuilder.CreateIntegerPropertySetter(className, propertyName);
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
			bool result = getter.IsGetter;
			
			Assert.IsTrue(result);
		}

		[Test]
		public void SetterMethodIsSetterProperty()
		{
			bool result = setter.IsSetter;
			
			Assert.IsTrue(result);
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
