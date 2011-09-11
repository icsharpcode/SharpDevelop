// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Helpers
{
	public static class MvcMasterPageFileNameAssert
	{
		public static void AreEqual(MvcMasterPageFileName expected, MvcMasterPageFileName actual)
		{
			string expectedAsString = GetMvcMasterPageFileNameAsString(expected);
			string actualAsString = GetMvcMasterPageFileNameAsString(actual);
			Assert.AreEqual(expectedAsString, actualAsString);
		}
		
		public static string GetMvcMasterPageFileNameAsString(MvcMasterPageFileName fileName)
		{
			return String.Format(
				"FileName: {0}\r\nFolder: {1}\r\n, FullPath: {2}",
				fileName.FileName,
				fileName.FolderRelativeToProject,
				fileName.FullPath);
		}
	}
}
