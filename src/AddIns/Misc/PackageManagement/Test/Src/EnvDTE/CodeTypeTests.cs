// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeTypeTests
	{
		CodeType codeType;
		ProjectContentHelper helper;
		IClass fakeClass;
		
		void CreateProjectContent()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateClass(string name)
		{
			fakeClass = helper.AddClassToProjectContent(name);
		}
		
		void CreateCodeType()
		{
			codeType = new CodeType(helper.FakeProjectContent, fakeClass);
		}
		
		void AddAttributeToClass(string name)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(name);
			attributeHelper.AddAttributeToClass(fakeClass);
		}
		
		[Test]
		public void Attributes_ClassHasOneAttribute_ReturnsOneAttribute()
		{
			CreateProjectContent();
			CreateClass("TestClass");
			AddAttributeToClass("TestAttribute");
			CreateCodeType();
			
			CodeElements attributes = codeType.Attributes;
			
			CodeAttribute2 attribute = attributes.Item(1) as CodeAttribute2;
			
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("Test", attribute.Name);
		}
	}
}
