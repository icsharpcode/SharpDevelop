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

namespace WixBinding.Tests.DialogXmlGeneration
{
	/// <summary>
	/// One list item is removed from the list box.
	/// </summary>
	[TestFixture]
	public class ListBoxItemRemovedTestFixture : DialogLoadingTestFixtureBase
	{
		string listBoxItem1Text;
		int listBoxItemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				ListBox listBox = (ListBox)dialog.Controls[0];
				listBox.Items.RemoveAt(0);
			
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement listBoxElement = (XmlElement)dialogElement.SelectSingleNode("//w:ListBox[@Property='ListBoxProperty']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				listBoxItemCount = listBoxElement.ChildNodes.Count;
				
				XmlElement listBoxItem1Element = (XmlElement)listBoxElement.ChildNodes[0];
				listBoxItem1Text = listBoxItem1Element.GetAttribute("Text");
			}
		}

		[Test]
		public void OneListItem()
		{
			Assert.AreEqual(1, listBoxItemCount);
		}
		
		[Test]
		public void ListBoxItem1Text()
		{
			Assert.AreEqual("second", listBoxItem1Text);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='ListBox1' Type='ListBox' X='20' Y='187' Width='330' Height='40' Property='ListBoxProperty'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<ListBox Property='ListBoxProperty'>\r\n" +
				"\t\t\t\t<ListItem Text='first'/>\r\n" +
				"\t\t\t\t<ListItem Text='second'/>\r\n" +
				"\t\t\t</ListBox>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
