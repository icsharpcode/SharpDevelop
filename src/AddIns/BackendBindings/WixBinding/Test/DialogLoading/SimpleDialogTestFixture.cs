// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WixBinding;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests the loading of a simple Wix dialog that has just an id, height and
	/// width.
	/// </summary>
	[TestFixture]
	public class SimpleDialogTestFixture
	{
		string dialogName;
		FormBorderStyle borderStyle;
		Size clientSize;
		bool maximizeBox;
		bool minimizeBox;
		WixDocument doc;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.GetDialog("WelcomeDialog");
			using (Form simpleDialog = wixDialog.CreateDialog()) {
				dialogName = simpleDialog.Name;
				borderStyle = simpleDialog.FormBorderStyle;
				clientSize = simpleDialog.ClientSize;
				minimizeBox = simpleDialog.MinimizeBox;
				maximizeBox = simpleDialog.MaximizeBox;
			}
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("WelcomeDialog", dialogName);
		}
		
		[Test]
		public void FixedDialogBorder()
		{
			Assert.AreEqual(FormBorderStyle.FixedDialog, borderStyle);
		}
		
		[Test]
		public void ClientSizeHeight()
		{
			int expectedHeight = Convert.ToInt32(270 * WixDialog.InstallerUnit);
			Assert.AreEqual(expectedHeight, clientSize.Height);
		}
		
		[Test]
		public void ClientSizeWidth()
		{
			int expectedWidth = Convert.ToInt32(370 * WixDialog.InstallerUnit);
			Assert.AreEqual(expectedWidth, clientSize.Width);
		}
		
		[Test]
		public void MinimizeBox()
		{
			Assert.IsTrue(minimizeBox);
		}
		
		[Test]
		public void MaximizeBox()
		{
			Assert.IsFalse(maximizeBox);
		}
		
		[Test]
		public void DialogIdWithSingleQuote()
		{
			Assert.IsNull(doc.GetDialog("Test'Id"));
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
