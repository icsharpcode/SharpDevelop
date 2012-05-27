// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeProperty2Tests
	{
		CodeProperty2 property;
		PropertyHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new PropertyHelper();
		}
		
		void CreateCodeProperty2()
		{
			property = new CodeProperty2(helper.Property);
		}
		
		[Test]
		public void Attributes_PropertyHasOneAttribute_ReturnsOneAttribute()
		{
			helper.CreateProperty("MyProperty");
			helper.AddAttribute("Tests.TestAttribute", "TestAttribute");
			CreateCodeProperty2();
			
			CodeElements attributes = property.Attributes;
			
			CodeAttribute2 attribute = attributes.Item(1) as CodeAttribute2;
			
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("Tests.TestAttribute", attribute.FullName);
		}
	}
}
