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
		ClassHelper helper;
		
		void CreateProjectContent()
		{
			helper = new ClassHelper();
		}
		
		void CreateClass(string name)
		{
			helper.CreateClass(name);
		}
		
		void CreateCodeType()
		{
			codeType = new CodeType(helper.ProjectContentHelper.ProjectContent, helper.Class);
		}
		
		void AddAttributeToClass(string name)
		{
			helper.AddAttributeToClass(name);
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
