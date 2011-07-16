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
	public class ThisReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpThisReferenceExpressionTest1()
		{
			ThisReferenceExpression tre = ParseUtilCSharp.ParseExpression<ThisReferenceExpression>("this");
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetThisReferenceExpressionTest1()
		{
			ThisReferenceExpression ie = ParseUtilVBNet.ParseExpression<ThisReferenceExpression>("Me");
		}
		#endregion
	}
}
