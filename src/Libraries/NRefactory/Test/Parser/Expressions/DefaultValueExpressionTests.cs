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
	public class DefaultValueExpressionTests
	{
		[Test]
		public void CSharpSimpleDefaultValue()
		{
			DefaultValueExpression toe = (DefaultValueExpression)ParseUtilCSharp.ParseExpression("default(T)", typeof(DefaultValueExpression));
			Assert.AreEqual("T", toe.TypeReference.Type);
		}
		
		[Test]
		public void CSharpFullQualifiedDefaultValue()
		{
			DefaultValueExpression toe = (DefaultValueExpression)ParseUtilCSharp.ParseExpression("default(MyNamespace.N1.MyType)", typeof(DefaultValueExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
		}
		
		[Test]
		public void CSharpGenericDefaultValue()
		{
			DefaultValueExpression toe = (DefaultValueExpression)ParseUtilCSharp.ParseExpression("default(MyNamespace.N1.MyType<string>)", typeof(DefaultValueExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
			Assert.AreEqual("string", toe.TypeReference.GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpDefaultValueAsIntializer()
		{
			// This test is failing because we need a resolver for the "default:" / "default(" conflict.
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("T a = default(T);", typeof(LocalVariableDeclaration));
			DefaultValueExpression dve = (DefaultValueExpression)lvd.Variables[0].Initializer;
			Assert.AreEqual("T", dve.TypeReference.Type);
		}
	}
}
