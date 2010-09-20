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
