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
