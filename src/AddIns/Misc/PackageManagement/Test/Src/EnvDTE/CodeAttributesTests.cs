// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeAttributesTests
	{
		IClass fakeClass;
		CodeAttributes attributes;
		ProjectContentHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateCodeAttributes()
		{
			attributes = new CodeAttributes(fakeClass);
		}
		
		void CreateMSBuildClass()
		{
			fakeClass = helper.AddClassToProjectContent("MyClass");
		}
		
		void AddAttributeToClass(string name)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(name);
			attributeHelper.AddAttributeToClass(fakeClass);
		}
		
		List<CodeElement> GetEnumerator()
		{
			return attributes.ToList();
		}
		
		[Test]
		public void GetEnumerator_ClassHasOneAttribute_ReturnsOneAttribute()
		{
			CreateMSBuildClass();
			AddAttributeToClass("TestAttribute");
			CreateCodeAttributes();
		
			List<CodeElement> attributeList = GetEnumerator();
			
			CodeAttribute2 attribute = attributeList.FirstOrDefault() as CodeAttribute2;
			
			Assert.AreEqual(1, attributeList.Count);
			Assert.AreEqual("Test", attribute.Name);
		}
		
		[Test]
		public void Item_GetItemByNameWhenClassHasOneAttribute_ReturnsOneAttribute()
		{
			CreateMSBuildClass();
			AddAttributeToClass("TestAttribute");
			CreateCodeAttributes();
		
			CodeAttribute2 attribute = attributes.Item("Test") as CodeAttribute2;
			
			Assert.AreEqual("Test", attribute.Name);
		}
	}
}
