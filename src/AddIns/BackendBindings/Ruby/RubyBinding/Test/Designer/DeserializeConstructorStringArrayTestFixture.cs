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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the constructor arguments are returned when the first argument is
	/// an array.
	/// </summary>
	[TestFixture]
	public class DeserializeConstructorStringArrayTestFixture
	{		
		string code =  "System::Windows::Forms::ListViewItem(System::Array[System::String].new(\r\n" +
				"    [\"a\",\r\n" +
				"    \"sa\",\r\n" +
				"    \"sa2\"]))\r\n";

		List<object> args;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockComponentCreator componentCreator = new MockComponentCreator();
			MethodCall callExpression = RubyParserHelper.GetMethodCall(code);			
			RubyCodeDeserializer deserializer = new RubyCodeDeserializer(componentCreator);
			args = deserializer.GetArguments(callExpression);
		}
		
		[Test]
		public void OneArgument()
		{
			Assert.AreEqual(1, args.Count);
		}

		[Test]
		public void ArgumentIsStringArray()
		{
			string[] array = new string[0];
			Assert.IsInstanceOf(array.GetType(), args[0]);
		}
	}
}
