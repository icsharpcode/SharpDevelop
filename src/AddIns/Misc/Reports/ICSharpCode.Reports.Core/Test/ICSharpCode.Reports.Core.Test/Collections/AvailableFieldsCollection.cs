/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 20.02.2009
 * Zeit: 20:09
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Basics
{
	[TestFixture]
	public class AvailableFieldsCollectionFixture
	{
		private AvailableFieldsCollection itemCollection;
		
		[Test]
		public void Constructor ()
		{
			AvailableFieldsCollection it = new AvailableFieldsCollection();
			Assert.IsNotNull(it,"Should not be 'null'");
			Assert.AreEqual(0,it.Count,"Count should be '0'");
		}
		
		
			[Test]
		public void CurrentCulture ()
		{
			ColumnCollection cc = new ColumnCollection();
			Assert.AreEqual(System.Globalization.CultureInfo.CurrentCulture,ColumnCollection.Culture,
			                "Culture should be 'CurrentCulture'");
		}
		
		#region Find
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsNull ()
		{
			itemCollection = this.PlainCollection();
			AbstractColumn r = itemCollection.Find(null);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsEmpty ()
		{
			itemCollection = this.PlainCollection();
			AbstractColumn r = itemCollection.Find(String.Empty);
		}
		
		
		[Test]
		public void PlainCollectionFind ()
		{
			this.itemCollection = this.PlainCollection();
			AbstractColumn r = itemCollection.Find("t2");
			Assert.AreEqual("t2",r.ColumnName);
		}
		
		[Test]
		public void FindKeyNotExist ()
		{
			this.itemCollection = this.PlainCollection();
			AbstractColumn r = itemCollection.Find("t5");
			Assert.IsNull(r);
		}
		#endregion
		
		private AvailableFieldsCollection PlainCollection()
		{
			AvailableFieldsCollection ri = new AvailableFieldsCollection();
			AbstractColumn t1 = new AbstractColumn();
			t1.ColumnName = "t1";
			ri.Add(t1);
			 t1 = new AbstractColumn();
			t1.ColumnName = "t2";
			ri.Add(t1);
			t1 = new AbstractColumn();
			t1.ColumnName = "t3";
			ri.Add(t1);
			return ri;
		}
	}
}
