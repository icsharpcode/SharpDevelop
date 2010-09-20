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
	/// Tests the loading of a simple Wix dialog that contains a check box.
	/// </summary>
	[TestFixture]
	public class CheckBoxTestFixture : DialogLoadingTestFixtureBase
	{
		string name;
		string text;
		Point location;
		Size size;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CreatedComponents.Clear();
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				CheckBox checkBox = (CheckBox)dialog.Controls[0];
				name = checkBox.Name;
				text = checkBox.Text;
				location = checkBox.Location;
				size = checkBox.Size;
			}
		}
		
		[Test]
		public void CheckBoxName()
		{
			Assert.AreEqual("Check", name);
		}
		
		[Test]
		public void CheckBoxText()
		{
			Assert.AreEqual("CheckBoxText", text);
		}
		
		[Test]
		public void TwoControlsCreated()
		{
			Assert.AreEqual(2, CreatedComponents.Count);
		}
		
		[Test]
		public void CheckBoxLocation()
		{
			int expectedX = Convert.ToInt32(10 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(10 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, location);
		}
		
		[Test]
		public void CheckBoxSize()
		{
			int expectedWidth = Convert.ToInt32(50 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(20 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, size);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Check' Type='CheckBox' X='10' Y='10' Width='50' Height='20' Text='CheckBoxText'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
