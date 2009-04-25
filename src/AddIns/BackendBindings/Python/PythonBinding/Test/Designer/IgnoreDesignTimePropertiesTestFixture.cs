// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// The python form code generator should ignore any design time properties (e.g. Locked) of the form. 
	/// These are put in the .resx file not the source code.
	/// </summary>
	[TestFixture]
	public class IgnoreDesignTimePropertiesTestFixture
	{
		string expectedCode = "def InitializeComponent(self):\r\n" +
							  "    self.SuspendLayout()\r\n" +
							  "    # \r\n" +
							  "    # MainForm\r\n" +
							  "    # \r\n" +
							  "    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
							  "    self.Name = \"MainForm\"\r\n" +
							  "    self.ResumeLayout(False)\r\n" +
							  "    self.PerformLayout()\r\n";

		string generatedCode;
		PropertyDescriptorCollection propertyDescriptors;
		
		/// <summary>
		/// After a form is loaded onto a DesignSurface this checks that the PythonForm does not try to 
		/// add design time properties and does not throw a null reference exception.
		/// </summary>
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				PythonControl pythonForm = new PythonControl("    ");
				generatedCode = pythonForm.GenerateInitializeComponentMethod(form);
				
				propertyDescriptors = PythonControl.GetSerializableProperties(form);
			}
		}
		
		[Test]
		public void DesignTimePropertyIsIgnoredInGeneratedCode()
		{						
			Assert.AreEqual(expectedCode, generatedCode);
		}
		
		[Test]
		public void AtLeastOneSerializableProperty()
		{
			Assert.IsTrue(propertyDescriptors.Count > 0);
		}
		
		[Test]
		public void NoLockedProperty()
		{
			Assert.IsFalse(ContainsProperty(propertyDescriptors, "Locked"), "Locked property is not expected.");
		}
		
		[Test]
		public void NoTopLevelPropertyWhichHasDesignTimeVisibilityHidden()
		{
			Assert.IsFalse(ContainsProperty(propertyDescriptors, "TopLevel"), "TopLevel property is not expected.");
		}

		[Test]
		public void NoTagPropertyWhichHasDefaultValue()
		{
			Assert.IsFalse(ContainsProperty(propertyDescriptors, "Tag"), "Tag property is not expected.");
		}
		
		[Test]
		public void PropertiesAreSorted()
		{	
			List<string> strings = new List<string>();
			List<string> unsortedStrings = new List<string>();
			foreach (PropertyDescriptor p in propertyDescriptors) {
				strings.Add(p.Name);
				unsortedStrings.Add(p.Name);
			}
			strings.Sort();
			
			Assert.AreEqual(strings, unsortedStrings);
		}
		
		/// <summary>
		/// Tests that the Controls property is returned in the GetSerializableProperties method.
		/// </summary>
		[Test]
		public void ContainsDesignerSerializationContentProperties()
		{
			Assert.IsTrue(ContainsProperty(propertyDescriptors, "Controls"), "Controls property should be returned.");
		}
		
		static bool ContainsProperty(PropertyDescriptorCollection propertyDescriptors, string name)
		{
			foreach (PropertyDescriptor property in propertyDescriptors) {
				if (property.Name == name) {
					return true;
				}
			}
			return false;
		}
		
		bool HasDesignOnlyAttribute(AttributeCollection attributes)
		{
			foreach (Attribute a in attributes) {
				if (a is DesignOnlyAttribute) {
					return true;
				}
			}
			return false;
		}
	}
}
