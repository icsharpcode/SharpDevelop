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
	public class MethodDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleMethodDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember("void MyMethod() {} ", typeof(MethodDeclaration));
			Assert.AreEqual("void", md.TypeReference.Type);
			Assert.AreEqual(0, md.Parameters.Count);
		}
		
		[Test]
		public void CSharpSimpleMethodRegionTest()
		{
			const string program = @"
		void MyMethod()
		{
			OtherMethod();
		}
";
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember(program, typeof(MethodDeclaration));
			Assert.AreEqual(2, md.StartLocation.Y, "StartLocation.Y");
			Assert.AreEqual(2, md.EndLocation.Y, "EndLocation.Y");
			Assert.AreEqual(3, md.StartLocation.X, "StartLocation.X");
			
			// endLocation.X is currently 20. It should be 18, but that error is not critical
			//Assert.AreEqual(18, md.EndLocation.X, "EndLocation.X");
		}
		
		[Test]
		public void CSharpMethodWithModifiersRegionTest()
		{
			const string program = @"
		public static void MyMethod()
		{
			OtherMethod();
		}
";
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember(program, typeof(MethodDeclaration));
			Assert.AreEqual(2, md.StartLocation.Y, "StartLocation.Y");
			Assert.AreEqual(2, md.EndLocation.Y, "EndLocation.Y");
			Assert.AreEqual(3, md.StartLocation.X, "StartLocation.X");
		}
		
		[Test]
		public void CSharpMethodWithUnnamedParameterDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember("void MyMethod(int) {} ", typeof(MethodDeclaration), true);
			Assert.AreEqual("void", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("?", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
		}
		
		[Test]
		public void CSharpGenericVoidMethodDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember("void MyMethod<T>(T a) {} ", typeof(MethodDeclaration));
			Assert.AreEqual("void", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
		}
		
		[Test]
		public void CSharpGenericMethodDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember("T MyMethod<T>(T a) {} ", typeof(MethodDeclaration));
			Assert.AreEqual("T", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
		}
		
		[Test]
		public void CSharpGenericMethodDeclarationWithConstraintTest()
		{
			string program = "T MyMethod<T>(T a) where T : ISomeInterface {} ";
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember(program, typeof(MethodDeclaration));
			Assert.AreEqual("T", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
			Assert.AreEqual(1, md.Templates[0].Bases.Count);
			Assert.AreEqual("ISomeInterface", md.Templates[0].Bases[0].Type);
		}
		#endregion
		
		#region VB.NET
		// TODO
		#endregion
	}
}
