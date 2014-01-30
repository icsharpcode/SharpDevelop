// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class RubyCodeDeserializerTests
	{
		RubyCodeDeserializer deserializer;
		MockComponentCreator componentCreator;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			componentCreator = new MockComponentCreator();
			deserializer = new RubyCodeDeserializer(componentCreator);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullIronRubyAstNode()
		{
			Expression expression = null;
			deserializer.Deserialize(expression);
		}
		
		[Test]
		public void UnknownTypeName()
		{
			string rubyCode = "self.Cursors = System::Windows::Forms::UnknownType.AppStarting";
			Assert.IsNull(DeserializeRhsAssignment(rubyCode));
		}

		[Test]
		public void UnknownPropertyName()
		{
			string rubyCode = "self.Cursors = System::Windows::Forms::Cursors.UnknownCursorsProperty";			
			Assert.IsNull(DeserializeRhsAssignment(rubyCode));
		}
		
		[Test]
		public void UnknownTypeNameInCallExpression()
		{
			string rubyCode = "self.Cursors = System::Windows::Forms::UnknownType.CreateDefaultCursor()";
			Assert.IsNull(DeserializeRhsAssignment(rubyCode));
		}
		
		[Test]
		public void DeserializeColor()
		{
			string rubyCode = "@button1.FlatAppearance.BorderColor = System::Drawing::Color.Red";
			Assert.AreEqual(Color.Red, DeserializeRhsAssignment(rubyCode));
		}

		[Test]
		public void EnumReturnedInArgumentsPassedToConstructor()
		{
			string rubyCode = "self.Font = System::Drawing::Font.new(\"Times New Roman\", System::Drawing::FontStyle.Regular, System::Drawing::GraphicsUnit.Point)";
			SimpleAssignmentExpression assignment = RubyParserHelper.GetSimpleAssignmentExpression(rubyCode);
			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add("Times New Roman");
			expectedArgs.Add(FontStyle.Regular);
			expectedArgs.Add(GraphicsUnit.Point);
						
			List<object> args = deserializer.GetArguments(assignment.Right as MethodCall);
			
			Assert.AreEqual(expectedArgs, args);
		}
		
		[Test]
		public void EnumBitwiseOr()
		{
			string rubyCode = "self.textBox1.Anchor = System::Windows::Forms::AnchorStyles.Top | System::Windows::Forms::AnchorStyles.Bottom";
			
			AnchorStyles expectedStyles = AnchorStyles.Top | AnchorStyles.Bottom;
			Assert.AreEqual(expectedStyles, DeserializeRhsAssignment(rubyCode));
		}

		[Test]
		public void MultipleEnumBitwiseOr()
		{
			string rubyCode = "self.textBox1.Anchor = System::Windows::Forms::AnchorStyles.Top | System::Windows::Forms::AnchorStyles.Bottom | System::Windows::Forms::AnchorStyles.Left";
			
			AnchorStyles expectedStyles = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			Assert.AreEqual(expectedStyles, DeserializeRhsAssignment(rubyCode));
		}
		
		[Test]
		public void DeserializeNameExpression()
		{
			string rubyCode = "self.Items = self";
			Assert.IsNull(DeserializeRhsAssignment(rubyCode));
		}
		
		/// <summary>
		/// Deserializes the right hand side of the assignment.
		/// </summary>
		object DeserializeRhsAssignment(string rubyCode)
		{
			return deserializer.Deserialize(RubyParserHelper.GetSimpleAssignmentExpression(rubyCode).Right);
		}
	}
}
