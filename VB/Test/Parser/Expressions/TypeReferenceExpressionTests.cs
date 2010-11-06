// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.VB.Parser;
using ICSharpCode.NRefactory.VB.Dom;

namespace ICSharpCode.NRefactory.VB.Tests.Dom
{
	[TestFixture]
	public class TypeReferenceExpressionTests
	{
		#region VB.NET
		[Test]
		public void VBIntReferenceExpression()
		{
			MemberReferenceExpression fre = ParseUtilVBNet.ParseExpression<MemberReferenceExpression>("inTeGer.MaxValue");
			Assert.AreEqual("MaxValue", fre.MemberName);
			Assert.AreEqual("System.Int32", ((TypeReferenceExpression)fre.TargetObject).TypeReference.Type);
		}
		
		[Test]
		public void VBStandaloneIntReferenceExpression()
		{
			TypeReferenceExpression tre = ParseUtilVBNet.ParseExpression<TypeReferenceExpression>("inTeGer");
			Assert.AreEqual("System.Int32", tre.TypeReference.Type);
		}
		
		[Test]
		public void VBObjectReferenceExpression()
		{
			MemberReferenceExpression fre = ParseUtilVBNet.ParseExpression<MemberReferenceExpression>("Object.ReferenceEquals");
			Assert.AreEqual("ReferenceEquals", fre.MemberName);
			Assert.AreEqual("System.Object", ((TypeReferenceExpression)fre.TargetObject).TypeReference.Type);
		}
		
		[Test]
		public void VBStandaloneObjectReferenceExpression()
		{
			TypeReferenceExpression tre = ParseUtilVBNet.ParseExpression<TypeReferenceExpression>("obJect");
			Assert.AreEqual("System.Object", tre.TypeReference.Type);
		}
		#endregion
	}
}
