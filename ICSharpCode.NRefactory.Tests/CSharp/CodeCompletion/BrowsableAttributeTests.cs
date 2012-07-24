//
// BrowsableAttributeTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using NUnit.Framework;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.CSharp.CodeCompletion
{
	[TestFixture]
	public class BrowsableAttributeTests : TestBase
	{
		[Test()]
		public void TestBrowsableClasses ()
		{
			
			CompletionDataList provider = CodeCompletionBugTests.CreateProvider (
				@"
using System;
using System.ComponentModel;

[Browsable(true)]
class BrowsableTest {}

[Browsable(false)]
class NotBrowsableTest {}

class Test
{
	void Test ()
	{
		$B$
	}
}");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("BrowsableTest"), "'BrowsableTest' not found.");
			Assert.IsNull (provider.Find ("NotBrowsableTest"), "'NotBrowsableTest' found.");
		}

		[Test()]
		public void TestBrowsableMembers ()
		{
			CompletionDataList provider = CodeCompletionBugTests.CreateProvider (
				@"
using System;
using System.ComponentModel;

class Test
{
	[Browsable(true)]
	int BrowsableTest { get; set; }

	[Browsable(false)]
	int NotBrowsableTest { get; set; }

	void Test ()
	{
		$B$
	}
}");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("BrowsableTest"), "'BrowsableTest' not found.");
			Assert.IsNull (provider.Find ("NotBrowsableTest"), "'NotBrowsableTest' found.");
		}

		[Test()]
		public void TestEditorBrowsableAttributeClasses ()
		{
			
			CompletionDataList provider = CodeCompletionBugTests.CreateProvider (
				@"
using System;
using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Always)]
class BrowsableTest {}

[EditorBrowsable(EditorBrowsableState.Never)]
class NotBrowsableTest {}

class Test
{
	void Test ()
	{
		$B$
	}
}");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("BrowsableTest"), "'BrowsableTest' not found.");
			Assert.IsNull (provider.Find ("NotBrowsableTest"), "'NotBrowsableTest' found.");
		}

		
		[Test()]
		public void TestEditorBrowsableAttributeMembers ()
		{
			
			CompletionDataList provider = CodeCompletionBugTests.CreateProvider (
				@"
using System;
using System.ComponentModel;

class Test
{
	[EditorBrowsable(EditorBrowsableState.Always)]
	int BrowsableTest { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	int NotBrowsableTest { get; set; }

	void Test ()
	{
		$B$
	}
}");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("BrowsableTest"), "'BrowsableTest' not found.");
			Assert.IsNull (provider.Find ("NotBrowsableTest"), "'NotBrowsableTest' found.");
		}
	}
}

