// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that an event handler such as button1.Click += new EventHandler(Button1Click) is converted
	/// to Python correctly.
	/// </summary>
	[TestFixture]
	public class EventHandlerWithObjectCreationTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{" +
						"\t\tbutton = new Button();\r\n" +
						"\t\tbutton.Click += new EventHandler(ButtonClick);\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\tvoid ButtonClick(object sender, EventArgs e)\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tbutton = Button()\r\n" +
									"\t\tbutton.Click += self.ButtonClick\r\n" +
									"\r\n" +
									"\tdef ButtonClick(self, sender, e):\r\n" +
									"\t\tpass";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
