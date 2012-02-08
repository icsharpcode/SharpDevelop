// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ItemTypeTests
	{
		[Test]
		public void IsFolder_ItemTypeIsFolder_ReturnsTrue()
		{
			var itemType = ItemType.Folder;
			bool result = itemType.IsFolder();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsFolder_ItemTypeIsResource_ReturnsFalse()
		{
			var itemType = ItemType.Resource;
			bool result = itemType.IsFolder();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsFolder_ItemTypeIsWebReferences_ReturnsTrue()
		{
			var itemType = ItemType.WebReferences;
			bool result = itemType.IsFolder();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsFolder_ItemTypeIsServiceReferences_ReturnsTrue()
		{
			var itemType = ItemType.ServiceReferences;
			bool result = itemType.IsFolder();
			
			Assert.IsTrue(result);
		}
	}
}
