/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			// TODO
		#endregion 
	}
}
