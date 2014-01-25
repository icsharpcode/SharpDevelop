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
using System.Collections.Generic;
using ICSharpCode.Core;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockAddInTreeTestFixture
	{
		MockAddInTree addinTree;
		List<string> myItems;
		
		[SetUp]
		public void Init()
		{
			myItems = new List<string>();
			myItems.Add("a");
			myItems.Add("b");
			
			addinTree = new MockAddInTree();
			addinTree.AddItems("MyItems", myItems);
		}
		
		[Test]
		public void BuildItemsReturnsMyItemsListWhenMyItemsPathNameUsedAsParameter()
		{
			Assert.AreEqual(myItems, addinTree.BuildItems<string>("MyItems", null));
		}
		
		[Test]
		public void BuildItemsThrowsExceptionWhenUnknownPathNamedUsedAsParameter()
		{
			Assert.Throws<TreePathNotFoundException>(delegate { addinTree.BuildItems<string>("Unknown", null); });
		}
	}
}
