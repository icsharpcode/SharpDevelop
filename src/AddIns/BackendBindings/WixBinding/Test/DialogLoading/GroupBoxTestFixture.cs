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
	/// Tests the loading of a simple Wix dialog that contains a group box.
	/// </summary>
	[TestFixture]
	public class GroupBoxTestFixture : DialogLoadingTestFixtureBase
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
				GroupBox groupBox = (GroupBox)dialog.Controls[0];
				name = groupBox.Name;
				text = groupBox.Text;
				location = groupBox.Location;
				size = groupBox.Size;
			}
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("ControlId", name);
		}
		
		[Test]
		public void Text()
		{
			Assert.AreEqual("Text", text);
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
			int expectedWidth = (int)Math.Round(50 * WixDialog.InstallerUnit);
			int expectedHeight = (int)Math.Round(50 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, size);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='ControlId' Type='GroupBox' X='10' Y='10' Width='50' Height='50' Text='Text'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
