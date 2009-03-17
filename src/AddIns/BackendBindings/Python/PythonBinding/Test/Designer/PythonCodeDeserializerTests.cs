// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class PythonCodeDeserializerTests : LoadFormTestFixtureBase
	{
		PythonCodeDeserializer deserializer;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			deserializer = new PythonCodeDeserializer(this);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullIronPythonAstNode()
		{
			deserializer.Deserialize(null);
		}
		
		[Test]
		public void UnknownTypeName()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.UnknownType.AppStarting";
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", pythonCode);
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			AssignmentStatement assignment = suiteStatement.Statements[0] as AssignmentStatement;
			
			Assert.IsNull(deserializer.Deserialize(assignment.Right));
		}

		[Test]
		public void UnknownPropertyName()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.Cursors.UnknownCursorsProperty";
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", pythonCode);
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			AssignmentStatement assignment = suiteStatement.Statements[0] as AssignmentStatement;
			
			Assert.IsNull(deserializer.Deserialize(assignment.Right));
		}
		
		[Test]
		public void UnknownTypeNameInCallExpression()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.UnknownType.CreateDefaultCursor()";
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", pythonCode);
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			AssignmentStatement assignment = suiteStatement.Statements[0] as AssignmentStatement;
			
			Assert.IsNull(deserializer.Deserialize(assignment.Right));
		}		
	}
}
