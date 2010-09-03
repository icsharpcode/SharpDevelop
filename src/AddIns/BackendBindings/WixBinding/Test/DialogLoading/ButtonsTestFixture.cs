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
	/// Tests the loading of a simple Wix dialog that has two buttons.
	/// </summary>
	[TestFixture]
	public class ButtonsTestFixture : DialogLoadingTestFixtureBase
	{
		int controlsAddedCount;
		string cancelButtonName;
		string nextButtonName;
		Point nextButtonLocation;
		Size nextButtonSize;
		string nextButtonText;
		string dialogAcceptButtonName;
		string dialogCancelButtonName;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			controlsAddedCount = 0;
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				foreach (Control control in dialog.Controls) {
					++controlsAddedCount;
				}
				Button nextButton = (Button)dialog.Controls[0];
				nextButtonName = nextButton.Name;
				nextButtonLocation = nextButton.Location;
				nextButtonSize = nextButton.Size;
				nextButtonText = nextButton.Text;
				
				dialogAcceptButtonName = ((Button)dialog.AcceptButton).Name;
				
				Button cancelButton = (Button)dialog.Controls[1];
				cancelButtonName = cancelButton.Name;
				
				dialogCancelButtonName = ((Button)dialog.CancelButton).Name;
			}
		}
		
		[Test]
		public void FormHasTwoControlsAdded()
		{
			Assert.AreEqual(2, controlsAddedCount);
		}
		
		[Test]
		public void CancelButtonName()
		{
			Assert.AreEqual("Cancel", cancelButtonName);
		}
		
		[Test]
		public void NextButtonName()
		{
			Assert.AreEqual("Next", nextButtonName);
		}
		
		[Test]
		public void NextButtonText()
		{
			Assert.AreEqual("[Button_Next]", nextButtonText);
		}
		
		[Test]
		public void NextButtonLocation()
		{
			int expectedX = Convert.ToInt32(236 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(243 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, nextButtonLocation);
		}
		
		[Test]
		public void NextButtonSize()
		{
			int expectedWidth = Convert.ToInt32(60 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(20 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, nextButtonSize);
		}
		
		/// <summary>
		/// Make sure the buttons are created through the IComponentCreator interface.
		/// </summary>
		[Test]
		public void ThreeComponentsCreated()
		{
			Assert.AreEqual(3, CreatedComponents.Count);
		}
		
		[Test]
		public void DialogHasAcceptButton()
		{
			Assert.AreEqual(nextButtonName, dialogAcceptButtonName);
		}
		
		[Test]
		public void DialogHasCancelButton()
		{
			Assert.AreEqual(cancelButtonName, dialogCancelButtonName);
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' X='236' Y='243' Width='60' Height='20' Default='yes' Text='[Button_Next]'/>\r\n" +
				"\t\t\t\t<Control Id='Cancel' Type='PushButton' X='304' Y='243' Width='56' Height='17' Cancel='yes' Text='[Button_Cancel]'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
