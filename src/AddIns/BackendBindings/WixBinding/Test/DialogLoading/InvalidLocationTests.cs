// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using WixBinding;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests that we get WixDialogExceptions with detailed information about 
	/// invalid X and Y location.
	/// </summary>
	[TestFixture]
	public class InvalidLocationTests
	{
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			WixBindingTestsHelper.RegisterResourceStringsWithSharpDevelopResourceManager();
		}
		
		[Test]
		public void MissingX()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project);
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='200' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' Y='243' Width='60' Height='20' Text='Next'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			doc.LoadXml(xml);
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			try {
				wixDialog.CreateDialog();
				Assert.Fail("Expected an exception before this line.");
			} catch (WixDialogException ex) {
				Assert.AreEqual("Control", ex.ElementName);
				Assert.AreEqual("Next", ex.Id);
				Assert.AreEqual("Required attribute 'X' is missing.", ex.Message);
			}
		}
		
		[Test]
		public void MissingWidth()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project);
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='200' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' X='236' Width='60' Height='20' Text='Next'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			doc.LoadXml(xml);
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			try {
				wixDialog.CreateDialog();
				Assert.Fail("Expected an exception before this line.");
			} catch (WixDialogException ex) {
				Assert.AreEqual("Control", ex.ElementName);
				Assert.AreEqual("Next", ex.Id);
				Assert.AreEqual("Required attribute 'Y' is missing.", ex.Message);
			}
		}
	}
}
