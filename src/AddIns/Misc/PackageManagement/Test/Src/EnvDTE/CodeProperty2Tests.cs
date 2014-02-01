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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeProperty2Tests : CodeModelTestBase
	{
		CodeProperty2 property;
		
		void CreateCodeProperty2(string code)
		{
			AddCodeFile("class.cs", code);
			IProperty member = assemblyModel
				.TopLevelTypeDefinitions
				.First()
				.Members
				.First()
				.Resolve() as IProperty;
			
			property = new CodeProperty2(codeModelContext, member);
		}
		
		[Test]
		public void Attributes_PropertyHasOneAttribute_ReturnsOneAttribute()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    [System.Obsolete]\r\n" +
				"    public int MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.CodeElements attributes = property.Attributes;
			
			CodeAttribute2 attribute = attributes.Item(1) as CodeAttribute2;
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("System.ObsoleteAttribute", attribute.FullName);
		}
		
		[Test]
		public void Name_PropertyCalledMyProperty_ReturnsMyProperty()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get; set; }\r\n" +
				"}");
			
			string name = property.Name;
			
			Assert.AreEqual("MyProperty", name);
		}
		
		[Test]
		public void Parent_Class1ContainsProperty_ReturnsClass1()
		{
			CreateCodeProperty2(
				"namespace Tests {\r\n" +
				"    class Class1 {\r\n" +
				"        public int MyProperty { get; set; }\r\n" +
				"    }\r\n" +
				"}");
			
			global::EnvDTE.CodeClass parentClass = property.Parent;
			
			Assert.AreEqual("Tests.Class1", parentClass.FullName);
		}
		
		[Test]
		public void Access_PublicProperty_AccessIsPublic()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMAccess access = property.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateProperty_AccessIsPrivate()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    private int MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMAccess access = property.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void ReadWrite_PropertyHasGetterAndSetter_ReturnsReadWriteProperty()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMPropertyKind kind = property.ReadWrite;
			
			Assert.AreEqual(global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindReadWrite, kind);
		}
		
		[Test]
		public void ReadWrite_PropertyHasGetterOnly_ReturnsReadOnlyProperty()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get { return 0; } }\r\n" +
				"}");
			
			global::EnvDTE.vsCMPropertyKind kind = property.ReadWrite;
			
			Assert.AreEqual(global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindReadOnly, kind);
		}
		
		[Test]
		public void ReadWrite_PropertyHasSetterOnly_ReturnsWriteOnlyProperty()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { set { } }\r\n" +
				"}");
			
			global::EnvDTE.vsCMPropertyKind kind = property.ReadWrite;
			
			Assert.AreEqual(global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindWriteOnly, kind);
		}
		
		[Test]
		public void Parameters_PropertyIsIndexerWithOneParameter_ReturnsOneParameter()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int this[int item] { get { return 0; } }\r\n" +
				"}");
			
			global::EnvDTE.CodeElements parameters = property.Parameters;
			
			CodeParameter parameter = parameters.FirstCodeParameterOrDefault();
			Assert.AreEqual(1, parameters.Count);
			Assert.AreEqual("item", parameter.Name);
		}
		
		[Test]
		public void Getter_PublicGetter_ReturnsPublicGetterCodeFunction()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get { return 0; } }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction getter = property.Getter;
			global::EnvDTE.vsCMAccess access = getter.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Getter_PrivateGetter_ReturnsPrivateGetterCodeFunction()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    private int MyProperty { get { return 0; } }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction getter = property.Getter;
			global::EnvDTE.vsCMAccess access = getter.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void Getter_NoGetter_ReturnsNull()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { set { } }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction getter = property.Getter;
			
			Assert.IsNull(getter);
		}
		
		[Test]
		public void Getter_PublicPropertyButPrivateGetter_ReturnsPrivateGetterCodeFunction()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { private get; public set; }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction getter = property.Getter;
			global::EnvDTE.vsCMAccess access = getter.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void Setter_PublicSetter_ReturnsPublicSetterCodeFunction()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { set { } }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction setter = property.Setter;
			global::EnvDTE.vsCMAccess access = setter.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Setter_PrivateSetter_ReturnsPrivateSetterCodeFunction()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    private int MyProperty { set { } }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction setter = property.Setter;
			global::EnvDTE.vsCMAccess access = setter.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void Setter_NoSetter_ReturnsNull()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get { return 0; } }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction setter = property.Setter;
			
			Assert.IsNull(setter);
		}
		
		[Test]
		public void Setter_PublicPropertyButPrivateSetter_ReturnsPrivateSetterCodeFunction()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get; private set; }\r\n" +
				"}");
			
			global::EnvDTE.CodeFunction setter = property.Setter;
			global::EnvDTE.vsCMAccess access = setter.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void Type_PropertyTypeIsSystemString_ReturnsSystemString()
		{
			CreateCodeProperty2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    public string MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.CodeTypeRef typeRef = property.Type;
			string fullName = typeRef.AsFullName;
			
			Assert.AreEqual("System.String", fullName);
		}
		
		[Test]
		public void Type_PropertyTypeIsSystemString_TypesParentIsProperty()
		{
			CreateCodeProperty2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    public string MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.CodeTypeRef typeRef = property.Type;
			global::EnvDTE.CodeElement parent = typeRef.Parent;
			
			Assert.AreEqual(property, parent);
		}
		
		[Test]
		public void Type_PropertyTypeIsSystemString_TypeRefTypeInfoLocationIsExternal()
		{
			CreateCodeProperty2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    public string MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.CodeTypeRef typeRef = property.Type;
			global::EnvDTE.vsCMInfoLocation location = typeRef.CodeType.InfoLocation;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, location);
		}
		
		[Test]
		public void Type_PropertyTypeExistsInProject_TypeRefTypeInfoLocationIsProject()
		{
			CreateCodeProperty2(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    public MyType MyProperty { get; set; }\r\n" +
				"}\r\n" +
				"public class MyType {}");
			
			global::EnvDTE.CodeTypeRef typeRef = property.Type;
			global::EnvDTE.vsCMInfoLocation location = typeRef.CodeType.InfoLocation;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, location);
		}
		
		[Test]
		public void Kind_PublicProperty_ReturnsProperty()
		{
			CreateCodeProperty2(
				"class MyClass {\r\n" +
				"    public int MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMElement kind = property.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementProperty, kind);
		}
	}
}
