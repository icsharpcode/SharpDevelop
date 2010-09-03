// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ConvertEventHandlerForClassWithNamespaceTestFixture
	{
		DefaultCompilationUnit unit;
		ParseInformation parseInfo;
		
		string csharp = 
			"namespace NewNamespace\r\n" +
			"{\r\n" +
			"    class Foo\r\n" +
			"    {\r\n" +
			"        public Foo()\r\n" +
			"        {" +
			"            button = new Button();\r\n" +
			"            button.Click += ButtonClick;\r\n" +
			"            button.MouseDown += self.OnMouseDown;\r\n" +
			"        }\r\n" +
			"    \r\n" +
			"        void ButtonClick(object sender, EventArgs e)\r\n" +
			"        {\r\n" +
			"        }\r\n" +
			"    \r\n" +
			"        void OnMouseDown(object sender, EventArgs e)\r\n" +
			"        {\r\n" +
			"        }\r\n" +
			"    }\r\n" +
			"}";
		
		[SetUp]
		public void Init()
		{
			DefaultProjectContent projectContent = new DefaultProjectContent();
			unit = new DefaultCompilationUnit(projectContent);
			DefaultClass c = new DefaultClass(unit, "NewNamespace.Foo");
			
			DefaultMethod buttonClickMethod = new DefaultMethod(c, "ButtonClick");
			AddSenderAndEventArgsParameters(buttonClickMethod);
			c.Methods.Add(buttonClickMethod);
			
			DefaultMethod onMouseDownMethod = new DefaultMethod(c, "OnMouseDown");
			AddSenderAndEventArgsParameters(onMouseDownMethod);
			c.Methods.Add(onMouseDownMethod);
			
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
				"module NewNamespace\r\n" +
				"    class Foo\r\n" +
				"        def initialize()\r\n" +
				"            button = Button.new()\r\n" +
				"            button.Click { |sender, e| self.ButtonClick(sender, e) }\r\n" +
				"            button.MouseDown { |sender, e| self.OnMouseDown(sender, e) }\r\n" +
				"        end\r\n" +
				"\r\n" +
				"        def ButtonClick(sender, e)\r\n" +
				"        end\r\n" +
				"\r\n" +
				"        def OnMouseDown(sender, e)\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create("test.cs", parseInfo);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code, code);
		}
		
		[Test]
		public void CanGetFooClassFromCompilationUnitProjectContentGetClassMethod()
		{
			IClass c = unit.ProjectContent.GetClass("NewNamespace.Foo", 0);
			Assert.AreEqual("Foo", c.Name);
		}
	}
}
