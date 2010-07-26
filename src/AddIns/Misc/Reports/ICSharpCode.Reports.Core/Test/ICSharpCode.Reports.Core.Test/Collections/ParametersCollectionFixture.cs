/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.02.2009
 * Zeit: 19:01
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Basics
{
	[TestFixture]
	public class ParametersCollectionFixture
	{
		[Test]
		public void Constructor()
		{
			ParameterCollection p = new ParameterCollection();
			Assert.IsNotNull (p,"Collection should not be 'null'");
			Assert.AreEqual(0,p.Count,"Lenght should be '0'");
		}
		
		
		[Test]
		public void CurrentCulture ()
		{
			ParameterCollection p = new ParameterCollection();
			Assert.AreEqual(System.Globalization.CultureInfo.CurrentCulture,ParameterCollection.Culture,
			                "Culture should be 'CurrentCulture'");
		}
		
		
		[Test]
		public void Add2Elements()
		{
			ParameterCollection p = this.CollectionWith2Parameters();
			Assert.AreEqual(2,p.Count,"Count should be '2'");
		}
		
		
		[Test]
		public void AddParamToExistingList()
		{
			ParameterCollection p = this.CollectionWith2Parameters();
			p.Add (new BasicParameter("p3","Parameter3"));
			Assert.AreEqual(3,p.Count,"Count should be '3'");
		}
		
		[Test]
		public void NameNotExist ()
		{
			ParameterCollection p = this.CollectionWith3Parameters();
			BasicParameter c = p.Find("noexist");
			Assert.IsNull(c,"Column should be 'null'");
		}
		
		
		[Test]
		public void FindByName()
		{
			ParameterCollection p = this.CollectionWith3Parameters();
			BasicParameter bp = p.Find("p2");
			Assert.IsNotNull(bp);
			Assert.AreEqual(bp.ParameterValue,"Parameter2","Find should return valid element");
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsNull ()
		{
			ParameterCollection p = this.CollectionWith3Parameters();
			BasicParameter bp = p.Find(null);
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FindWithNameIsEmpty ()
		{
			
			ParameterCollection p = this.CollectionWith3Parameters();
			BasicParameter bp = p.Find(String.Empty);
			Assert.IsNotNull(bp,"Item should not be 'null'");			
		}
		
		#region SqlParameters
		[Test]
		public void CreateCollectionWithOneSqlParameter ()
		{
			ParameterCollection p = this.CollectionWithSQLParameters();
			Assert.AreEqual(3,p.Count);
		}
		
		[Test]
		public void SecoundParameterShouldSqlParameter ()
		{
			ParameterCollection p = this.CollectionWithSQLParameters();
			BasicParameter bp = p[1];
			Assert.That(bp,Is.InstanceOf(typeof(SqlParameter)));
		}
		
		
		[Test]
		public void CheckValueSqlParameter ()
		{
			ParameterCollection p = this.CollectionWithSQLParameters();
			SqlParameter bp = (SqlParameter)p[1];
			Assert.That(bp,Is.InstanceOf(typeof(SqlParameter)));
			Assert.AreEqual("p2",bp.ParameterName);
			Assert.AreEqual("Parameter2",bp.ParameterValue);
			Assert.AreEqual(System.Data.DbType.String,bp.DataType);
		}
		
		
		[Test]
		public void ExtractAllSqlParameters ()
		{
			ParameterCollection p = this.CollectionWithSQLParameters();
			p.Add(new SqlParameter ("sql",System.Data.DbType.String,"SqlParameter2"));
			System.Collections.Generic.List<SqlParameter> pp = p.ExtractSqlParameters();
			Assert.IsNotNull(pp);
			Assert.AreEqual(2,pp.Count);
			SqlParameter p1 = pp[0];
			//p.Add(new SqlParameter ("p2",System.Data.DbType.String,"Parameter2"));
			Assert.AreEqual("p2",p1.ParameterName);
			Assert.AreEqual("Parameter2",p1.ParameterValue);
			Assert.AreEqual(System.Data.DbType.String,p1.DataType);
//			p.Add(new SqlParameter ("sql",System.Data.DbType.String,"SqlParameter2"));
			SqlParameter p2 = pp[1];
			Assert.AreEqual("sql",p2.ParameterName);
			Assert.AreEqual("SqlParameter2",p2.ParameterValue);
			Assert.AreEqual(System.Data.DbType.String,p2.DataType);
		}
		#endregion
		
		[Test]
		public void CreateHashtableFromParameterCollection ()
		{
			ParameterCollection p = this.CollectionWith3Parameters();
			System.Collections.Hashtable ht = p.CreateHash();
			string p1 = (string)ht["p2"];
			Assert.AreEqual("Parameter2",p1);
		}
		
		
		private ParameterCollection CollectionWith2Parameters()
		{
			ParameterCollection p = new ParameterCollection();
			p.Add (new BasicParameter("p1","Parameter1"));
			p.Add(new BasicParameter ("p2","Parameter2"));
			return p;        
		}
		
		
		private ParameterCollection CollectionWith3Parameters()
		{
			ParameterCollection p = new ParameterCollection();
			p.Add (new BasicParameter("p1","Parameter1"));
			p.Add(new BasicParameter ("p2","Parameter2"));
			p.Add(new BasicParameter ("p3","Parameter3"));
			return p;
		}
		
		private ParameterCollection CollectionWithSQLParameters()
		{
			ParameterCollection p = new ParameterCollection();
			p.Add (new BasicParameter("p1","Parameter1"));
			p.Add(new SqlParameter ("p2",System.Data.DbType.String,"Parameter2"));
			p.Add(new BasicParameter ("p3","Parameter3"));
			return p;
		}
	}
}
