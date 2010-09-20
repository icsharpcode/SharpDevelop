// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
