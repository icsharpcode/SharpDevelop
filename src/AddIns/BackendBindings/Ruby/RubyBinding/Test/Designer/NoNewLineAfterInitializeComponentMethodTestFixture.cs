// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests the code can be generated if there is no new line after the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class NoNewLineAfterInitializeComponentMethodTestFixture
	{
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (TextEditorControl textEditor = new TextEditorControl()) {
				document = textEditor.Document;
				textEditor.Text = GetTextEditorCode();

				RubyParser parser = new RubyParser();
				ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.TextContent);

				using (DesignSurface designSurface = new DesignSurface(typeof(UserControl))) {
					IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
					UserControl userControl = (UserControl)host.RootComponent;			
					userControl.ClientSize = new Size(489, 389);
					
					PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(userControl);
					PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
					namePropertyDescriptor.SetValue(userControl, "userControl1");
					
					DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
					using (serializationManager.CreateSession()) {
						RubyDesignerGenerator.Merge(host, document, compilationUnit, new MockTextEditorProperties(), serializationManager);
					}
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode =
				"require \"System.Windows.Forms\"\r\n" +
				"\r\n" +
				"class MyUserControl < UserControl\r\n" +
				"\tdef initialize()\r\n" +
				"\t\tself.InitializeComponent()\r\n" +
				"\tend\r\n" +
				"\r\n" +
				"\tdef InitializeComponent()\r\n" +
				"\t\tself.SuspendLayout()\r\n" +
				"\t\t# \r\n" +
				"\t\t# userControl1\r\n" +
				"\t\t# \r\n" + 
				"\t\tself.Name = \"userControl1\"\r\n" +
				"\t\tself.Size = System::Drawing::Size.new(489, 389)\r\n" +
				"\t\tself.ResumeLayout(false)\r\n" +
				"\tend\r\n" +
				"end";
			
			Assert.AreEqual(expectedCode, document.TextContent);
		}
		
		/// <summary>
		/// No new line after the pass statement for InitializeComponent method.
		/// </summary>
		string GetTextEditorCode()
		{
			return
				"require \"System.Windows.Forms\"\r\n" +
				"\r\n" +
				"class MyUserControl < UserControl\r\n" +
				"\tdef initialize()\r\n" +
				"\t\tself.InitializeComponent()\r\n" +
				"\tend\r\n" +
				"\r\n" +
				"\tdef InitializeComponent()\r\n" +
				"\tend\r\n" +
				"end";
		}
	}
}
