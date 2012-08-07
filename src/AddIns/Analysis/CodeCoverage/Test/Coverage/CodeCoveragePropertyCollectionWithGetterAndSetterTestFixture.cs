// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
