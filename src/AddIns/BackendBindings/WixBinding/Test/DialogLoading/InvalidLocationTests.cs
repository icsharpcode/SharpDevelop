// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;
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
			SD.InitializeForUnitTests();
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
