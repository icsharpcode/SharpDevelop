// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class IndexerDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpIndexerDeclarationTest()
		{
			PropertyDeclaration id = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int this[int a, string b] { get { } set { } }");
			Assert.AreEqual(2, id.Parameters.Count);
			Assert.IsTrue(id.HasGetRegion, "No get region found!");
			Assert.IsTrue(id.HasSetRegion, "No set region found!");
			Assert.IsTrue(id.IsIndexer, "No Default modifier set!");
		}
		
		[Test]
		public void CSharpIndexerImplementingInterfaceTest()
		{
			PropertyDeclaration id = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface.this[int a, string b] { get { } set { } }");
			Assert.AreEqual(2, id.Parameters.Count);
			Assert.IsTrue(id.HasGetRegion, "No get region found!");
			Assert.IsTrue(id.HasSetRegion, "No set region found!");
			Assert.IsTrue(id.IsIndexer, "No Default modifier set!");
			
			Assert.AreEqual("MyInterface", id.InterfaceImplementations[0].InterfaceType.Type);
		}
		
		[Test]
		public void CSharpIndexerImplementingGenericInterfaceTest()
		{
			PropertyDeclaration id = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface<string>.this[int a, string b] { get { } set { } }");
			Assert.AreEqual(2, id.Parameters.Count);
			Assert.IsTrue(id.HasGetRegion, "No get region found!");
			Assert.IsTrue(id.HasSetRegion, "No set region found!");
			Assert.IsTrue(id.IsIndexer, "No Default modifier set!");
			
			Assert.AreEqual("MyInterface", id.InterfaceImplementations[0].InterfaceType.Type);
			Assert.AreEqual("System.String", id.InterfaceImplementations[0].InterfaceType.GenericTypes[0].Type);
		}
		#endregion
		
		#region VB.NET
		// no vb.net representation (indexers are properties named "item" in vb.net)
		#endregion
	}
}
