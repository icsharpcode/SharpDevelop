// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
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
