// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that an event handler such as button1.Click += new EventHandler(Button1Click) is converted
	/// to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class EventHandlerWithObjectCreationTestFixture
	{
		DefaultCompilationUnit unit;
		ParseInformation parseInfo;
		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public Foo()\r\n" +
			"    {" +
			"        button = new Button();\r\n" +
			"        button.Click += new EventHandler(ButtonClick);\r\n" +
			"    }\r\n" +
			"\r\n" +
			"    void ButtonClick(object sender, EventArgs e)\r\n" +
			"    {\r\n" +
			"    }\r\n" +
			"}";
			
		[SetUp]
		public void Init()
		{
			DefaultProjectContent projectContent = new DefaultProjectContent();
			unit = new DefaultCompilationUnit(projectContent);
			DefaultClass c = new DefaultClass(unit, "Foo");
			
			DefaultMethod buttonClickMethod = new DefaultMethod(c, "ButtonClick");
			AddSenderAndEventArgsParameters(buttonClickMethod);
			c.Methods.Add(buttonClickMethod);
			
			projectContent.AddClassToNamespaceList(c);
			
			parseInfo = new ParseInformation(unit);
		}
		
		void AddSenderAndEventArgsParameters(IMethod method)
		{
			DefaultReturnType returnType = new DefaultReturnType(method.DeclaringType);
			DomRegion region = new DomRegion();
			method.Parameters.Add(new DefaultParameter("sender", returnType, region));
			method.Parameters.Add(new DefaultParameter("e", returnType, region));
		}

		[Test]
		public void ConvertedRubyCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        button = Button.new()\r\n" +
				"        button.Click { |sender, e| self.ButtonClick(sender, e) }\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def ButtonClick(sender, e)\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp, parseInfo);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void CanGetFooClassFromCompilationUnitProjectContentGetClassMethod()
		{
			IClass c = unit.ProjectContent.GetClass("Foo", 0);
			Assert.AreEqual("Foo", c.Name);
		}
	}
}
