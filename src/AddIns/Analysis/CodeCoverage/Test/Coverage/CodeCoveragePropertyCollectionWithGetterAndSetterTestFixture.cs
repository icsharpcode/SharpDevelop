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
using System.Reflection;
using System.Xml.Linq;

using ICSharpCode.CodeCoverage;
using ICSharpCode.CodeCoverage.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Tests that two CodeCoverageMethod classes that are getter and setter for one property are returned
	/// as a single CodeCoverageProperty from the CodeCoveragePropertyCollection class.
	/// </summary>
	[TestFixture]
	public class CodeCoveragePropertyCollectionWithGetterAndSetterTestFixture
	{
		CodeCoveragePropertyCollection properties;
		CodeCoverageMethod getterMethod;
		CodeCoverageMethod setterMethod;
		CodeCoverageProperty property;
		
		XElement CreatePropertySetter(string className, string propertyName)
		{
			return CodeCoverageMethodXElementBuilder.CreatePropertySetter(className, propertyName, "System.Int32");
		}
		
		XElement CreatePropertyGetter(string className, string propertyName)
		{
			return CodeCoverageMethodXElementBuilder.CreatePropertyGetter(className, propertyName, "System.Int32");
		}
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			properties = new CodeCoveragePropertyCollection();
			XElement getterElement = CreatePropertyGetter("MyTests", "Count");
			getterMethod = new CodeCoverageMethod("MyTests", getterElement);
			XElement setterElement = CreatePropertySetter("MyTests", "Count");
			setterMethod = new CodeCoverageMethod("MyTests", setterElement);
			
			properties.Add(getterMethod);
			properties.Add(setterMethod);
			
			if (properties.Count > 0) {
				property = properties[0];
			}
		}
		
		[Test]
		public void OneProperty()
		{
			Assert.AreEqual(1, properties.Count);
		}
		
		[Test]
		public void PropertyNameIsCount()
		{
			Assert.AreEqual("Count", property.Name);
		}
	}
}
