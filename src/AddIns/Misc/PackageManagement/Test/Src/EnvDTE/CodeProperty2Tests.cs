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
		
		[Test]
		public void Name_PropertyCalledMyProperty_ReturnsMyProperty()
		{
			helper.CreateProperty("MyProperty");
			CreateCodeProperty2();
			
			string name = property.Name;
			
			Assert.AreEqual("MyProperty", name);
		}
		
		[Test]
		public void Parent_Class1ContainsProperty_ReturnsClass1()
		{
			helper.CreateProperty("MyProperty");
			helper.AddParentClass("Tests.Class1");
			CreateCodeProperty2();
			
			CodeClass parentClass = property.Parent;
			
			Assert.AreEqual("Tests.Class1", parentClass.FullName);
		}
		
		[Test]
		public void Access_PublicProperty_AccessIsPublic()
		{
			helper.CreatePublicProperty("MyProperty");
			CreateCodeProperty2();
			
			vsCMAccess access = property.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateProperty_AccessIsPrivate()
		{
			helper.CreatePrivateProperty("MyProperty");
			CreateCodeProperty2();
			
			vsCMAccess access = property.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void ReadWrite_PropertyHasGetterAndSetter_ReturnsReadWriteProperty()
		{
			helper.CreatePublicProperty("MyProperty");
			helper.HasGetterAndSetter();
			CreateCodeProperty2();
			
			vsCMPropertyKind kind = property.ReadWrite;
			
			Assert.AreEqual(vsCMPropertyKind.vsCMPropertyKindReadWrite, kind);
		}
		
		[Test]
		public void ReadWrite_PropertyHasGetterOnly_ReturnsReadOnlyProperty()
		{
			helper.CreatePublicProperty("MyProperty");
			helper.HasGetterOnly();
			CreateCodeProperty2();
			
			vsCMPropertyKind kind = property.ReadWrite;
			
			Assert.AreEqual(vsCMPropertyKind.vsCMPropertyKindReadOnly, kind);
		}
		
		[Test]
		public void ReadWrite_PropertyHasSetterOnly_ReturnsWriteOnlyProperty()
		{
			helper.CreatePublicProperty("MyProperty");
			helper.HasSetterOnly();
			CreateCodeProperty2();
			
			vsCMPropertyKind kind = property.ReadWrite;
			
			Assert.AreEqual(vsCMPropertyKind.vsCMPropertyKindWriteOnly, kind);
		}
	}
}
