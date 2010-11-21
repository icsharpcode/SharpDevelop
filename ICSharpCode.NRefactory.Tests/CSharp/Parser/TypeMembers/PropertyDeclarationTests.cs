// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.TypeMembers
{
	[TestFixture]
	public class PropertyDeclarationTests
	{
		[Test]
		public void SimpleGetSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { get {} set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsNotNull(pd.GetAccessor);
			Assert.IsNotNull(pd.SetAccessor);
		}
		
		[Test]
		public void GetSetPropertyDeclarationWithAccessorModifiers()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { private get {} protected internal set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsNotNull(pd.GetAccessor);
			Assert.IsNotNull(pd.SetAccessor);
		}
		
		[Test]
		public void SimpleGetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsNotNull(pd.GetAccessor);
			Assert.IsNull(pd.SetAccessor);
		}
		
		[Test]
		public void SimpleSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsNull(pd.GetAccessor);
			Assert.IsNotNull(pd.SetAccessor);
		}
		
		[Test]
		public void PropertyRegionTest()
		{
			const string code = "class T {\n\tint Prop {\n\t\tget { return f; }\n\t\tset { f = value; }\n\t}\n}\n";
			int line2Pos = code.IndexOf("\tint Prop");
			int line3Pos = code.IndexOf("\t\tget");
			int line4Pos = code.IndexOf("\t\tset");
			
			CSharpParser parser = new CSharpParser();
			CompilationUnit cu = parser.Parse(new StringReader(code));
			PropertyDeclaration pd = (PropertyDeclaration)cu.Children.Single().GetChildByRole(TypeDeclaration.Roles.Member);
			Assert.AreEqual(new DomLocation(2, code.IndexOf("{\n\t\tget") - line2Pos + 1), pd.LBrace.StartLocation);
			Assert.AreEqual(new DomLocation(5, 3), pd.EndLocation);
			Assert.AreEqual(new DomLocation(3, code.IndexOf("{ return") - line3Pos + 1), pd.GetAccessor.Body.StartLocation);
			Assert.AreEqual(new DomLocation(3, code.IndexOf("}\n\t\tset") + 1 - line3Pos + 1), pd.GetAccessor.Body.EndLocation);
			Assert.AreEqual(new DomLocation(4, code.IndexOf("{ f =") - line4Pos + 1), pd.SetAccessor.Body.StartLocation);
			Assert.AreEqual(new DomLocation(4, code.IndexOf("}\n\t}") + 1 - line4Pos + 1), pd.SetAccessor.Body.EndLocation);
		}
		
		[Test, Ignore("type references not yet implemented")]
		public void PropertyImplementingInterfaceTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface.MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsNotNull(pd.GetAccessor);
			Assert.IsNull(pd.SetAccessor);
			
			Assert.AreEqual("MyInterface", pd.PrivateImplementationType);
		}
		
		[Test, Ignore("type references not yet implemented")]
		public void PropertyImplementingGenericInterfaceTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface<string>.MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsNotNull(pd.GetAccessor);
			Assert.IsNull(pd.SetAccessor);
			
			throw new NotImplementedException();
			//Assert.AreEqual("MyInterface", pd.InterfaceImplementations[0].InterfaceType.Type);
			//Assert.AreEqual("System.String", pd.InterfaceImplementations[0].InterfaceType.GenericTypes[0].Type);
		}
	}
}
