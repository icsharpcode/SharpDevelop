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
using System.Linq;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Basics
{
	
	[TestFixture]
	public class ReportItemCollectionFixture
	{
		
		private ReportItemCollection itemCollection;
		
		
		[Test]
		public void Constructor ()
		{
			ReportItemCollection it = new ReportItemCollection();
			Assert.IsNotNull(it,"Should not be 'null'");
			Assert.AreEqual(0,it.Count,"Count should be '0'");
		}
		
		
		[Test]
		public void CollectionLength()
		{
			itemCollection = this.PlainCollection();
			Assert.AreEqual(3,itemCollection.Count());
		}
		
		#region Find
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsNull ()
		{
			itemCollection = this.PlainCollection();
			BaseReportItem r = itemCollection.Find(null);
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsEmpty ()
		{
			itemCollection = this.PlainCollection();
			BaseReportItem r = itemCollection.Find(String.Empty);
		}
		
		
		[Test]
		public void PlainCollectionFind ()
		{
			this.itemCollection = this.PlainCollection();
			BaseReportItem r = itemCollection.Find("t2");
			Assert.AreEqual("t2",r.Name);
		}
		
		#endregion
		
		#region Insert
		
		[Test]
		public void InsertOneItem ()
		{
			itemCollection = this.PlainCollection();
			int len = itemCollection.Count();
			BaseReportItem r = new BaseReportItem();
			r.Name = "Inserted";
			itemCollection.Insert(1,r);
			Assert.AreEqual (len + 1,itemCollection.Count());
		
			// read inserted element and check some default values
			BaseReportItem r1 = itemCollection.Find("Inserted");
			Assert.AreEqual(GlobalValues.DefaultBackColor,r1.BackColor);
			Assert.AreEqual(System.Drawing.Color.Black,r1.ForeColor);
		}
		
		#endregion
		
		#region delete/remove
		
		[Test]
		public void RemoveFirstElementAndCheckForEvent ()
		{
			itemCollection = this.PlainCollection();
			int i = itemCollection.Count;
			itemCollection.RemoveAt(1);
			Assert.AreEqual(i-1,itemCollection.Count) ;
		}
		
		
		[Test]
		public void RemoveElementAndCheckForEvent ()
		{
			itemCollection = this.PlainCollection();
			int i = itemCollection.Count;
			BaseReportItem r = itemCollection[1];
			Assert.AreEqual("t2",r.Name);
			itemCollection.Remove (r);
			Assert.AreEqual(i-1,itemCollection.Count);
		}
		
		
		[Test]
		public void ClearAllItems ()
		{
			itemCollection = this.PlainCollection();
			Assert.AreEqual(3,itemCollection.Count);
			itemCollection.Clear();
			Assert.AreEqual(0,itemCollection.Count);
		}
		#endregion
		
		#region Exist
		
		[Test]
		public void ExistTest()
		{
			this.itemCollection = this.PlainCollection();
			Assert.AreEqual (true,this.itemCollection.Exist("t3"));
		}
		
		
		[Test]
		public void NotExistTest()
		{
			this.itemCollection = this.PlainCollection();
			Assert.AreEqual (false,this.itemCollection.Exist("t8"));
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExistWithNullString ()
		{
			this.itemCollection = this.PlainCollection();
			Assert.AreEqual (false,this.itemCollection.Exist(String.Empty));
		}
		
		#endregion
		
		#region privates
		
		private ReportItemCollection PlainCollection()
		{
			ReportItemCollection ri = new ReportItemCollection();
			BaseTextItem t1 = new BaseTextItem();
			t1.Name = "t1";
			ri.Add(t1);
			 t1 = new BaseTextItem();
			t1.Name = "t2";
			ri.Add(t1);
			t1 = new BaseTextItem();
			t1.Name = "t3";
			ri.Add(t1);
			return ri;
		}
		
		#endregion
	}
}
