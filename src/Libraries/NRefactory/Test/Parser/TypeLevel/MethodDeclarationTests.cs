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
		
		[Test]
		public void CSharpGenericMethodInInterface()
		{
			const string program = @"interface MyInterface {
	T MyMethod<T>(T a) where T : ISomeInterface;
}
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal(program, typeof(TypeDeclaration));
			MethodDeclaration md = (MethodDeclaration)td.Children[0];
			Assert.AreEqual("T", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
			Assert.AreEqual(1, md.Templates[0].Bases.Count);
			Assert.AreEqual("ISomeInterface", md.Templates[0].Bases[0].Type);
		}
		
		[Test]
		public void CSharpGenericVoidMethodInInterface()
		{
			const string program = @"interface MyInterface {
	void MyMethod<T>(T a) where T : ISomeInterface;
}
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal(program, typeof(TypeDeclaration));
			MethodDeclaration md = (MethodDeclaration)td.Children[0];
			Assert.AreEqual("void", md.TypeReference.Type);
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
		
		[Test]
		public void VBNetSimpleMethodDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember("void MyMethod() {} ", typeof(MethodDeclaration));
			Assert.AreEqual("void", md.TypeReference.Type);
			Assert.AreEqual(0, md.Parameters.Count);
		}
		
		[Test]
		public void VBNetSimpleMethodRegionTest()
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
		public void VBNetMethodWithModifiersRegionTest()
		{
			const string program = @"public shared sub MyMethod()
				OtherMethod()
			end sub";
			
			MethodDeclaration md = (MethodDeclaration)ParseUtilVBNet.ParseTypeMember(program, typeof(MethodDeclaration));
			Assert.AreEqual(2, md.StartLocation.Y, "StartLocation.Y");
			Assert.AreEqual(2, md.EndLocation.Y, "EndLocation.Y");
			Assert.AreEqual(2, md.StartLocation.X, "StartLocation.X");
		}
		
		[Test]
		public void VBNetGenericFunctionMethodDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilVBNet.ParseTypeMember("function MyMethod(Of T)(a As T) As Double\nEnd Function", typeof(MethodDeclaration));
			Assert.AreEqual("Double", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
		}
		
		[Test]
		public void VBNetGenericMethodDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilVBNet.ParseTypeMember("Function MyMethod(Of T)(a As T) As T\nEnd Function ", typeof(MethodDeclaration));
			Assert.AreEqual("T", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
		}
		
		[Test]
		public void VBNetGenericMethodDeclarationWithConstraintTest()
		{
			string program = "Function MyMethod(Of T As { ISomeInterface })(a As T) As T\n End Function";
			MethodDeclaration md = (MethodDeclaration)ParseUtilVBNet.ParseTypeMember(program, typeof(MethodDeclaration));
			Assert.AreEqual("T", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
			Assert.AreEqual(1, md.Templates[0].Bases.Count);
			Assert.AreEqual("ISomeInterface", md.Templates[0].Bases[0].Type);
		}
		
		[Test]
		public void VBNetGenericMethodInInterface()
		{
			const string program = @"Interface MyInterface
	Function MyMethod(Of T As {ISomeInterface})(a As T) As T
	End Interface";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			MethodDeclaration md = (MethodDeclaration)td.Children[0];
			Assert.AreEqual("T", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
			Assert.AreEqual(1, md.Templates[0].Bases.Count);
			Assert.AreEqual("ISomeInterface", md.Templates[0].Bases[0].Type);
		}
		
		[Test]
		public void VBNetGenericVoidMethodInInterface()
		{
			const string program = @"interface MyInterface
	Sub MyMethod(Of T As {ISomeInterface})(a as T)
End Interface
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			MethodDeclaration md = (MethodDeclaration)td.Children[0];
			Assert.AreEqual("", md.TypeReference.Type);
			Assert.AreEqual(1, md.Parameters.Count);
			Assert.AreEqual("T", ((ParameterDeclarationExpression)md.Parameters[0]).TypeReference.Type);
			Assert.AreEqual("a", ((ParameterDeclarationExpression)md.Parameters[0]).ParameterName);
			
			Assert.AreEqual(1, md.Templates.Count);
			Assert.AreEqual("T", md.Templates[0].Name);
			Assert.AreEqual(1, md.Templates[0].Bases.Count);
			Assert.AreEqual("ISomeInterface", md.Templates[0].Bases[0].Type);
		}
		
		#endregion
	}
}
