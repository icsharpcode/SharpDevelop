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
	/// Tests that the transparent labels have their alpha part of their BackColor
	/// set to zero.
	/// </summary>
	[TestFixture]
	public class TransparentLabelsTestFixture : DialogLoadingTestFixtureBase
	{
		Color titleLabelColor;
		Color descriptionLabelColor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CreatedComponents.Clear();
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				Label titleLabel = (Label)dialog.Controls[0];
				titleLabelColor = titleLabel.BackColor;
				
				Label descriptionLabel = (Label)dialog.Controls[1];
				descriptionLabelColor = descriptionLabel.BackColor;
			}
		}
		
		[Test]
		public void TitleLabelBackColourIsTransparent()
		{
			Assert.AreEqual(Color.FromArgb(0, SystemColors.Control), titleLabelColor);
		}
		
		[Test]
		public void DescriptionLabelBackColorIsNotTransparent()
		{
			Assert.AreEqual(SystemColors.Control, descriptionLabelColor);
		}
	
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<TextStyle Id='BigFont' FaceName='Verdana' Size='13' Bold='yes' />\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Title' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='yes' NoPrefix='yes' Text='test'/>\r\n" +
				"\t\t\t\t<Control Id='Description' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='no' NoPrefix='yes' Text='test'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
