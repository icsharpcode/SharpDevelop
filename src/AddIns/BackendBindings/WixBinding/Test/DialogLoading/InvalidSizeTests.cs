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
	/// invalid height and width.
	/// </summary>
	[TestFixture]
	public class InvalidSizeTests
	{
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			WixBindingTestsHelper.RegisterResourceStringsWithSharpDevelopResourceManager();
		}
		
		[Test]
		public void MissingHeight()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project);
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Width='370'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			doc.LoadXml(xml);
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			try {
				wixDialog.CreateDialog();
				Assert.Fail("Expected an exception before this line.");
			} catch (WixDialogException ex) {
				Assert.AreEqual("Dialog", ex.ElementName);
				Assert.AreEqual("WelcomeDialog", ex.Id);
				Assert.AreEqual("Required attribute 'Height' is missing.", ex.Message);
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
				"\t\t\t<Dialog Id='WelcomeDialog' Height='370'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			doc.LoadXml(xml);
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			try {
				wixDialog.CreateDialog();
				Assert.Fail("Expected an exception before this line.");
			} catch (WixDialogException ex) {
				Assert.AreEqual("Dialog", ex.ElementName);
				Assert.AreEqual("WelcomeDialog", ex.Id);
				Assert.AreEqual("Required attribute 'Width' is missing.", ex.Message);
			}
		}
		
		[Test]
		public void InvalidHeight()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project);
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='f' Width='370'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			doc.LoadXml(xml);
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			try {
				wixDialog.CreateDialog();
				Assert.Fail("Expected an exception before this line.");
			} catch (WixDialogException ex) {
				Assert.AreEqual("Dialog", ex.ElementName);
				Assert.AreEqual("WelcomeDialog", ex.Id);
				Assert.AreEqual("The Height attribute's value, 'f', is not a legal integer value.", ex.Message);
			}
		}
		
		[Test]
		public void InvalidWidth()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project);
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='100' Width='f'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			doc.LoadXml(xml);
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			try {
				wixDialog.CreateDialog();
				Assert.Fail("Expected an exception before this line.");
			} catch (WixDialogException ex) {
				Assert.AreEqual("Dialog", ex.ElementName);
				Assert.AreEqual("WelcomeDialog", ex.Id);
				Assert.AreEqual("The Width attribute's value, 'f', is not a legal integer value.", ex.Message);
			}
		}
	}
}
