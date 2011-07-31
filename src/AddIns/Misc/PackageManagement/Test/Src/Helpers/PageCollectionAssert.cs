// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public static class PageCollectionAssert
	{
		public static void AreEqual(IEnumerable<Page> expectedPages, IEnumerable<Page> actualPages)
		{
			List<string> convertedExpectedPages = ConvertToStrings(expectedPages);
			List<string> convertedActualPages = ConvertToStrings(actualPages);
			
			CollectionAssert.AreEqual(convertedExpectedPages, convertedActualPages);
		}
		
		static List<string> ConvertToStrings(IEnumerable<Page> pages)
		{
			List<string> pagesAsText = new List<string>();
			foreach (Page page in pages) {
				pagesAsText.Add(GetPageAsString(page));
			}
			return pagesAsText;
		}
		
		static string GetPageAsString(Page page)
		{
			return String.Format("Page: Number: {0}, IsSelected: {1}",
				page.Number,
				page.IsSelected);
		}
	}
}
