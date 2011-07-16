// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class OperatorDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpImplictOperatorDeclarationTest()
		{
			OperatorDeclaration od = ParseUtilCSharp.ParseTypeMember<OperatorDeclaration>("public static implicit operator double(MyObject f)  { return 0.5d; }");
			Assert.IsTrue(od.IsConversionOperator);
			Assert.AreEqual(1, od.Parameters.Count);
			Assert.AreEqual(ConversionType.Implicit, od.ConversionType);
			Assert.AreEqual("System.Double", od.TypeReference.Type);
		}
		
		[Test]
		public void CSharpExplicitOperatorDeclarationTest()
		{
			OperatorDeclaration od = ParseUtilCSharp.ParseTypeMember<OperatorDeclaration>("public static explicit operator double(MyObject f)  { return 0.5d; }");
			Assert.IsTrue(od.IsConversionOperator);
			Assert.AreEqual(1, od.Parameters.Count);
			Assert.AreEqual(ConversionType.Explicit, od.ConversionType);
			Assert.AreEqual("System.Double", od.TypeReference.Type);
		}
		
		[Test]
		public void CSharpBinaryPlusOperatorDeclarationTest()
		{
			OperatorDeclaration od = ParseUtilCSharp.ParseTypeMember<OperatorDeclaration>("public static MyObject operator +(MyObject a, MyObject b)  {}");
			Assert.IsTrue(!od.IsConversionOperator);
			Assert.AreEqual(OverloadableOperatorType.Add, od.OverloadableOperator);
			Assert.AreEqual(2, od.Parameters.Count);
			Assert.AreEqual("MyObject", od.TypeReference.Type);
			Assert.AreEqual("op_Addition", od.Name);
		}
		
		[Test]
		public void CSharpUnaryPlusOperatorDeclarationTest()
		{
			OperatorDeclaration od = ParseUtilCSharp.ParseTypeMember<OperatorDeclaration>("public static MyObject operator +(MyObject a)  {}");
			Assert.IsTrue(!od.IsConversionOperator);
			Assert.AreEqual(OverloadableOperatorType.UnaryPlus, od.OverloadableOperator);
			Assert.AreEqual(1, od.Parameters.Count);
			Assert.AreEqual("MyObject", od.TypeReference.Type);
			Assert.AreEqual("op_UnaryPlus", od.Name);
		}
		#endregion
		
		#region VB.NET
		
		[Test]
		public void VBNetImplictOperatorDeclarationTest()
		{
			string programm = @"Public Shared Operator + (ByVal v As Complex) As Complex
					Return v
				End Operator";
			
			OperatorDeclaration od = ParseUtilVBNet.ParseTypeMember<OperatorDeclaration>(programm);
			Assert.IsFalse(od.IsConversionOperator);
			Assert.AreEqual(1, od.Parameters.Count);
			Assert.AreEqual(ConversionType.None, od.ConversionType);
			Assert.AreEqual("Complex", od.TypeReference.Type);
		}
		#endregion 
	}
}
