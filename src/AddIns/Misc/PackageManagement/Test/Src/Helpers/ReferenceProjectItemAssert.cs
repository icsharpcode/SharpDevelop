// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public static class ReferenceProjectItemAssert
	{
		public static void AreEqual(ReferenceProjectItem expectedReferenceItem, ReferenceProjectItem actualReferenceItem)
		{
			string[] expectedValues = GetReferenceProjectItemAsStrings(expectedReferenceItem);
			string[] actualValues = GetReferenceProjectItemAsStrings(actualReferenceItem);
			
			Assert.AreEqual(expectedValues, actualValues);
		}
		
		static string[] GetReferenceProjectItemAsStrings(ReferenceProjectItem referenceItem)
		{
			if (referenceItem == null) {
				return new string[2];
			}
			return new string[] {
				"Include: " + referenceItem.Include,
				"HintPath: " + referenceItem.HintPath
			};
		}
	}
}
