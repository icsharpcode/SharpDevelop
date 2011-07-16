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
	public class PointerReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpPointerReferenceExpressionTest()
		{
			PointerReferenceExpression pre = ParseUtilCSharp.ParseExpression<PointerReferenceExpression>("myObj.field->b");
			Assert.IsTrue(pre.TargetObject is MemberReferenceExpression);
			Assert.AreEqual("b", pre.MemberName);
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion
	}
}
