/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 16.11.2008
 * Zeit: 18:33
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Basics
{
	
	[TestFixture]
	public class ReportItemCollectionFixture
	{
		
		private ReportItemCollection itemCollection;
		private bool gotEvent;
		
		
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
			itemCollection.Added += OnItemAdded;
			int len = itemCollection.Count();
			BaseReportItem r = new BaseReportItem();
			r.Name = "Inserted";
			itemCollection.Insert(1,r);
			Assert.AreEqual (len + 1,itemCollection.Count());
			Assert.IsTrue(gotEvent,"No inserted' Event");
			this.gotEvent = false;
			// read inserted element and check some default values
			BaseReportItem r1 = itemCollection.Find("Inserted");
			Assert.AreEqual(GlobalValues.DefaultBackColor,r1.BackColor);
			Assert.AreEqual(System.Drawing.Color.Black,r1.ForeColor);
		}
		
		
		private void OnItemAdded (object sender,CollectionChangedEventArgs<BaseReportItem> e)
		{
			this.gotEvent = true;
			Assert.AreEqual ("Inserted",e.Item.Name,"Item should be the same as we insert");
		}
		
		#endregion
		
		#region delete/remove
		
		[Test]
		public void RemoveFirstElementAndCheckForEvent ()
		{
			itemCollection = this.PlainCollection();
			int i = itemCollection.Count;
			itemCollection.Removed += OnRemoveItem;
			itemCollection.RemoveAt(1);
			Assert.AreEqual(i-1,itemCollection.Count) ;
			Assert.IsTrue(gotEvent,"No 'removed' Event");
			gotEvent = false;
		}
		
		
		[Test]
		public void RemoveElementAndCheckForEvent ()
		{
			itemCollection = this.PlainCollection();
			int i = itemCollection.Count;
			itemCollection.Removed += OnRemoveItem;
			BaseReportItem r = itemCollection[1];
			Assert.AreEqual("t2",r.Name);
			itemCollection.Remove (r);
			Assert.AreEqual(i-1,itemCollection.Count);
			Assert.IsTrue(gotEvent,"No 'removed' Event");
			gotEvent = false;
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
		
		private void OnRemoveItem (object sender,CollectionChangedEventArgs<BaseReportItem> e)
		{
			gotEvent = true;
			Assert.AreEqual("t2",e.Item.Name);
		}
		
		
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
