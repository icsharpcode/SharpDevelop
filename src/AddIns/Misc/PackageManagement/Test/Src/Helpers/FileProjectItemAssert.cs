// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public static class FileProjectItemAssert
	{
		public static void AreEqual(FileProjectItem expectedFileItem, FileProjectItem actualFileItem)
		{
			string[] expectedFileItemStrings = GetFileProjectItemAsString(expectedFileItem);
			string[] actualFileItemStrings = GetFileProjectItemAsString(actualFileItem);
			
			Assert.AreEqual(expectedFileItemStrings, actualFileItemStrings);
		}
		
		static string[] GetFileProjectItemAsString(FileProjectItem fileItem)
		{
			if (fileItem == null) {
				return new string[3];
			}
			
			return new string[] {
				"Include: " + fileItem.Include,
				"FileName: " + fileItem.FileName,
				"Type: " + fileItem.ItemType
			};
		}
	}
}
