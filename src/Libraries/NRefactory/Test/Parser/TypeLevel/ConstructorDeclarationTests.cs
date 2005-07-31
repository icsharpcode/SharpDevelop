// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;

using NUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class ConstructorDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpConstructorDeclarationTest1()
		{
			ConstructorDeclaration cd = (ConstructorDeclaration)ParseUtilCSharp.ParseTypeMember("MyClass() {}", typeof(ConstructorDeclaration));
			Assert.IsTrue(cd.ConstructorInitializer.IsNull);
		}
		
		[Test]
		public void CSharpConstructorDeclarationTest2()
		{
			ConstructorDeclaration cd = (ConstructorDeclaration)ParseUtilCSharp.ParseTypeMember("MyClass() : this(5) {}", typeof(ConstructorDeclaration));
			Assert.AreEqual(ConstructorInitializerType.This, cd.ConstructorInitializer.ConstructorInitializerType);
			Assert.AreEqual(1, cd.ConstructorInitializer.Arguments.Count);
		}
		
		[Test]
		public void CSharpConstructorDeclarationTest3()
		{
			ConstructorDeclaration cd = (ConstructorDeclaration)ParseUtilCSharp.ParseTypeMember("MyClass() : base(1, 2, 3) {}", typeof(ConstructorDeclaration));
			Assert.AreEqual(ConstructorInitializerType.Base, cd.ConstructorInitializer.ConstructorInitializerType);
			Assert.AreEqual(3, cd.ConstructorInitializer.Arguments.Count);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetConstructorDeclarationTest1()
		{
			string program = @"Sub New()
								End Sub";
			ConstructorDeclaration cd = (ConstructorDeclaration)ParseUtilVBNet.ParseTypeMember(program, typeof(ConstructorDeclaration));
			Assert.IsTrue(cd.ConstructorInitializer.IsNull);
		}
		
		[Test]
		public void VBNetConstructorDeclarationTest2()
		{
			ConstructorDeclaration cd = (ConstructorDeclaration)ParseUtilVBNet.ParseTypeMember("Sub New(x As Integer, Optional y As String) \nEnd Sub", typeof(ConstructorDeclaration));
			Assert.AreEqual(2, cd.Parameters.Count);
			Assert.AreEqual("Integer", cd.Parameters[0].TypeReference.Type);
			Assert.AreEqual("String", cd.Parameters[1].TypeReference.Type);
			Assert.AreEqual(ParamModifier.Optional, cd.Parameters[1].ParamModifier & ParamModifier.Optional);
		}
		#endregion 
	}
}
