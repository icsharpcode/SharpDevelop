// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WixBinding;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests the loading of a simple Wix dialog that has a radio button group.
	/// </summary>
	[TestFixture]
	public class RadioButtonGroupTestFixture : DialogLoadingTestFixtureBase
	{
		int controlsAddedCount;
		Point radioButtonGroupLocation;
		Size radioButtonGroupSize;
		string radioButtonGroupName;
		string acceptRadioButtonName;
		string declineRadioButtonName;
		string radioButtonGroupPropertyName;
		Point acceptRadioButtonLocation;
		Point declineRadioButtonLocation;
		Size acceptRadioButtonSize;
		Size declineRadioButtonSize;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			controlsAddedCount = 0;
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("AcceptLicenseDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				foreach (Control control in dialog.Controls) {
					++controlsAddedCount;
				}
				RadioButtonGroupBox radioButtonGroup = (RadioButtonGroupBox)dialog.Controls[0];
				radioButtonGroupName = radioButtonGroup.Name;
				radioButtonGroupPropertyName = radioButtonGroup.PropertyName;
				radioButtonGroupLocation = radioButtonGroup.Location;
				radioButtonGroupSize = radioButtonGroup.Size;
				
				RadioButton acceptRadioButton = (RadioButton)radioButtonGroup.Controls[0];
				acceptRadioButtonName = acceptRadioButton.Name;
				acceptRadioButtonLocation = acceptRadioButton.Location;
				acceptRadioButtonSize = acceptRadioButton.Size;
				
				RadioButton declineRadioButton = (RadioButton)radioButtonGroup.Controls[1];
				declineRadioButtonName = declineRadioButton.Name;
				declineRadioButtonLocation = declineRadioButton.Location;
				declineRadioButtonSize = declineRadioButton.Size;
			}
		}
		
		[Test]
		public void FormHasOneControlAdded()
		{
			Assert.AreEqual(1, controlsAddedCount);
		}
		
		[Test]
		public void RadioButtonGroupName()
		{
			Assert.AreEqual("Buttons", radioButtonGroupName);
		}
		
		[Test]
		public void RadioButtonGroupPropertyName()
		{
			Assert.AreEqual("AcceptLicense", radioButtonGroupPropertyName);
		}
		
		[Test]
		public void AcceptRadioButtonName()
		{
			Assert.AreEqual("AcceptLicenseRadioButton1", acceptRadioButtonName);
		}	
				
		[Test]
		public void DeclineRadioButtonName()
		{
			Assert.AreEqual("AcceptLicenseRadioButton2", declineRadioButtonName);
		}	
		
		[Test]
		public void RadioButtonGroupBoxLocation()
		{
			int expectedX = Convert.ToInt32(20 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(187 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, radioButtonGroupLocation);
		}
	
		[Test]
		public void RadioButtonGroupBoxSize()
		{
			int expectedWidth = Convert.ToInt32(330 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(40 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, radioButtonGroupSize);
		}
		
		[Test]
		public void AcceptRadioButtonLocation()
		{
			int expectedX = Convert.ToInt32(5 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(0 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, acceptRadioButtonLocation);
		}
	
		[Test]
		public void AcceptRadioButtonSize()
		{
			int expectedWidth = Convert.ToInt32(300 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(15 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, acceptRadioButtonSize);
		}		

		[Test]
		public void DeclineRadioButtonLocation()
		{
			int expectedX = Convert.ToInt32(5 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(20 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, declineRadioButtonLocation);
		}
	
		[Test]
		public void DeclineRadioButtonSize()
		{
			int expectedWidth = Convert.ToInt32(300 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(15 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, declineRadioButtonSize);
		}				
		
		/// <summary>
		/// Make sure the two radio buttons, one form and a group box are created through 
		/// the IComponentCreator interface.
		/// </summary>
		[Test]
		public void FourComponentsCreated()
		{
			Assert.AreEqual(4, CreatedComponents.Count);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='AcceptLicenseDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Buttons' Type='RadioButtonGroup' X='20' Y='187' Width='330' Height='40' Property='AcceptLicense'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<RadioButtonGroup Property='AcceptLicense'>\r\n" +
				"\t\t\t\t<RadioButton Text='I accept' X='5' Y='0' Width='300' Height='15' Value='Yes'/>\r\n" +
				"\t\t\t\t<RadioButton Text='I do not accept' X='5' Y='20' Width='300' Height='15'  Value='No'/>\r\n" +
				"\t\t\t</RadioButtonGroup>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
