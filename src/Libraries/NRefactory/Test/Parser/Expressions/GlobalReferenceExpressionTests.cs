// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
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
	public class GlobalReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpGlobalReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("global::System", typeof(FieldReferenceExpression));
			Assert.IsTrue(fre.TargetObject is GlobalReferenceExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetGlobalReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("Global.System", typeof(FieldReferenceExpression));
			Assert.IsTrue(fre.TargetObject is GlobalReferenceExpression);
		}
		#endregion
	}
}
