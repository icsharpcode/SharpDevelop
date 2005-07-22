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
		public void VBNetSpecializedObjectCastExpression()
		{
			TestSpecializedCast("CObj(o)", typeof(System.Object));
		}		
		#endregion
	}
}
