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
	/// Tests that a Control with type Icon is not removed from the dialog xml
	/// after viewing the dialog in the designer and then switching back to source code.
	/// </summary>
	[TestFixture]
	public class DialogWithIconControlTypeOpenedAndClosedInDesignerTests
	{
		XmlElement dialogElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());

			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog()) {
				dialog.Text = "New dialog title";
				dialog.ClientSize = new Size(200, 100);
				dialogElement = wixDialog.UpdateDialogElement(dialog);
			}
		}
		
		[Test]
		public void UpdateDialogElement_GetIconControl_IconControlStillExists()
		{
			var namespaceManager = new WixNamespaceManager(dialogElement.OwnerDocument.NameTable);
			var control = dialogElement.SelectSingleNode("w:Control", namespaceManager) as XmlElement;
			string id = control.GetAttribute("Id");
			
			Assert.AreEqual("MyIcon", id);
		}
		
		string GetWixXml()
		{
			return
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"        <UI>\r\n" +
				"            <Dialog Id='WelcomeDialog' Height='270' Width='370' Title='Welcome Dialog Title'>\r\n" +
				"                <Control Id='MyIcon' Type='Icon' X='20' Y='60' Width='24' Height='24' FixedSize='yes' IconSize='16' Text='IconName'>\r\n" +
				"                </Control>\r\n" +
				"            </Dialog>\r\n" +
				"        </UI>\r\n" +
				"    </Fragment>\r\n" +
				"</Wix>";
		}
	}
}
