/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 11.09.2009
 * Zeit: 19:40
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseClasses
{
	[TestFixture]
	public class AbstractColumnFixture
	{
		[Test]
		public void PlainConstructor()
		{
			AbstractColumn ab = new AbstractColumn();
			Assert.AreEqual(typeof(string),ab.DataType);
			Assert.AreEqual(String.Empty,ab.ColumnName);
		}
		
		
		[Test]
		public void ConstructorColumnName()
		{
			string cn = "AbstractColumn";
			AbstractColumn ab = new AbstractColumn(cn);
			Assert.AreEqual(typeof(string),ab.DataType);
			Assert.AreEqual(cn,ab.ColumnName);
		}
		
		
		[Test]
		public void ConstructorColumnNameAndType()
		{
			string cn = "AbstractColumn";
			AbstractColumn ab = new AbstractColumn(cn,typeof(int));
			Assert.AreEqual(typeof(int),ab.DataType);
			Assert.AreEqual(cn,ab.ColumnName);
		}
		
		[Test]
		public void SetColumnName()
		{
			string cn = "AbstractColumn";
			AbstractColumn ab = new AbstractColumn();
			ab.ColumnName = cn;
			Assert.AreEqual(cn,ab.ColumnName);
		}
		
		
		[Test]
		public void SetDataType()
		{
			AbstractColumn ab = new AbstractColumn();
			ab.DataType = typeof(int);
			Assert.AreEqual(typeof(int),ab.DataType);
		}
		
		
		[Test]
		public void GetDataTypeName()
		{
			AbstractColumn ab = new AbstractColumn();
			ab.DataType = typeof(int);
			Assert.AreEqual("System.Int32",ab.DataTypeName);
		}
		
		[Test]
		public void SetDataTypeName()
		{
			string dtn = "System.Int32";
			AbstractColumn ab = new AbstractColumn();
			ab.DataTypeName = dtn;
			Assert.AreEqual(typeof(int),ab.DataType);
		}
		
		
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
