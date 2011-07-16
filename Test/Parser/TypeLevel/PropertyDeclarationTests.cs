// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class PropertyDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleGetSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { get {} set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		
		[Test]
		public void CSharpGetSetPropertyDeclarationWithAccessorModifiers()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { private get {} protected internal set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		
		[Test]
		public void CSharpSimpleGetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(!pd.HasSetRegion);
		}
		
		[Test]
		public void CSharpSimpleSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(!pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		
		void CSharpPropertyRegionTest(bool parseMethodBodies)
		{
			const string code = "class T {\n\tint Prop {\n\t\tget { return f; }\n\t\tset { f = value; }\n\t}\n}\n";
			int line2Pos = code.IndexOf("\tint Prop");
			int line3Pos = code.IndexOf("\t\tget");
			int line4Pos = code.IndexOf("\t\tset");
			
			IParser p = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(code));
			p.ParseMethodBodies = parseMethodBodies;
			p.Parse();
			PropertyDeclaration pd = (PropertyDeclaration)p.CompilationUnit.Children[0].Children[0];
			Assert.AreEqual(new Location(code.IndexOf("{\n\t\tget") - line2Pos + 1, 2), pd.BodyStart);
			Assert.AreEqual(new Location(3, 5), pd.BodyEnd);
			Assert.AreEqual(new Location(code.IndexOf("{ return") - line3Pos + 1, 3), pd.GetRegion.Block.StartLocation);
			Assert.AreEqual(new Location(code.IndexOf("}\n\t\tset") + 1 - line3Pos + 1, 3), pd.GetRegion.Block.EndLocation);
			Assert.AreEqual(new Location(code.IndexOf("{ f =") - line4Pos + 1, 4), pd.SetRegion.Block.StartLocation);
			Assert.AreEqual(new Location(code.IndexOf("}\n\t}") + 1 - line4Pos + 1, 4), pd.SetRegion.Block.EndLocation);
		}
		
		[Test]
		public void CSharpPropertyRegionTest()
		{
			CSharpPropertyRegionTest(true);
		}
		
		[Test]
		public void CSharpPropertyRegionTestSkipParseMethodBodies()
		{
			CSharpPropertyRegionTest(false);
		}
		
		[Test]
		public void CSharpPropertyImplementingInterfaceTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface.MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(!pd.HasSetRegion);
			
			Assert.AreEqual("MyInterface", pd.InterfaceImplementations[0].InterfaceType.Type);
		}
		
		[Test]
		public void CSharpPropertyImplementingGenericInterfaceTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface<string>.MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(!pd.HasSetRegion);
			
			Assert.AreEqual("MyInterface", pd.InterfaceImplementations[0].InterfaceType.Type);
			Assert.AreEqual("System.String", pd.InterfaceImplementations[0].InterfaceType.GenericTypes[0].Type);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleGetSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("Property MyProperty As Integer \n Get \n End Get \n Set \n End Set\nEnd Property");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		
		[Test]
		public void VBNetSimpleGetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("ReadOnly Property MyProperty \nGet\nEnd Get\nEnd Property");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsFalse(pd.HasSetRegion);
			Assert.IsTrue((pd.Modifier & Modifiers.ReadOnly) == Modifiers.ReadOnly);
		}
		
		[Test]
		public void VBNetSimpleSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("WriteOnly Property MyProperty \n Set\nEnd Set\nEnd Property ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
			Assert.IsTrue((pd.Modifier & Modifiers.WriteOnly) == Modifiers.WriteOnly);
		}
		
		[Test]
		public void VBNetAutoPropertyTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("Property MyProperty");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
			Assert.AreEqual(pd.Initializer, Expression.Null);
		}
		
		[Test]
		public void VBNetReadOnlyAutoPropertyTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("ReadOnly Property MyProperty");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsFalse(pd.HasSetRegion);
			Assert.AreEqual(pd.Initializer, Expression.Null);
		}
		
		[Test]
		public void VBNetWriteOnlyAutoPropertyTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("WriteOnly Property MyProperty");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
			Assert.AreEqual(pd.Initializer, Expression.Null);
		}
		
		[Test]
		public void VBNetSimpleInitializerAutoPropertyTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("Property MyProperty = 5");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
			Assert.AreEqual(pd.Initializer.ToString(), new PrimitiveExpression(5).ToString());
		}
		
		[Test]
		public void VBNetSimpleInitializerAutoPropertyWithTypeTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("Property MyProperty As Integer = 5");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.AreEqual("System.Int32", pd.TypeReference.Type);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
			Assert.AreEqual(pd.Initializer.ToString(), new PrimitiveExpression(5).ToString());
		}
		
		[Test]
		public void VBNetSimpleObjectInitializerAutoPropertyTest()
		{
			PropertyDeclaration pd = ParseUtilVBNet.ParseTypeMember<PropertyDeclaration>("Property MyProperty As New List");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.AreEqual("List", pd.TypeReference.Type);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
			Assert.AreEqual(pd.Initializer.ToString(), new ObjectCreateExpression(new TypeReference("List"), null).ToString());
		}
		#endregion
	}
}
