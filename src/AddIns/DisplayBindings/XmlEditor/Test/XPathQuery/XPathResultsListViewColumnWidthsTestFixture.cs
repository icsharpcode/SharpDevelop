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
	/// Tests that the XPath results list view column widths are remembered.
	/// </summary>
	[TestFixture]
	public class XPathResultsListViewColumnWidthsTestFixture
	{
		int matchColumnWidthAfterLoad;
		int lineColumnWidthAfterLoad;
		int matchColumnWidthAfterSave;
		int lineColumnWidthAfterSave;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (XPathQueryControl queryControl = new XPathQueryControl()) {
				Properties p = new Properties();
				p.Set("XPathResultsListView.MatchColumn.Width", 10);
				p.Set("XPathResultsListView.LineColumn.Width", 20);
				queryControl.SetMemento(p);

				matchColumnWidthAfterLoad = queryControl.XPathResultsListView.Columns[0].Width;
				lineColumnWidthAfterLoad = queryControl.XPathResultsListView.Columns[1].Width;
				
				queryControl.XPathResultsListView.Columns[0].Width = 40;
				queryControl.XPathResultsListView.Columns[1].Width = 50;

				p = queryControl.CreateMemento();
				matchColumnWidthAfterSave = p.Get<int>("XPathResultsListView.MatchColumn.Width", 0);
				lineColumnWidthAfterSave = p.Get<int>("XPathResultsListView.LineColumn.Width", 0);
			}
		}
		
		[Test]
		public void MatchColumnWidthAfterLoad()
		{
			Assert.AreEqual(10, matchColumnWidthAfterLoad);
		}

		[Test]
		public void LineColumnWidthAfterLoad()
		{
			Assert.AreEqual(20, lineColumnWidthAfterLoad);
		}

		[Test]
		public void MatchColumnWidthAfterSave()
		{
			Assert.AreEqual(40, matchColumnWidthAfterSave);
		}
		
		[Test]
		public void LineColumnWidthAfterSave()
		{
			Assert.AreEqual(50, lineColumnWidthAfterSave);
		}
	}
}
