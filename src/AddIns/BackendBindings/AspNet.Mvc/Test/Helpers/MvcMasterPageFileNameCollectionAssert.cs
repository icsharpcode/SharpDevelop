// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Helpers
{
	public static class MvcMasterPageFileNameCollectionAssert
	{
		public static void AreEqual(IEnumerable<MvcMasterPageFileName> expected, IEnumerable<MvcMasterPageFileName> actual)
		{
			List<string> expectedAsStrings = GetMvcMasterPageFileNamesAsStrings(expected);
			List<string> actualAsStrings = GetMvcMasterPageFileNamesAsStrings(actual);
			
			CollectionAssert.AreEqual(expectedAsStrings, actualAsStrings);
		}
		
		static List<string> GetMvcMasterPageFileNamesAsStrings(IEnumerable<MvcMasterPageFileName> fileNames)
		{
			var convertedFileNames = new List<string>();
			foreach (MvcMasterPageFileName fileName in fileNames) {
				string fileNameAsString = MvcMasterPageFileNameAssert.GetMvcMasterPageFileNameAsString(fileName);
				convertedFileNames.Add(fileNameAsString);
			}
			return convertedFileNames;
		}
	}
}
