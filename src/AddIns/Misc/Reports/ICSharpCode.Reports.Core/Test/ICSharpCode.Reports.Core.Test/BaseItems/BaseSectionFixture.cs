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

using System;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseSectionFixture
	{
		#region constructor
		
		[Test]
		public void PlainConstructor()
		{
			BaseSection section = new BaseSection();
			Assert.AreEqual(String.Empty,section.Name);
		}
		
		
		[Test]
		public void ConstructorWithNameAsParam ()
		{
			BaseSection section = new BaseSection("mysection");
			Assert.AreEqual("mysection",section.Name);
		}
			
		#endregion
		
		#region initialvalues
		
		[Test]
		public void CheckInitialValues ()
		{
			BaseSection section = new BaseSection();
			Assert.AreEqual(0,section.Items.Count);
			Assert.AreEqual(0,section.SectionMargin);
			Assert.AreEqual(0,section.SectionOffset);
			Assert.AreEqual(false,section.PageBreakAfter);
			Assert.AreEqual(false,section.DrawBorder);
			Assert.AreEqual(System.Drawing.Color.White,section.BackColor);
		}
		
		#endregion
		
		[Test]
		public void FindItem_NoContainer()
		{
			BaseSection section = ConfigurePlainSection();
			BaseReportItem item = section.FindItem("ImageItem");
			Assert.That(item != null);
			Assert.IsInstanceOf(typeof(BaseImageItem),item);
			Assert.That(item.Name == "ImageItem");
		}
		
		
		[Test]
		public void FindItemNullIfItemNotExist()
		{
			BaseSection section = ConfigurePlainSection();
			BaseReportItem item = section.FindItem("notexist");
			Assert.IsNull(item);
		}
		
		
		[Test]
		public void FindItemInRow ()
		{
			BaseSection section = ConfigureSectionWithRow();
			BaseReportItem item = section.FindItem("TextItem");
			Assert.That(item != null);
			Assert.IsInstanceOf(typeof(BaseTextItem),item);
			Assert.That(item.Name == "TextItem");
		}
		
		
		[Test]
		public void FindItemInRowNullIfItemNotExist ()
		{
			BaseSection section = ConfigureSectionWithRow();
			BaseReportItem item = section.FindItem("notexist");
			Assert.IsNull(item);
		}
		
		private BaseSection ConfigureSectionWithRow ()
		{
			BaseSection section = new BaseSection();
			BaseRowItem row = new BaseRowItem();
			BaseTextItem bti = new BaseTextItem{
				Name = "TextItem"
			};
			
			BaseImageItem bii = new BaseImageItem(){
				Name = "ImageItem"
			};
			row.Items.Add(bti);
			row.Items.Add(bii);
			section.Items.Add(row);
			return section;
		}
		
		
		private BaseSection ConfigurePlainSection ()
		{
			BaseSection section = new BaseSection();
			BaseTextItem bti = new BaseTextItem{
				Name = "TextItem"
			};
			
			BaseImageItem bii = new BaseImageItem(){
				Name = "ImageItem"
			};
			section.Items.Add(bti);
			section.Items.Add(bii);
			return section;
		}
			
		
		#region
		#endregion
		
		[TestFixtureSetUp]
		public void Init()
		{
			// TODO: Add Init code.
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
		
		
	}
}
