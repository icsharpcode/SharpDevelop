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
	/// Tests the loading of a simple Wix dialog that contains a combo box.
	/// </summary>
	[TestFixture]
	public class ComboBoxTestFixture : DialogLoadingTestFixtureBase
	{
		string name;
		string text;
		Point location;
		Size size;
		int itemCount;
		string item1Text;
		string item2Text;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CreatedComponents.Clear();
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				ComboBox comboBox = (ComboBox)dialog.Controls[0];
				name = comboBox.Name;
				text = comboBox.Text;
				location = comboBox.Location;
				size = comboBox.Size;
				
				// Combo box items.
				itemCount = comboBox.Items.Count;
				item1Text = (string)comboBox.Items[0];
				item2Text = (string)comboBox.Items[1];
			}
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("ControlId", name);
		}
		
		/// <summary>
		/// Text is set to the property value.
		/// </summary>
		[Test]
		public void Text()
		{
			Assert.AreEqual("ComboBoxProperty", text);
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
			int expectedHeight = Convert.ToInt32(16 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, size);
		}
		
		[Test]
		public void TwoItemsAdded()
		{
			Assert.AreEqual(2, itemCount);
		}
		
		[Test]
		public void Item1Text()
		{
			Assert.AreEqual("Item1", item1Text);
		}
		
		[Test]
		public void Item2Text()
		{
			Assert.AreEqual("Item2", item2Text);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='ControlId' Type='ComboBox' X='10' Y='10' Width='50' Height='16' Property='ComboBoxProperty' Text='Text'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<ComboBox Property='ComboBoxProperty'>\r\n" +
				"\t\t\t\t<ListItem Text='Item1' Value='Yes'/>\r\n" +
				"\t\t\t\t<ListItem Value='No'>\r\n" +
				"\t\t\t\t\t<Text>Item2</Text>\r\n" +
				"\t\t\t\t</ListItem>\r\n" +
				"\t\t\t</ComboBox>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
