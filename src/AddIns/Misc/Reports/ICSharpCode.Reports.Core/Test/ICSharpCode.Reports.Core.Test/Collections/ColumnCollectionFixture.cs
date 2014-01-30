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
