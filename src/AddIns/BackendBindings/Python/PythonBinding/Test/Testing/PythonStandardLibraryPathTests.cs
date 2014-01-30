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
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonStandardLibraryPathTests
	{
		[Test]
		public void PathsPropertyReturnsPython26LibDirectory()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(@"c:\python26\lib");
			string[] expectedPaths = new string[] { @"c:\python26\lib" };
			Assert.AreEqual(expectedPaths, path.Directories);
		}
		
		[Test]
		public void HasPathReturnsTrueForNonEmptyPathString()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(@"c:\python26\lib");
			Assert.IsTrue(path.HasPath);
		}
		
		[Test]
		public void HasPathReturnsFalseForEmptyPathString()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(String.Empty);
			Assert.IsFalse(path.HasPath);
		}
		
		[Test]
		public void DirectoryPropertyReturnsPython26LibDirectoryAndPython26LibTkDirectory()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(@"c:\python26\lib;c:\python26\lib\lib-tk");
			string[] expectedPaths = new string[] { @"c:\python26\lib", @"c:\python26\lib\lib-tk" };
			Assert.AreEqual(expectedPaths, path.Directories);
		}
		
		[Test]
		public void DirectoryPropertyReturnsPython26LibDirectoryAndPython26LibTkDirectorySetInPathProperty()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(String.Empty);
			path.Path = @"c:\python26\lib;c:\python26\lib\lib-tk";
			string[] expectedPaths = new string[] { @"c:\python26\lib", @"c:\python26\lib\lib-tk" };
			Assert.AreEqual(expectedPaths, path.Directories);
		}
		
		[Test]
		public void DirectoriesAreClearedWhenPathIsSetToDifferentValue()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(@"c:\temp");
			path.Path = @"c:\python26\lib;c:\python26\lib\lib-tk";
			string[] expectedPaths = new string[] { @"c:\python26\lib", @"c:\python26\lib\lib-tk" };
			Assert.AreEqual(expectedPaths, path.Directories);
		}
		
		[Test]
		public void EmptyDirectoryInPathNotAddedToDirectories()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(@"c:\temp;;c:\python\lib");
			string[] expectedPaths = new string[] { @"c:\temp", @"c:\python\lib" };
			Assert.AreEqual(expectedPaths, path.Directories);
		}
		
		[Test]
		public void DirectoryWithJustWhitespaceIsNotAddedToPath()
		{
			PythonStandardLibraryPath path = new PythonStandardLibraryPath(@"c:\temp;    ;c:\python\lib");
			string[] expectedPaths = new string[] { @"c:\temp", @"c:\python\lib" };
			Assert.AreEqual(expectedPaths, path.Directories);
		}
	}
}
