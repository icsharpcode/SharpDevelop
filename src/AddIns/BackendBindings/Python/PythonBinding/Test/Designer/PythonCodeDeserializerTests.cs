// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class PythonCodeDeserializerTests
	{
		PythonCodeDeserializer deserializer;
		MockComponentCreator componentCreator;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			componentCreator = new MockComponentCreator();
			deserializer = new PythonCodeDeserializer(componentCreator);
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
			Assert.IsNull(DeserializeRhsAssignment(pythonCode));
		}

		[Test]
		public void UnknownPropertyName()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.Cursors.UnknownCursorsProperty";			
			Assert.IsNull(DeserializeRhsAssignment(pythonCode));
		}
		
		[Test]
		public void UnknownTypeNameInCallExpression()
		{
			string pythonCode = "self.Cursors = System.Windows.Forms.UnknownType.CreateDefaultCursor()";
			Assert.IsNull(DeserializeRhsAssignment(pythonCode));
		}

		[Test]
		public void EnumReturnedInArgumentsPassedToConstructor()
		{
			string pythonCode = "self.Font = System.Drawing.Font(\"Times New Roman\", System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)";
			AssignmentStatement assignment = PythonParserHelper.GetAssignmentStatement(pythonCode);
			
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
			Assert.AreEqual(expectedStyles, DeserializeRhsAssignment(pythonCode));
		}

		[Test]
		public void MultipleEnumBitwiseOr()
		{
			string pythonCode = "self.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left";
			
			AnchorStyles expectedStyles = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			Assert.AreEqual(expectedStyles, DeserializeRhsAssignment(pythonCode));
		}
		
		[Test]
		public void DeserializeNameExpression()
		{
			string pythonCode = "self.Items = self";
			Assert.IsNull(DeserializeRhsAssignment(pythonCode));
		}
		
		/// <summary>
		/// Deserializes the right hand side of the assignment.
		/// </summary>
		object DeserializeRhsAssignment(string pythonCode)
		{
			return deserializer.Deserialize(PythonParserHelper.GetAssignmentStatement(pythonCode).Right);
		}
	}
}
