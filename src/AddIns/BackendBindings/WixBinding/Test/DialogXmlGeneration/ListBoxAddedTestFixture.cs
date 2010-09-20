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
	/// Adds a new list box to the dialog.
	/// </summary>
	[TestFixture]
	public class ListBoxAddedTestFixture : DialogLoadingTestFixtureBase
	{
		int listBoxItemCount;
		string controlName;
		string controlType;
		string listBoxItem1Text;
		string listBoxItem2Text;
		
		[SetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				ListBox listBox = new ListBox();
				listBox.Name = "NewListBox";
				listBox.Items.Add("New item1");
				listBox.Items.Add("New item2");
				dialog.Controls.Add(listBox);
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement controlElement = (XmlElement)dialogElement.ChildNodes[0];
				controlName = controlElement.GetAttribute("Id");
				controlType = controlElement.GetAttribute("Type");
				
				XmlElement listBoxElement = (XmlElement)controlElement.ChildNodes[0];
				
				listBoxItemCount = listBoxElement.ChildNodes.Count;
				XmlElement listBoxItem1Element = (XmlElement)listBoxElement.ChildNodes[0];
				listBoxItem1Text = listBoxItem1Element.GetAttribute("Text");
				XmlElement listBoxItem2Element = (XmlElement)listBoxElement.ChildNodes[1];
				listBoxItem2Text = listBoxItem2Element.GetAttribute("Text");
			}
		}
		
		[Test]
		public void TwoListBoxItems()
		{
			Assert.AreEqual(2, listBoxItemCount);
		}

		[Test]
		public void ControlName()
		{
			Assert.AreEqual("NewListBox", controlName);
		}
		
		[Test]
		public void ControlType()
		{
			Assert.AreEqual("ListBox", controlType);
		}
		
		[Test]
		public void ListBoxItem1Text()
		{
			Assert.AreEqual("New item1", listBoxItem1Text);
		}
		
		[Test]
		public void ListBoxItem2Text()
		{
			Assert.AreEqual("New item2", listBoxItem2Text);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
