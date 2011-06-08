// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using DTE = ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public static class ProjectItemCollectionAssert
	{
		public static void AreEqual(string[] expectedItems, List<DTE.ProjectItem> itemsList)
		{
			var actualItems = new List<string>();
			itemsList.ForEach(r => actualItems.Add(r.Name));
			
			CollectionAssert.AreEqual(expectedItems, actualItems);
		}
		
		public static void AreEqual(string[] expectedItems, IEnumerable itemsList)
		{
			var actualItems = new List<string>();
			foreach (DTE.ProjectItem item in itemsList) {
				actualItems.Add(item.Name);
			}
			
			CollectionAssert.AreEqual(expectedItems, actualItems);
		}
	}
}
