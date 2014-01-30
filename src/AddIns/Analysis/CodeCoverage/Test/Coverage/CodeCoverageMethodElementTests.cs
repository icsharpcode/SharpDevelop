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
using System.Xml.Linq;
using ICSharpCode.CodeCoverage.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class CodeCoverageMethodElementTests
	{
		CodeCoverageMethodElement methodElement;
		
		void CreateMethod(string name)
		{
			XElement element = CodeCoverageMethodXElementBuilder.CreateMethod("MyClass", name);
			CreateMethod(element);
		}
		
		void CreateMethod(XElement element)
		{
			methodElement = new CodeCoverageMethodElement(element);
		}
		
		void CreatePropertyGetterMethod(string propertyName)
		{
			XElement element = CodeCoverageMethodXElementBuilder.CreateIntegerPropertyGetter("MyClass", propertyName);
			CreateMethod(element);
		}
		
		void CreatePropertySetterMethod(string propertyName)
		{
			XElement element = CodeCoverageMethodXElementBuilder.CreateIntegerPropertySetter("MyClass", propertyName);
			CreateMethod(element);
		}
		
		void CreatePropertyGetterWithInvalidAttributeValue(string propertyName)
		{
			XElement element = CodeCoverageMethodXElementBuilder.CreateIntegerPropertyGetter("MyClass", propertyName);
			element.SetIsGetterAttributeValue("INVALID");
			CreateMethod(element);
		}
		
		void CreatePropertySetterWithInvalidAttributeValue(string propertyName)
		{
			XElement element = CodeCoverageMethodXElementBuilder.CreateIntegerPropertySetter("MyClass", propertyName);
			element.SetIsSetterAttributeValue("INVALID");
			CreateMethod(element);
		}
		
		void CreateMethodWithSignature(string methodSignature)
		{
			var builder = new CodeCoverageMethodXElementBuilder(methodSignature);
			CreateMethod(builder.MethodElement);
		}
		
		void CreateMethodElementThatHasNoNameElement()
		{
			var builder = new CodeCoverageMethodXElementBuilder(String.Empty);
			builder.MethodElement.RemoveNodes();
			CreateMethod(builder.MethodElement);
		}
		
		[Test]
		public void IsGetter_MethodIsNotProperty_ReturnsFalse()
		{
			CreateMethod("MyMethod");
			
			bool result = methodElement.IsGetter;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsProperty_MethodIsNotProperty_ReturnsFalse()
		{
			CreateMethod("MyMethod");
			
			bool result = methodElement.IsProperty;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsGetter_MethodIsPropertyGetter_ReturnsTrue()
		{
			CreatePropertyGetterMethod("Count");
			
			bool result = methodElement.IsGetter;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsGetter_MethodIsPropertyGetterButAttributeValueIsNotValidBooleanValue_ReturnsFalse()
		{
			CreatePropertyGetterWithInvalidAttributeValue("Count");
			
			bool result = methodElement.IsGetter;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsProperty_MethodIsPropertyGetter_ReturnsTrue()
		{
			CreatePropertyGetterMethod("Count");
			
			bool result = methodElement.IsProperty;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsSetter_MethodIsNotProperty_ReturnsFalse()
		{
			CreateMethod("MyMethod");
			
			bool result = methodElement.IsSetter;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsSetter_MethodIsPropertySetter_ReturnsTrue()
		{
			CreatePropertySetterMethod("Count");
			
			bool result = methodElement.IsSetter;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsProperty_MethodIsPropertySetter_ReturnsTrue()
		{
			CreatePropertySetterMethod("Count");
			
			bool result = methodElement.IsProperty;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsSetter_MethodIsPropertySetterButAttributeValueIsNotValidBooleanValue_ReturnsFalse()
		{
			CreatePropertySetterWithInvalidAttributeValue("Count");
			
			bool result = methodElement.IsSetter;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void MethodName_ElementHasFullMethodSignature_ReturnsMethodName()
		{
			CreateMethodWithSignature("System.Void MyClass::MyMethod(System.Int32)");
			
			string methodName = methodElement.MethodName;
			
			Assert.AreEqual("MyMethod", methodName);
		}
		
		[Test]
		public void MethodName_ElementHasNoNameElement_ReturnsEmptyString()
		{
			CreateMethodElementThatHasNoNameElement();
			
			string methodName = methodElement.MethodName;
			
			Assert.AreEqual(String.Empty, methodName);
		}
	}
}
