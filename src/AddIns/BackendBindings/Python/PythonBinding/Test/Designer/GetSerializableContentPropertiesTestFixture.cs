// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Gets properties that are marked as DesignerSerializationVisibility.Content
	/// </summary>
	[TestFixture]
	public class GetSerializableContentPropertiesTestFixture
	{
		PropertyDescriptorCollection properties;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (Form form = new Form()) {
				// Modify Form.Text so it is identified as needing serialization.
				form.Text = "abc";
				properties = PythonForm.GetSerializableContentProperties(form);
			}
		}
		
		[Test]
		public void FormControlsPropertyReturned()
		{
			Assert.IsNotNull(properties.Find("Controls", false), "Property not found: Controls");
		}
		
		[Test]
		public void FormTextPropertyIsNotReturned()
		{
			Assert.IsNull(properties.Find("Text", false), "Property should not be found: Text");
		}
	}
}
