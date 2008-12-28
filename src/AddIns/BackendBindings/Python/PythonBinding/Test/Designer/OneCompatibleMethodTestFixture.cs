// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
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
			DerivedPythonDesignerGenerator generator = new DerivedPythonDesignerGenerator();
			MockTextEditorViewContent mockViewContent = new MockTextEditorViewContent();
			DerivedFormDesignerViewContent viewContent = new DerivedFormDesignerViewContent(mockViewContent, new MockOpenedFile("Test.py"));
			viewContent.DesignerCodeFileContent = GetTextEditorCode();
			
			// Create parse info.
			PythonParser parser = new PythonParser();
			ICompilationUnit unit = parser.Parse(new MockProjectContent(), @"C:\Projects\MyProject\test.py", GetTextEditorCode());
			ParseInformation parseInfo = new ParseInformation();
			parseInfo.SetCompilationUnit(unit);
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
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents(self):\r\n" +
					"\t\tself._button1 = System.Windows.Forms.Button()\r\n" +
					"\t\tself.Controls.Add(self._button1)\r\n" +
					"\t\r\n" +
					"\tdef button1_click(self, sender, e):\r\n" +
					"\t\tpass";
		}		
	}
}
