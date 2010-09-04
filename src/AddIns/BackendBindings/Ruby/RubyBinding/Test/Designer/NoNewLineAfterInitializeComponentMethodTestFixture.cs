// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests the code can be generated if there is no new line after the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class NoNewLineAfterInitializeComponentMethodTestFixture
	{
		TextDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			AvalonEdit.TextEditor textEditor = new AvalonEdit.TextEditor();
			document = textEditor.Document;
			textEditor.Text = GetTextEditorCode();

			RubyParser parser = new RubyParser();
			ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.Text);

			using (DesignSurface designSurface = new DesignSurface(typeof(UserControl))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				UserControl userControl = (UserControl)host.RootComponent;			
				userControl.ClientSize = new Size(489, 389);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(userControl);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(userControl, "userControl1");
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					AvalonEditDocumentAdapter docAdapter = new AvalonEditDocumentAdapter(document, null);
					RubyDesignerGenerator generator = new RubyDesignerGenerator(new MockTextEditorOptions());
					generator.Merge(host, docAdapter, compilationUnit, serializationManager);
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
			
			Assert.AreEqual(expectedCode, document.Text);
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
