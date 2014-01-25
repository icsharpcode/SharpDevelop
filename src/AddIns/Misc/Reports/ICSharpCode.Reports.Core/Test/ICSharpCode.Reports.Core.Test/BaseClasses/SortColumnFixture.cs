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
using System.ComponentModel;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Basics
{
	[TestFixture]
	public class SortColumnFixture
	{
		string columnName = "TestColumn";
		
		[Test]
		public void DefaultConstructor()
		{
			SortColumn sc = new SortColumn("TestColumn",typeof(System.String));
			Assert.AreEqual(sc.ColumnName,columnName,"ColumnName shoul be equal 'columnName'");
			Assert.AreEqual (typeof(System.String),sc.DataType);
			Assert.AreEqual(System.ComponentModel.ListSortDirection.Ascending,sc.SortDirection);
		}
		
		[Test]
		public void Constr_2 ()
		{
			SortColumn sc = new SortColumn ("TestColumn",
			                                ListSortDirection.Descending);
			Assert.AreEqual(columnName,sc.ColumnName,"ColumnName should be equal 'columnName'");
			Assert.AreEqual(ListSortDirection.Descending,
			                sc.SortDirection,"SortDirection shoul be Descending");
			Type t = typeof(System.String);
			Assert.AreEqual(t,sc.DataType,"Type should be 'String'");
		}
		
		[Test]
		public void Constr_3 ()
		{
			SortColumn sc = new SortColumn ("TestColumn",
			                                ListSortDirection.Ascending,typeof(System.String),true);
			Assert.AreEqual(columnName,
			                sc.ColumnName,
			                "ColumnName should be equal 'columnName'");
			Assert.AreEqual(ListSortDirection.Ascending,
			                sc.SortDirection,"SortDirection shoul be Aescending");
			Type t = typeof(System.String);
			Assert.AreEqual(t,sc.DataType,"Type should be 'String'");
			Assert.IsTrue(sc.CaseSensitive);
		}
	}
}
