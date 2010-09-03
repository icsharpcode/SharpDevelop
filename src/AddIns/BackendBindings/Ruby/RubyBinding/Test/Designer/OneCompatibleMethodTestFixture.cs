// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that one method is found to be that could
	/// be used as a event handler.
	/// </summary>
	[TestFixture]
	public class OneCompatibleMethodTestFixture
	{
		ICollection compatibleMethods;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			DerivedRubyDesignerGenerator generator = new DerivedRubyDesignerGenerator();
			MockTextEditorViewContent mockViewContent = new MockTextEditorViewContent();
			DerivedFormDesignerViewContent viewContent = new DerivedFormDesignerViewContent(mockViewContent, new MockOpenedFile("Test.rb"));
			viewContent.DesignerCodeFileContent = GetTextEditorCode();
			
			// Create parse info.
			RubyParser parser = new RubyParser();
			ICompilationUnit unit = parser.Parse(new MockProjectContent(), @"C:\Projects\MyProject\test.rb", GetTextEditorCode());
			ParseInformation parseInfo = new ParseInformation(unit);
			generator.ParseInfoToReturnFromParseFileMethod = parseInfo;
			
			// Attach view content to generator.
			generator.Attach(viewContent);
			
			// Get compatible methods for event.
			MockEventDescriptor eventDescriptor = new MockEventDescriptor("Click");
			compatibleMethods = generator.GetCompatibleMethods(eventDescriptor);
		}

		[Test]
		public void OneCompatibleMethod()
		{
			Assert.AreEqual(1, compatibleMethods.Count);
		}
		
		[Test]
		public void CompatibleMethodName()
		{
			IEnumerator enumerator = compatibleMethods.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual("button1_click", (string)enumerator.Current);
		}
		
		string GetTextEditorCode()
		{
			return "class MainForm < System::Windows::Forms::Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents()\r\n" +
					"\t\t@button1 = System::Windows::Forms::Button.new()\r\n" +
					"\t\tself.Controls.Add(@button1)\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef button1_click(sender, e)\r\n" +
					"\tend\r\n" +
					"end";
		}		
	}
}
