// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeFileSystem : IFileSystem
	{
		Dictionary<string, string[]> directoryFiles = new Dictionary<string, string[]>();
		
		public void AddFakeFiles(string path, string searchPattern, string[] files)
		{
			string key = GetKey(path, searchPattern);
			directoryFiles.Add(key, files);
		}
		
		string GetKey(string path, string searchPattern)
		{
			return path + " - " + searchPattern;
		}
		
		public string[] GetFiles(string path, string searchPattern)
		{
			string key = GetKey(path, searchPattern);
			string[] files = null;
			if (directoryFiles.TryGetValue(key, out files)) {
				return files;
			}
			return new string[0];
		}
	}
}
