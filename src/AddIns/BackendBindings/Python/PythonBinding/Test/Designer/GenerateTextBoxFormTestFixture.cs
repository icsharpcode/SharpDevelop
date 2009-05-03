// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateTextBoxFormTestFixture
	{
		string generatedPythonCode;
		string textBoxPropertyCode;
		string textBoxSuspendLayoutCode;
		string textBoxResumeLayoutCode;
		string textBoxCreationCode;
		string textBoxPropertyOwnerName;
		string suspendLayoutCode;
		string resumeLayoutCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(284, 264);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				TextBox textBox = (TextBox)host.CreateComponent(typeof(TextBox), "textBox1");
				textBox.Size = new Size(110, 20);
				textBox.TabIndex = 1;
				textBox.Location = new Point(10, 10);
				
				form.Controls.Add(textBox);
				
				string indentString = "    ";
				PythonControl pythonForm = new PythonControl(indentString);
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
				
				PythonCodeBuilder codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = indentString;
				codeBuilder.IncreaseIndent();
				PythonDesignerComponent designerComponent = new PythonDesignerComponent(textBox);
				designerComponent.AppendComponent(codeBuilder);
				textBoxPropertyCode = codeBuilder.ToString();
				
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = indentString;
				designerComponent.AppendCreateInstance(codeBuilder);
				textBoxCreationCode = codeBuilder.ToString();				
				
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = indentString;
				designerComponent.AppendSuspendLayout(codeBuilder);
				textBoxSuspendLayoutCode = codeBuilder.ToString();
				
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = indentString;
				designerComponent.AppendResumeLayout(codeBuilder);
				textBoxResumeLayoutCode = codeBuilder.ToString();	
				
				textBoxPropertyOwnerName = designerComponent.GetPropertyOwnerName();
				
				PythonDesignerRootComponent designerRootComponent = new PythonDesignerRootComponent(form);
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = indentString;
				designerRootComponent.AppendSuspendLayout(codeBuilder);
				suspendLayoutCode = codeBuilder.ToString();
				
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = indentString;
				designerRootComponent.AppendResumeLayout(codeBuilder);
				resumeLayoutCode = codeBuilder.ToString();
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self._textBox1 = System.Windows.Forms.TextBox()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # textBox1\r\n" +
								"    # \r\n" +
								"    self._textBox1.Location = System.Drawing.Point(10, 10)\r\n" +
								"    self._textBox1.Name = \"textBox1\"\r\n" +
								"    self._textBox1.Size = System.Drawing.Size(110, 20)\r\n" +
								"    self._textBox1.TabIndex = 1\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Controls.Add(self._textBox1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
		
		[Test]
		public void TextBoxGeneratedCode()
		{
			string expectedCode = "    # \r\n" +
								"    # textBox1\r\n" +
								"    # \r\n" +
								"    self._textBox1.Location = System.Drawing.Point(10, 10)\r\n" +
								"    self._textBox1.Name = \"textBox1\"\r\n" +
								"    self._textBox1.Size = System.Drawing.Size(110, 20)\r\n" +
								"    self._textBox1.TabIndex = 1\r\n";
			Assert.AreEqual(expectedCode, textBoxPropertyCode);
		}
		
		[Test]
		public void SuspendLayoutCodeNotGenerated()
		{
			Assert.AreEqual(String.Empty, textBoxSuspendLayoutCode);
		}
		
		[Test]
		public void ResumeLayoutCodeNotGenerated()
		{
			Assert.AreEqual(String.Empty, textBoxResumeLayoutCode);
		}
		
		[Test]
		public void TextBoxCreationCode()
		{
			string expectedCode = "self._textBox1 = System.Windows.Forms.TextBox()\r\n";
			Assert.AreEqual(expectedCode, textBoxCreationCode);
		}
		
		[Test]
		public void TextBoxPropertyOwnerName()
		{
			Assert.AreEqual("self._textBox1", textBoxPropertyOwnerName);
		}
		
		[Test]
		public void SuspendLayoutCode()
		{
			Assert.AreEqual("self.SuspendLayout()\r\n", suspendLayoutCode);
		}
		
		[Test]
		public void ResumeLayoutCode()
		{
			string expectedCode = "self.ResumeLayout(False)\r\n" +
								"self.PerformLayout()\r\n";
			Assert.AreEqual(expectedCode, resumeLayoutCode);
		}				
	}
}
