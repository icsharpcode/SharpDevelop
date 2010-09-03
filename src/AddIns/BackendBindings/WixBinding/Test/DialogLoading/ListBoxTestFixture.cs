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
	/// Tests the loading of a simple Wix dialog that contains a list box.
	/// </summary>
	[TestFixture]
	public class ListBoxTestFixture : DialogLoadingTestFixtureBase
	{
		string name;
		Point location;
		Size size;
		int listItemCount;
		string listItem1Text;
		string listItem2Text;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CreatedComponents.Clear();
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				ListBox listBox = (ListBox)dialog.Controls[0];
				name = listBox.Name;
				location = listBox.Location;
				size = listBox.Size;
				
				// List items.
				listItemCount = listBox.Items.Count;
				listItem1Text = (string)listBox.Items[0];
				listItem2Text = (string)listBox.Items[1];
			}
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("ControlId", name);
		}
		
		[Test]
		public void TwoControlsCreated()
		{
			Assert.AreEqual(2, CreatedComponents.Count);
		}
		
		[Test]
		public void Location()
		{
			int expectedX = Convert.ToInt32(10 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(10 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, location);
		}
		
		[Test]
		public void Size()
		{
			int expectedWidth = Convert.ToInt32(50 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(50 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, size);
		}
		
		[Test]
		public void TwoListItemsAdded()
		{
			Assert.AreEqual(2, listItemCount);
		}
		
		[Test]
		public void ListItem1Text()
		{
			Assert.AreEqual("ListItem1", listItem1Text);
		}
		
		[Test]
		public void ListItem2Text()
		{
			Assert.AreEqual("ListItem2", listItem2Text);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='ControlId' Type='ListBox' X='10' Y='10' Width='50' Height='50' Property='ListBoxProperty'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<ListBox Property='ListBoxProperty'>\r\n" +
				"\t\t\t\t<ListItem Text='ListItem1' Value='Yes'/>\r\n" +
				"\t\t\t\t<ListItem Value='No'>\r\n" +
				"\t\t\t\t\t<Text>ListItem2</Text>\r\n" +
				"\t\t\t\t</ListItem>\r\n" +
				"\t\t\t</ListBox>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
