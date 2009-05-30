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
	/// Tests that the grid column widths are remembered by the query control.
	/// </summary>
	[TestFixture]
	public class NamespaceGridColumnWidthsLoadedTestFixture
	{
		int prefixColumnWidthAfterLoad;
		int prefixColumnWidthAfterSave;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (XPathQueryControl queryControl = new XPathQueryControl()) {
				Properties p = new Properties();
				p.Set("NamespacesDataGridView.PrefixColumn.Width", 10);
				queryControl.SetMemento(p);

				prefixColumnWidthAfterLoad = queryControl.NamespacesDataGridView.Columns["prefixColumn"].Width;
	
				queryControl.NamespacesDataGridView.Columns["prefixColumn"].Width = 40;

				p = queryControl.CreateMemento();
				prefixColumnWidthAfterSave = p.Get<int>("NamespacesDataGridView.PrefixColumn.Width", 0);
			}
		}
		
		[Test]
		public void PrefixColumnWidthAfterLoad()
		{
			Assert.AreEqual(10, prefixColumnWidthAfterLoad);
		}
		
		[Test]
		public void PrefixColumnWidthAfterSave()
		{
			Assert.AreEqual(40, prefixColumnWidthAfterSave);
		}
	}
}
