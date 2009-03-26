// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
		public new void SetUpFixture()
		{
			deserializer = new PythonCodeDeserializer(this);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullIronPythonAstNode()
		{
			Node node = null;
			deserializer.Deserialize(node);
		}
		
		[Test]
		public void UnknownTypeName()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.UnknownType.AppStarting";
			Assert.IsNull(Deserialize(pythonCode));
		}

		[Test]
		public void UnknownPropertyName()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.Cursors.UnknownCursorsProperty";			
			Assert.IsNull(Deserialize(pythonCode));
		}
		
		[Test]
		public void UnknownTypeNameInCallExpression()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.UnknownType.CreateDefaultCursor()";
			Assert.IsNull(Deserialize(pythonCode));
		}

		[Test]
		public void EnumReturnedInArgumentsPassedToConstructor()
		{
			string pythonCode = "self.Font = System.Drawing.Font(\"Times New Roman\", System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)";
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", pythonCode);
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			AssignmentStatement assignment = suiteStatement.Statements[0] as AssignmentStatement;
			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add("Times New Roman");
			expectedArgs.Add(FontStyle.Regular);
			expectedArgs.Add(GraphicsUnit.Point);
						
			List<object> args = deserializer.GetArguments(assignment.Right as CallExpression);
			
			Assert.AreEqual(expectedArgs, args);
		}
		
		[Test]
		public void EnumBitwiseOr()
		{
			string pythonCode = "self.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom";
			
			AnchorStyles expectedStyles = AnchorStyles.Top | AnchorStyles.Bottom;
			Assert.AreEqual(expectedStyles, Deserialize(pythonCode));
		}

		[Test]
		public void MultipleEnumBitwiseOr()
		{
			string pythonCode = "self.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left";
			
			AnchorStyles expectedStyles = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			Assert.AreEqual(expectedStyles, Deserialize(pythonCode));
		}
		
		/// <summary>
		/// Deserializes the right hand side of the assignment.
		/// </summary>
		object Deserialize(string pythonCode)
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", pythonCode);
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			AssignmentStatement assignment = suiteStatement.Statements[0] as AssignmentStatement;
			return deserializer.Deserialize(assignment.Right);
		}
	}
}
