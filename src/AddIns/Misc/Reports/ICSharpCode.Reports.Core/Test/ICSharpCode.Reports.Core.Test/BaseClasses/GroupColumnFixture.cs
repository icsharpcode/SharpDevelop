// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
