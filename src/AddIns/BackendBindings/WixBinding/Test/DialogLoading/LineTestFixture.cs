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
	/// Tests the loading of a simple Wix dialog that contains a line.
	/// </summary>
	[TestFixture]
	public class LineTestFixture : DialogLoadingTestFixtureBase
	{
		string lineName;
		Point lineLocation;
		BorderStyle lineBorder;
		Size lineSize;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CreatedComponents.Clear();
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				Label line = (Label)dialog.Controls[0];
				lineName = line.Name;
				lineLocation = line.Location;
				lineBorder = line.BorderStyle;
				lineSize = line.Size;
			}
		}
		
		[Test]
		public void LineName()
		{
			Assert.AreEqual("BottomLine", lineName);
		}
		
		[Test]
		public void TwoControlsCreated()
		{
			Assert.AreEqual(2, CreatedComponents.Count);
		}
		
		[Test]
		public void LineLocation()
		{
			int expectedX = Convert.ToInt32(10 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(234 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, lineLocation);
		}
		
		[Test]
		public void LineSize()
		{
			int expectedWidth = Convert.ToInt32(360 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(2 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, lineSize);
		}
		
		/// <summary>
		/// Should be fixed 3d since this gives the best looking line.
		/// </summary>
		[Test]
		public void LineBorder()
		{
			Assert.AreEqual(BorderStyle.Fixed3D, lineBorder);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='BottomLine' Type='Line' X='10' Y='234' Width='360' Height='0' />\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
