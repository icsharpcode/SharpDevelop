// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Helpers
{
	public static class MvcProjectFileAssert
	{
		public static void AreEqual(MvcProjectFile expected, MvcProjectFile actual)
		{
			string expectedAsString = GetMvcProjectFileAsString(expected);
			string actualAsString = GetMvcProjectFileAsString(actual);
			Assert.AreEqual(expectedAsString, actualAsString);
		}
		
		public static string GetMvcProjectFileAsString(MvcProjectFile fileName)
		{
			return String.Format(
				"FileName: {0}\r\nFolder: {1}\r\n, FullPath: {2}",
				fileName.FileName,
				fileName.FolderRelativeToProject,
				fileName.FullPath);
		}
	}
}
