// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class CodeCoveragePropertyTestFixture
	{
		CodeCoverageMethod getter;
		CodeCoverageMethod setter;
		
		[SetUp]
		public void Init()
		{
			getter = new CodeCoverageMethod("get_Count", "MyTest", MethodAttributes.SpecialName);
			setter = new CodeCoverageMethod("set_Count", "MyTest", MethodAttributes.SpecialName);
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
