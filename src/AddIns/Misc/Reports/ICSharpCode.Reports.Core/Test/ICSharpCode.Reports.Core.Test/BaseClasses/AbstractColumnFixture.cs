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
