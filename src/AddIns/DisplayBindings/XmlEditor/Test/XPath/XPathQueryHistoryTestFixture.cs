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

using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace XmlEditor.Tests.XPath
{
	/// <summary>
	/// Tests that the XPath queries are remembered.
	/// </summary>
	[TestFixture]
	public class XPathQueryHistoryTestFixture
	{		
		string[] expectedXPathsAfterLoad;
		string[] comboBoxItemsAfterLoad;
		string comboBoxTextAfterLoad;
		string[] expectedXPathsAfterSave;
		string xpathQueryAfterSave;
		string[] xpathsAfterSave;
		
		[SetUp]
		public void SetUpFixture()
		{
			using (XPathQueryControl queryControl = new XPathQueryControl()) {
				Properties p = new Properties();
				p.Set("XPathQuery.LastQuery", "//w:Wix");
				expectedXPathsAfterLoad = new string[] {"//w:Fragment", "//w:Dialog"};
				p.SetList("XPathQuery.History", expectedXPathsAfterLoad);
				queryControl.SetMemento(p);

				comboBoxTextAfterLoad = queryControl.XPathComboBox.Text;
				comboBoxItemsAfterLoad = GetComboBoxItems(queryControl.XPathComboBox);

				queryControl.XPathComboBox.Text = "*";
				queryControl.XPathComboBox.Items.Clear();
				queryControl.XPathComboBox.Items.Add("xs:schema");
				expectedXPathsAfterSave = GetComboBoxItems(queryControl.XPathComboBox);

				p = queryControl.CreateMemento();
				
				xpathQueryAfterSave = p.Get("XPathQuery.LastQuery", String.Empty);
				xpathsAfterSave = p.GetList<string>("XPathQuery.History").ToArray();
			}
		}
		
		string[] GetComboBoxItems(ComboBox comboBox)
		{
			List<string> items = new List<string>();
			foreach (string item in comboBox.Items) {
				items.Add(item);
			}
			return items.ToArray();
		}
		
		[Test]
		public void ComboBoxTextAfterLoad()
		{
			Assert.AreEqual("//w:Wix", comboBoxTextAfterLoad);
		}
		
		[Test]
		public void ComboBoxItemsAfterLoad()
		{
			for (int i = 0; i < expectedXPathsAfterLoad.Length; ++i) {
				Assert.AreEqual(expectedXPathsAfterLoad[i], comboBoxItemsAfterLoad[i]);
			}
		}
		
		[Test]
		public void XPathQueryTextAfterSave()
		{
			Assert.AreEqual("*", xpathQueryAfterSave);
		}
		
		[Test]
		public void XPathsAfterSave()
		{
			for (int i = 0; i < expectedXPathsAfterSave.Length; ++i) {
				Assert.AreEqual(expectedXPathsAfterSave[i], xpathsAfterSave[i]);
			}
		}
	}
}
