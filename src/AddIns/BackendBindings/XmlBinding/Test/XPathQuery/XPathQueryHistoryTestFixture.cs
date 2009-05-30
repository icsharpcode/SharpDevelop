// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1662 $</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace XmlEditor.Tests.XPathQuery
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
				p.Set("XPathQuery.History", expectedXPathsAfterLoad);
				queryControl.SetMemento(p);

				comboBoxTextAfterLoad = queryControl.XPathComboBox.Text;
				comboBoxItemsAfterLoad = GetComboBoxItems(queryControl.XPathComboBox);

				queryControl.XPathComboBox.Text = "*";
				queryControl.XPathComboBox.Items.Clear();
				queryControl.XPathComboBox.Items.Add("xs:schema");
				expectedXPathsAfterSave = GetComboBoxItems(queryControl.XPathComboBox);

				p = queryControl.CreateMemento();
				
				xpathQueryAfterSave = p.Get("XPathQuery.LastQuery", String.Empty);
				xpathsAfterSave = p.Get("XPathQuery.History", new string[0]);
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
