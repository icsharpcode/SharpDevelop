// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using MbUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class GlobalReferenceExpressionTests
	{
		[Test]
		public void CSharpGlobalReferenceExpressionTest()
		{
			TypeReferenceExpression tre = (TypeReferenceExpression)ParseUtilCSharp.ParseExpression("global::System", typeof(TypeReferenceExpression));
			Assert.IsTrue(tre.TypeReference.IsGlobal);
			Assert.AreEqual("System", tre.TypeReference.Type);
		}
		
		[Test]
		public void VBNetGlobalReferenceExpressionTest()
		{
			TypeReferenceExpression tre = (TypeReferenceExpression)ParseUtilVBNet.ParseExpression("Global.System", typeof(TypeReferenceExpression));
			Assert.IsTrue(tre.TypeReference.IsGlobal);
			Assert.AreEqual("System", tre.TypeReference.Type);
		}
		
		[Test]
		public void CSharpGlobalTypeDeclaration()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("global::System.String a;", typeof(LocalVariableDeclaration));
			TypeReference typeRef = lvd.GetTypeForVariable(0);
			Assert.IsTrue(typeRef.IsGlobal);
			Assert.AreEqual("System.String", typeRef.Type);
		}
		
		[Test]
		public void VBNetGlobalTypeDeclaration()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As Global.System.String", typeof(LocalVariableDeclaration));
			TypeReference typeRef = lvd.GetTypeForVariable(0);
			Assert.IsTrue(typeRef.IsGlobal);
			Assert.AreEqual("System.String", typeRef.Type);
		}
	}
}
