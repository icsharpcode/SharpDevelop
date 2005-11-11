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
			Assert.AreEqual("double", od.ConvertToType.Type);
		}
		
		[Test]
		public void CSharpExplicitOperatorDeclarationTest()
		{
			OperatorDeclaration od = ParseUtilCSharp.ParseTypeMember<OperatorDeclaration>("public static explicit operator double(MyObject f)  { return 0.5d; }");
			Assert.IsTrue(od.IsConversionOperator);
			Assert.AreEqual(1, od.Parameters.Count);
			Assert.AreEqual(ConversionType.Explicit, od.ConversionType);
			Assert.AreEqual("double", od.ConvertToType.Type);
		}
		
		[Test]
		public void CSharpPlusOperatorDeclarationTest()
		{
			OperatorDeclaration od = ParseUtilCSharp.ParseTypeMember<OperatorDeclaration>("public static MyObject operator +(MyObject a, MyObject b)  {}");
			Assert.IsTrue(!od.IsConversionOperator);
			Assert.AreEqual(2, od.Parameters.Count);
			Assert.AreEqual("MyObject", od.TypeReference.Type);
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
			Assert.AreEqual("Complex", od.ConvertToType.Type);
		}
		#endregion 
	}
}
