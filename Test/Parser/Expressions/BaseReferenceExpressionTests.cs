// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class BaseReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpBaseReferenceExpressionTest1()
		{
			MemberReferenceExpression fre = ParseUtilCSharp.ParseExpression<MemberReferenceExpression>("base.myField");
			Assert.IsTrue(fre.TargetObject is BaseReferenceExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetBaseReferenceExpressionTest1()
		{
			MemberReferenceExpression fre = ParseUtilVBNet.ParseExpression<MemberReferenceExpression>("MyBase.myField");
			Assert.IsTrue(fre.TargetObject is BaseReferenceExpression);
		}
		#endregion
	}
}
