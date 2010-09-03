// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class NamespaceDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleNamespaceTest()
		{
			string program = "namespace TestNamespace {\n" +
			                 "}\n";
			NamespaceDeclaration ns = ParseUtilCSharp.ParseGlobal<NamespaceDeclaration>(program);
			Assert.AreEqual("TestNamespace", ns.Name);
		}

		
		[Test]
		public void CSharpJuggedNamespaceTest()
		{
			string program = "namespace N1 {//TestNamespace\n" +
			                 "    namespace N2 {// Declares a namespace named N2 within N1.\n" +
			                 "    }\n" +
			                 "}\n";
			NamespaceDeclaration ns = ParseUtilCSharp.ParseGlobal<NamespaceDeclaration>(program);
			
			Assert.AreEqual("N1", ns.Name);
			
			Assert.IsTrue(ns.Children[0] is NamespaceDeclaration);
			
			ns = (NamespaceDeclaration)ns.Children[0];
			
			Assert.AreEqual("N2", ns.Name);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleNamespaceTest()
		{
			string program = "Namespace TestNamespace" + Environment.NewLine +
			                 "End Namespace" +Environment.NewLine;
			NamespaceDeclaration ns = ParseUtilVBNet.ParseGlobal<NamespaceDeclaration>(program);
			Assert.AreEqual("TestNamespace", ns.Name);
		}
		
		[Test]
		public void VBNetJuggedNamespaceTest()
		{
			string program = "Namespace N1 'TestNamespace\n" +
			                 "    Namespace N2   ' Declares a namespace named N2 within N1.\n" +
			                 "    End Namespace\n" +
			                 "End Namespace\n";
			
			NamespaceDeclaration ns = ParseUtilVBNet.ParseGlobal<NamespaceDeclaration>(program);
			
			Assert.AreEqual("N1", ns.Name);
			
			Assert.IsTrue(ns.Children[0] is NamespaceDeclaration);
			
			ns = (NamespaceDeclaration)ns.Children[0];
			
			Assert.AreEqual("N2", ns.Name);
		}
		#endregion
	}
}
