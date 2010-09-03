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
	/// Tests the loading of a simple Wix dialog that has a radio button group but
	/// the group is missing from the document.
	/// </summary>
	[TestFixture]
	public class MissingRadioButtonsGroupTestFixture : DialogLoadingTestFixtureBase
	{
		int controlsAddedCount;
		string radioButtonGroupName;
		int radioButtonGroupChildControlsAdded;
		
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
				
				Panel radioButtonGroup = (Panel)dialog.Controls[0];
				radioButtonGroupName = radioButtonGroup.Name;
				
				foreach (Control child in radioButtonGroup.Controls) {
					++radioButtonGroupChildControlsAdded;
				}
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
		public void TwoComponentsCreated()
		{
			Assert.AreEqual(2, CreatedComponents.Count);
		}
		
		[Test]
		public void RadioButtonGroupHasNoControls()
		{
			Assert.AreEqual(0, radioButtonGroupChildControlsAdded);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='AcceptLicenseDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Buttons' Type='RadioButtonGroup' X='20' Y='187' Width='330' Height='40' Property='AcceptLicense'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
