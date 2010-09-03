// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests the PropertyPad's SortObjectsBySiteName method.
	/// </summary>
	[TestFixture]
	public class PropertyPadSortingTests
	{
		[Test]
		public void SimpleTypeNameSort()
		{
			string a = String.Empty;
			int b = 0;
			
			object[] unsortedObjects = new object[] {a, b};
			object[] expectedSortedObjects = new object[] {b, a};
			
			Assert.AreEqual(expectedSortedObjects, PropertyPad.SortObjectsBySiteName(unsortedObjects));
		}
		
		[Test]
		public void FirstObjectIsNull()
		{
			string a = String.Empty;
			object b = null;

			object[] unsortedObjects = new object[] {a, b};
			object[] expectedSortedObjects = new object[] {b, a};
			
			Assert.AreEqual(expectedSortedObjects, PropertyPad.SortObjectsBySiteName(unsortedObjects));
		}
		
		[Test]
		public void SortedBySite()
		{
			
			MockSite site1 = new MockSite();
			site1.Name = "b";
			MockSite site2 = new MockSite();
			site2.Name = "a";
			MockSite site3 = new MockSite();
			site3.Name = "z";
			
			MockComponent c1 = new MockComponent();
			c1.Site = site1;
			MockComponent c2 = new MockComponent();
			c2.Site = site2;
			MockComponent c3 = new MockComponent();
			c3.Site = site3;

			object[] unsortedObjects = new object[] {c1, c2, c3};
			object[] expectedSortedObjects = new object[] {c2, c1, c3};
			
			Assert.AreEqual(expectedSortedObjects, PropertyPad.SortObjectsBySiteName(unsortedObjects));
		}
		
		[Test]
		public void OneSiteIsNull()
		{
			MockSite site1 = new MockSite();
			site1.Name = "z";
			
			MockComponent c1 = new MockComponent();
			c1.Site = site1;
			MockComponent c2 = new MockComponent();
			
			object[] unsortedObjects = new object[] {c1, c2};
			object[] expectedSortedObjects = new object[] {c2, c1};
			
			Assert.AreEqual(expectedSortedObjects, PropertyPad.SortObjectsBySiteName(unsortedObjects));			
		}

		[Test]
		public void OneSiteNameIsNull()
		{
			MockSite site1 = new MockSite();
			site1.Name = "z";
			MockSite site2 = new MockSite();
			site2.Name = null;
			
			MockComponent c1 = new MockComponent();
			c1.Site = site1;
			MockComponent c2 = new MockComponent();
			c2.Site = site2;
			
			object[] unsortedObjects = new object[] {c1, c2};
			object[] expectedSortedObjects = new object[] {c2, c1};
			
			Assert.AreEqual(expectedSortedObjects, PropertyPad.SortObjectsBySiteName(unsortedObjects));			
		}
		
	}
}
