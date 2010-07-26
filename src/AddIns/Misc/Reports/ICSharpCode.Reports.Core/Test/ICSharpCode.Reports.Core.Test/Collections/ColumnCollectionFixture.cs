/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 21.11.2008
 * Zeit: 19:30
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Basics
{
	[TestFixture]
	public class ColumnCollectionFixture
	{
		
		[Test]
		public void Constructor()
		{
			ColumnCollection cc = new ColumnCollection();
			Assert.IsNotNull (cc,"Collection should not be 'null'");
			Assert.AreEqual(0,cc.Count,"Lenght should be '0'");
		}
		
		[Test]
		public void CurrentCulture ()
		{
			ColumnCollection cc = new ColumnCollection();
			Assert.AreEqual(System.Globalization.CultureInfo.CurrentCulture,ColumnCollection.Culture,
			                "Culture should be 'CurrentCulture'");
		}
		
		[Test]
		public void Add2Elements()
		{
			ColumnCollection cc = CollectionWith2Columns();
			Assert.AreEqual(2,cc.Count,"Count should be '0'");
		}
		
		
		[Test]
		public void FindFirstElement ()
		{
			ColumnCollection cc = CollectionWith2Columns();
			AbstractColumn c = cc.Find("Column1");
			Assert.IsNotNull(c);
			Assert.That(c,Is.InstanceOf(typeof(SortColumn)));
		}
		
		
		[Test]
		public void FindSecondElement ()
		{
			ColumnCollection cc = CollectionWith2Columns();
			AbstractColumn c = cc.Find("Column2");
			Assert.IsNotNull (c,"Should not be 'null'");
			Assert.That(c,Is.InstanceOf(typeof(SortColumn)));
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsNull ()
		{
			ColumnCollection cc = new ColumnCollection();
			AbstractColumn c = cc.Find(null);
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsEmpty ()
		{
			ColumnCollection cc = new ColumnCollection();
			AbstractColumn c = cc.Find(String.Empty);
		}
		
		
		[Test]
		public void FindWithNameNotExist ()
		{
			ColumnCollection cc = new ColumnCollection();
			AbstractColumn c = cc.Find("noexist");
			Assert.IsNull(c,"Column should be 'null'");
		}
		
		
		private ColumnCollection CollectionWith2Columns()
		{
			ColumnCollection cc = new ColumnCollection();
			cc.Add (new SortColumn("Column1",typeof(System.String)));
			cc.Add(new SortColumn("Column2",System.ComponentModel.ListSortDirection.Ascending,typeof(System.String),true));
			return cc;
			        
		}
	}
}
