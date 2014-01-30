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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.DirectoryImport
{
	/// <summary>
	/// Tests the ExcludedNames class's IsExcluded method.
	/// </summary>
	[TestFixture]
	public class ExcludedNamesTests
	{
		ExcludedNames excludedNames;
		
		[SetUp]
		public void Init()
		{
			excludedNames = new ExcludedNames();
		}
		
		[Test]
		public void AllTextFilesExcluded()
		{
			excludedNames.Add("*.txt");
			
			Assert.IsTrue(excludedNames.IsExcluded("readme.txt"));
		}
			
		[Test]
		public void CaseInsensitive()
		{
			excludedNames.Add("readme.txt");
			Assert.IsTrue(excludedNames.IsExcluded("README.TXT"));
		}
		
		[Test]
		public void SingleCharacterWildcard()
		{
			excludedNames.Add("test?.txt");
			
			Assert.IsTrue(excludedNames.IsExcluded("test1.txt"));
			Assert.IsFalse(excludedNames.IsExcluded("test.txt"));
		}
		
		[Test]
		public void RegexInExcludedName()
		{
			excludedNames.Add("?tes(ab)+.txt");
			
			Assert.IsTrue(excludedNames.IsExcluded("Ates(ab)+.txt"));
			Assert.IsFalse(excludedNames.IsExcluded("Atesababab.txt"));
		}
	}
}
