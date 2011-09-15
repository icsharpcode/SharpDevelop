// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Helpers
{
	public static class MvcProjectFileCollectionAssert
	{
		public static void AreEqual(IEnumerable<MvcProjectFile> expected, IEnumerable<MvcProjectFile> actual)
		{
			List<string> expectedAsStrings = GetMvcProjectFilesAsStrings(expected);
			List<string> actualAsStrings = GetMvcProjectFilesAsStrings(actual);
			
			CollectionAssert.AreEqual(expectedAsStrings, actualAsStrings);
		}
		
		static List<string> GetMvcProjectFilesAsStrings(IEnumerable<MvcProjectFile> fileNames)
		{
			var convertedFileNames = new List<string>();
			foreach (MvcProjectFile fileName in fileNames) {
				string fileNameAsString = MvcProjectFileAssert.GetMvcProjectFileAsString(fileName);
				convertedFileNames.Add(fileNameAsString);
			}
			return convertedFileNames;
		}
	}
}
