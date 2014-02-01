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
	/// Tests the loading of a simple Wix dialog that has a radio button group but
	/// the group is missing from the document.
	/// </summary>
	[TestFixture]
	public class MissingRadioButtonsGroupTestFixture : DialogLoadingTestFixtureBase
	{
		int controlsAddedCount;
		string radioButtonGroupName;
		int radioButtonGroupChildControlsAdded;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			controlsAddedCount = 0;
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("AcceptLicenseDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				foreach (Control control in dialog.Controls) {
					++controlsAddedCount;
				}
				
				Panel radioButtonGroup = (Panel)dialog.Controls[0];
				radioButtonGroupName = radioButtonGroup.Name;
				
				foreach (Control child in radioButtonGroup.Controls) {
					++radioButtonGroupChildControlsAdded;
				}
			}
		}
		
		[Test]
		public void FormHasOneControlAdded()
		{
			Assert.AreEqual(1, controlsAddedCount);
		}
		
		[Test]
		public void RadioButtonGroupName()
		{
			Assert.AreEqual("Buttons", radioButtonGroupName);
		}
		
		[Test]
		public void TwoComponentsCreated()
		{
			Assert.AreEqual(2, CreatedComponents.Count);
		}
		
		[Test]
		public void RadioButtonGroupHasNoControls()
		{
			Assert.AreEqual(0, radioButtonGroupChildControlsAdded);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='AcceptLicenseDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Buttons' Type='RadioButtonGroup' X='20' Y='187' Width='330' Height='40' Property='AcceptLicense'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
