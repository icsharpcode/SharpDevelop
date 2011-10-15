// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingPathResolver : ITextTemplatingPathResolver
	{
		public Dictionary<string, string> Paths = new Dictionary<string, string>();
		
		public string ResolvePath(string path)
		{
			string resolvedPath = null;
			if (Paths.TryGetValue(path, out resolvedPath)) {
				return resolvedPath;
			}
			return path;
		}
		
		public void AddPath(string path, string resolvedPath)
		{
			Paths.Add(path, resolvedPath);
		}
	}
}
