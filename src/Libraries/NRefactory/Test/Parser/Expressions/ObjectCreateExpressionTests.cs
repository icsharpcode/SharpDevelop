// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class ObjectCreateExpressionTests
	{
		void CheckSimpleObjectCreateExpression(ObjectCreateExpression oce)
		{
			Assert.AreEqual("MyObject", oce.CreateType.Type);
			Assert.AreEqual(3, oce.Parameters.Count);
			
			for (int i = 0; i < oce.Parameters.Count; ++i) {
				Assert.IsTrue(oce.Parameters[i] is PrimitiveExpression);
			}
		}
		
		#region C#
		[Test]
		public void CSharpSimpleObjectCreateExpressionTest()
		{
			CheckSimpleObjectCreateExpression(ParseUtilCSharp.ParseExpression<ObjectCreateExpression>("new MyObject(1, 2, 3)"));
		}
		
		[Test]
		public void CSharpNullableObjectCreateExpressionTest()
		{
			ObjectCreateExpression oce = ParseUtilCSharp.ParseExpression<ObjectCreateExpression>("new IntPtr?(1)");
			Assert.AreEqual("System.Nullable", oce.CreateType.SystemType);
			Assert.AreEqual(1, oce.CreateType.GenericTypes.Count);
			Assert.AreEqual("IntPtr", oce.CreateType.GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpInvalidNestedObjectCreateExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("WriteLine(new MyObject(1, 2, 3,))", true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is ObjectCreateExpression);
			CheckSimpleObjectCreateExpression((ObjectCreateExpression)expr.Arguments[0]);
		}
		
		[Test]
		public void CSharpInvalidTypeArgumentListObjectCreateExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("WriteLine(new SomeGenericType<int, >())", true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is ObjectCreateExpression);
			TypeReference typeRef = ((ObjectCreateExpression)expr.Arguments[0]).CreateType;
			Assert.AreEqual("SomeGenericType", typeRef.Type);
			Assert.AreEqual(1, typeRef.GenericTypes.Count);
			Assert.AreEqual("int", typeRef.GenericTypes[0].Type);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleObjectCreateExpressionTest()
		{
			CheckSimpleObjectCreateExpression(ParseUtilVBNet.ParseExpression<ObjectCreateExpression>("New MyObject(1, 2, 3)"));
		}
		
		[Test]
		public void VBNetInvalidTypeArgumentListObjectCreateExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = ParseUtilVBNet.ParseExpression<InvocationExpression>("WriteLine(New SomeGenericType(Of Integer, )())", true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is ObjectCreateExpression);
			TypeReference typeRef = ((ObjectCreateExpression)expr.Arguments[0]).CreateType;
			Assert.AreEqual("SomeGenericType", typeRef.Type);
			Assert.AreEqual(1, typeRef.GenericTypes.Count);
			Assert.AreEqual("Integer", typeRef.GenericTypes[0].Type);
		}
		#endregion
	}
}
