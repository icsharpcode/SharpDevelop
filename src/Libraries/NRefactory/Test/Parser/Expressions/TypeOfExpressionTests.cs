// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class TypeOfExpressionTests
	{
		#region C#
		[Test]
		public void CSharpSimpleTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyNamespace.N1.MyType)", typeof(TypeOfExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
		}
		
		[Test]
		public void CSharpGlobalTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(global::System.Console)", typeof(TypeOfExpression));
			Assert.AreEqual("System.Console", toe.TypeReference.Type);
		}
		
		[Test]
		public void CSharpPrimitiveTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(int)", typeof(TypeOfExpression));
			Assert.AreEqual("System.Int32", toe.TypeReference.SystemType);
		}
		
		[Test]
		public void CSharpVoidTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(void)", typeof(TypeOfExpression));
			Assert.AreEqual("System.Void", toe.TypeReference.SystemType);
		}
		
		[Test]
		public void CSharpArrayTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyType[])", typeof(TypeOfExpression));
			Assert.AreEqual("MyType", toe.TypeReference.Type);
			Assert.AreEqual(new int[] {0}, toe.TypeReference.RankSpecifier);
		}
		
		[Test]
		public void CSharpGenericTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyNamespace.N1.MyType<string>)", typeof(TypeOfExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
			Assert.AreEqual("System.String", toe.TypeReference.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void CSharpNestedGenericTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyType<string>.InnerClass<int>.InnerInnerClass)", typeof(TypeOfExpression));
			InnerClassTypeReference ic = (InnerClassTypeReference)toe.TypeReference;
			Assert.AreEqual("InnerInnerClass", ic.Type);
			Assert.AreEqual(0, ic.GenericTypes.Count);
			ic = (InnerClassTypeReference)ic.BaseType;
			Assert.AreEqual("InnerClass", ic.Type);
			Assert.AreEqual(1, ic.GenericTypes.Count);
			Assert.AreEqual("System.Int32", ic.GenericTypes[0].SystemType);
			Assert.AreEqual("MyType", ic.BaseType.Type);
			Assert.AreEqual(1, ic.BaseType.GenericTypes.Count);
			Assert.AreEqual("System.String", ic.BaseType.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void CSharpNullableTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyStruct?)", typeof(TypeOfExpression));
			Assert.AreEqual("System.Nullable", toe.TypeReference.SystemType);
			Assert.AreEqual("MyStruct", toe.TypeReference.GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpUnboundTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyType<,>)", typeof(TypeOfExpression));
			Assert.AreEqual("MyType", toe.TypeReference.Type);
			Assert.IsTrue(toe.TypeReference.GenericTypes[0].IsNull);
			Assert.IsTrue(toe.TypeReference.GenericTypes[1].IsNull);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBSimpleTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(MyNamespace.N1.MyType)", typeof(TypeOfExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
		}
		
		
		[Test]
		public void VBGlobalTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(Global.System.Console)", typeof(TypeOfExpression));
			Assert.AreEqual("System.Console", toe.TypeReference.Type);
		}
		
		[Test]
		public void VBPrimitiveTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(integer)", typeof(TypeOfExpression));
			Assert.AreEqual("System.Int32", toe.TypeReference.SystemType);
		}
		
		[Test]
		public void VBVoidTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(void)", typeof(TypeOfExpression));
			Assert.AreEqual("System.Void", toe.TypeReference.SystemType);
		}
		
		[Test]
		public void VBArrayTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(MyType())", typeof(TypeOfExpression));
			Assert.AreEqual("MyType", toe.TypeReference.Type);
			Assert.AreEqual(new int[] {0}, toe.TypeReference.RankSpecifier);
		}
		
		[Test]
		public void VBGenericTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(MyNamespace.N1.MyType(Of string))", typeof(TypeOfExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
			Assert.AreEqual("System.String", toe.TypeReference.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void VBUnboundTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(MyType(Of ,))", typeof(TypeOfExpression));
			Assert.AreEqual("MyType", toe.TypeReference.Type);
			Assert.IsTrue(toe.TypeReference.GenericTypes[0].IsNull);
			Assert.IsTrue(toe.TypeReference.GenericTypes[1].IsNull);
		}
		
		[Test]
		public void VBNestedGenericTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(MyType(Of string).InnerClass(of integer).InnerInnerClass)", typeof(TypeOfExpression));
			InnerClassTypeReference ic = (InnerClassTypeReference)toe.TypeReference;
			Assert.AreEqual("InnerInnerClass", ic.Type);
			Assert.AreEqual(0, ic.GenericTypes.Count);
			ic = (InnerClassTypeReference)ic.BaseType;
			Assert.AreEqual("InnerClass", ic.Type);
			Assert.AreEqual(1, ic.GenericTypes.Count);
			Assert.AreEqual("System.Int32", ic.GenericTypes[0].SystemType);
			Assert.AreEqual("MyType", ic.BaseType.Type);
			Assert.AreEqual(1, ic.BaseType.GenericTypes.Count);
			Assert.AreEqual("System.String", ic.BaseType.GenericTypes[0].SystemType);
		}
		#endregion
	}
}
