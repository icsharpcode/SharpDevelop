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
	/// <summary>
	/// A Form has a base ContainerControl class which has an AutoScaleMode property. This has the following attributes:
	/// 
	/// Browsable = false
	/// DesignerSerializationVisibility = Hidden
	/// EditorBrowsable = EditorBrowsableState.Advanced
	/// 
	/// However the forms root designer overrides these and shows it in the designer. 
	/// 
	/// This test fixture checks that the AutoScaleMode value will be generated in the form's code
	/// by the python forms designer.
	/// </summary>
	[TestFixture]
	public class GenerateAutoScaleModeFormTestFixture
	{
		string generatedPythonCode;
		
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

				PropertyDescriptor autoScaleModeDescriptor = descriptors.Find("AutoScaleMode", false);
				autoScaleModeDescriptor.SetValue(form, AutoScaleMode.Font);
				
				PropertyDescriptor autoScaleDimensionsDescriptor = descriptors.Find("AutoScaleDimensions", false);
				autoScaleDimensionsDescriptor.SetValue(form, new SizeF(6F, 13F));

				string indentString = "    ";
				PythonForm pythonForm = new PythonForm(indentString);
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
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
								"    self.AutoScaleDimensions = System.Drawing.SizeF(6, 13)\r\n" +
								"    self.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font\r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
