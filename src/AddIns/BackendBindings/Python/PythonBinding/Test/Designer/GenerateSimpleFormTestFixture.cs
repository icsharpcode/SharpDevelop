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
	public class GenerateSimpleFormTestFixture
	{
		string generatedPythonCode;
		string formPropertiesCode;
		string propertyOwnerName;
		
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
				
				string indentString = "    ";
				PythonControl pythonForm = new PythonControl(indentString);
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
				
				PythonCodeBuilder codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = indentString;
				codeBuilder.IncreaseIndent();
				PythonDesignerRootComponent designerRootComponent = new PythonDesignerRootComponent(form);
				propertyOwnerName = designerRootComponent.GetPropertyOwnerName();
				designerRootComponent.AppendComponentProperties(codeBuilder, true, false);
				formPropertiesCode = codeBuilder.ToString();
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
		
		[Test]
		public void FormPropertiesCode()
		{
			string expectedCode = "    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n";
			Assert.AreEqual(expectedCode, formPropertiesCode);
		}
		
		[Test]
		public void PropertyOwnerName()
		{
			Assert.AreEqual("self", propertyOwnerName);
		}		
	}
}
