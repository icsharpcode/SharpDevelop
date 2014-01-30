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
