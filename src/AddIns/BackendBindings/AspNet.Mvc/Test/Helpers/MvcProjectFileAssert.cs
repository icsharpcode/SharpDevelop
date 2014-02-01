// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
