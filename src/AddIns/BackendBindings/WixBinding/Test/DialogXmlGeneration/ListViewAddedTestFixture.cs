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
	/// Adds a new list view to the dialog.
	/// </summary>
	[TestFixture]
	public class ListViewAddedTestFixture : DialogLoadingTestFixtureBase
	{
		int listViewItemCount;
		string controlName;
		string controlType;
		string listViewItem1Text;
		string listViewItem2Text;
		
		[SetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				ListView listView = new ListView();
				listView.Name = "NewListView";
				listView.Items.Add("New item1");
				listView.Items.Add("New item2");
				dialog.Controls.Add(listView);
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement controlElement = (XmlElement)dialogElement.ChildNodes[0];
				controlName = controlElement.GetAttribute("Id");
				controlType = controlElement.GetAttribute("Type");
				
				XmlElement listViewElement = (XmlElement)controlElement.ChildNodes[0];
				
				listViewItemCount = listViewElement.ChildNodes.Count;
				XmlElement listViewItem1Element = (XmlElement)listViewElement.ChildNodes[0];
				listViewItem1Text = listViewItem1Element.GetAttribute("Text");
				XmlElement listViewItem2Element = (XmlElement)listViewElement.ChildNodes[1];
				listViewItem2Text = listViewItem2Element.GetAttribute("Text");
			}
		}
		
		[Test]
		public void TwoListViewItems()
		{
			Assert.AreEqual(2, listViewItemCount);
		}

		[Test]
		public void ControlName()
		{
			Assert.AreEqual("NewListView", controlName);
		}
		
		[Test]
		public void ControlType()
		{
			Assert.AreEqual("ListView", controlType);
		}
		
		[Test]
		public void ListViewItem1Text()
		{
			Assert.AreEqual("New item1", listViewItem1Text);
		}
		
		[Test]
		public void ListViewItem2Text()
		{
			Assert.AreEqual("New item2", listViewItem2Text);
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
