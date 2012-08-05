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
		
		XElement CreateSetterMethod(string methodSignature)
		{
			return CodeCoverageMethodXElementBuilder.CreateSetterMethod(methodSignature);
		}
		
		XElement CreateGetterMethod(string methodSignature)
		{
			return CodeCoverageMethodXElementBuilder.CreateGetterMethod(methodSignature);
		}
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			properties = new CodeCoveragePropertyCollection();
			XElement getterElement = CreateGetterMethod("System.Int32 MyTests::get_Count()");
			getterMethod = new CodeCoverageMethod("MyTests", getterElement);
			XElement setterElement = CreateSetterMethod("System.Void MyTests::set_Count(System.Int32)");
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
