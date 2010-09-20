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

namespace WixBinding.Tests.DialogXmlGeneration
{
	/// <summary>
	/// Tests that the NoMinimize attribute is removed after setting the
	/// Form's MinimizeBox back to true. 
	/// </summary>
	[TestFixture]
	public class NoMinimizeBoxChangedTestFixture
	{
		XmlElement dialogElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());

			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog()) {
				dialog.MinimizeBox = true;
				dialogElement = wixDialog.UpdateDialogElement(dialog);
			}
		}
		
		[Test]
		public void DialogMinimizeBox()
		{
			Assert.IsFalse(dialogElement.HasAttribute("NoMinimize"));
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370' NoMinimize='yes'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
