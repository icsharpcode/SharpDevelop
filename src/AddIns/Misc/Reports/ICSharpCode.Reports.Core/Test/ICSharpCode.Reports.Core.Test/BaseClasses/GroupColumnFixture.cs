/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 15.11.2008
 * Zeit: 19:38
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Basics
{
	[TestFixture]
	public class GroupColumnFixture
	{
		string columnName = "TestColumn";
		
		[Test]
		public void Constructor()
		{
			GroupColumn gc = new GroupColumn ("TestColumn",1, ListSortDirection.Ascending);
			Assert.AreEqual(columnName,gc.ColumnName);
			Assert.AreEqual(1,gc.GroupLevel);
			Assert.AreEqual(ListSortDirection.Ascending,gc.SortDirection);
			Type t = typeof(System.String);
			Assert.AreEqual(t,gc.DataType);
		}
		
		[Test]
		[ExpectedExceptionAttribute(typeof(GroupLevelException))]
		public void ConstructorWithInvalidGroupLevel()
		{
			GroupColumn gc = new GroupColumn ("TestColumn",-1, ListSortDirection.Ascending);
		}
		
		
	}
}
