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
	public class CastExpressionTests
	{
		#region C#
		[Test]
		public void CSharpSimpleCastExpression()
		{
			CastExpression ce = (CastExpression)ParseUtilCSharp.ParseExpression("(MyObject)o", typeof(CastExpression));
			Assert.AreEqual("MyObject", ce.CastTo.Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
			Assert.IsFalse(ce.IsSpecializedCast);
		}
		
		[Test]
		public void CSharpArrayCastExpression()
		{
			CastExpression ce = (CastExpression)ParseUtilCSharp.ParseExpression("(MyType[])o", typeof(CastExpression));
			Assert.AreEqual("MyType", ce.CastTo.Type);
			Assert.AreEqual(new int[] { 0 }, ce.CastTo.RankSpecifier);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
			Assert.IsFalse(ce.IsSpecializedCast);
		}
		
		[Test]
		public void GenericCastExpression()
		{
			CastExpression ce = (CastExpression)ParseUtilCSharp.ParseExpression("(List<string>)o", typeof(CastExpression));
			Assert.AreEqual("List", ce.CastTo.Type);
			Assert.AreEqual("string", ce.CastTo.GenericTypes[0].Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
			Assert.IsFalse(ce.IsSpecializedCast);
		}
		
		[Test]
		public void GenericArrayCastExpression()
		{
			CastExpression ce = (CastExpression)ParseUtilCSharp.ParseExpression("(List<string>[])o", typeof(CastExpression));
			Assert.AreEqual("List", ce.CastTo.Type);
			Assert.AreEqual("string", ce.CastTo.GenericTypes[0].Type);
			Assert.AreEqual(new int[] { 0 }, ce.CastTo.RankSpecifier);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
			Assert.IsFalse(ce.IsSpecializedCast);
		}
		
		[Test]
		public void CSharpCastMemberReferenceOnParenthesizedExpression()
		{
			// yes, we really wanted to evaluate .Member on expr and THEN cast the result to MyType
			CastExpression ce = (CastExpression)ParseUtilCSharp.ParseExpression("(MyType)(expr).Member", typeof(CastExpression));
			Assert.AreEqual("MyType", ce.CastTo.Type);
			Assert.IsTrue(ce.Expression is FieldReferenceExpression);
			Assert.IsFalse(ce.IsSpecializedCast);
		}
		#endregion
		
		#region VB.NET
		void TestSpecializedCast(string castExpression, Type castType)
		{
			CastExpression ce = (CastExpression)ParseUtilVBNet.ParseExpression(castExpression, typeof(CastExpression));
			Assert.AreEqual(castType.FullName, ce.CastTo.Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
			Assert.IsTrue(ce.IsSpecializedCast);
		}
		
		
		[Test]
		public void VBNetSimpleCastExpression()
		{
			CastExpression ce = (CastExpression)ParseUtilVBNet.ParseExpression("CType(o, MyObject)", typeof(CastExpression));
			Assert.AreEqual("MyObject", ce.CastTo.Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
			Assert.IsFalse(ce.IsSpecializedCast);
		}
		
		[Test]
		public void VBNetSpecializedBoolCastExpression()
		{
			TestSpecializedCast("CBool(o)", typeof(System.Boolean));
		}
		
		[Test]
		public void VBNetSpecializedCharCastExpression()
		{
			TestSpecializedCast("CChar(o)", typeof(System.Char));
		}
		
		
		[Test]
		public void VBNetSpecializedStringCastExpression()
		{
			TestSpecializedCast("CStr(o)", typeof(System.String));
		}
		
		[Test]
		public void VBNetSpecializedDateTimeCastExpression()
		{
			TestSpecializedCast("CDate(o)", typeof(System.DateTime));
		}
		
		[Test]
		public void VBNetSpecializedDecimalCastExpression()
		{
			TestSpecializedCast("CDec(o)", typeof(System.Decimal));
		}
		
		[Test]
		public void VBNetSpecializedSingleCastExpression()
		{
			TestSpecializedCast("CSng(o)", typeof(System.Single));
		}
		
		[Test]
		public void VBNetSpecializedDoubleCastExpression()
		{
			TestSpecializedCast("CDbl(o)", typeof(System.Double));
		}
		
		[Test]
		public void VBNetSpecializedByteCastExpression()
		{
			TestSpecializedCast("CByte(o)", typeof(System.Byte));
		}
		
		[Test]
		public void VBNetSpecializedInt16CastExpression()
		{
			TestSpecializedCast("CShort(o)", typeof(System.Int16));
		}
		
		[Test]
		public void VBNetSpecializedInt32CastExpression()
		{
			TestSpecializedCast("CInt(o)", typeof(System.Int32));
		}
		
		[Test]
		public void VBNetSpecializedInt64CastExpression()
		{
			TestSpecializedCast("CLng(o)", typeof(System.Int64));
		}
		
		[Test]
		public void VBNetSpecializedSByteCastExpression()
		{
			TestSpecializedCast("CSByte(o)", typeof(System.SByte));
		}
		
		[Test]
		public void VBNetSpecializedUInt16CastExpression()
		{
			TestSpecializedCast("CUShort(o)", typeof(System.UInt16));
		}
		
		[Test]
		public void VBNetSpecializedUInt32CastExpression()
		{
			TestSpecializedCast("CUInt(o)", typeof(System.UInt32));
		}
		
		[Test]
		public void VBNetSpecializedUInt64CastExpression()
		{
			TestSpecializedCast("CULng(o)", typeof(System.UInt64));
		}
		
		
		[Test]
		public void VBNetSpecializedObjectCastExpression()
		{
			TestSpecializedCast("CObj(o)", typeof(System.Object));
		}
		#endregion
	}
}
