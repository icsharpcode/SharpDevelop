// 
// KeywordTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
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

namespace ICSharpCode.NRefactory.CSharp.CodeCompletion
{
	[TestFixture()]
	public class KeywordTests : TestBase
	{
		[Test()]
		public void CaseKeywordTest ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"
class Class
{
	void Test (string t)
	{
		switch (t) {
			$c$
		}
	}
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("case"), "keyword 'case' not found.");
		}
		
		[Test()]
		public void CaseKeywordTestCase2 ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"
class Class
{
	void Test (string t)
	{
		$c$
	}
}
");
	}
		
		[Test()]
		public void CaseKeywordTestCase3 ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"
class Class
{
void Test (string t)
{
	switch (t) {
		case ""test"":
		$c$
	}
}
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("case"), "keyword 'case' not found.");
		}
		
		[Test()]
		public void ModifierKeywordTest ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"
$p$
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("public"), "keyword 'public' not found.");
			Assert.IsNotNull (provider.Find ("namespace"), "keyword 'namespace' not found.");
		}
		
		[Test()]
		public void ModifierKeywordTestCtrlSpace ()
		{
			var provider = CodeCompletionBugTests.CreateCtrlSpaceProvider (
@"
$p$
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("public"), "keyword 'public' not found.");
			Assert.IsNotNull (provider.Find ("namespace"), "keyword 'namespace' not found.");
		}
		
		[Test()]
		public void ModifierKeywordTestCase2 ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"
class Test 
{
	$p$
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("public"), "keyword 'public' not found.");
			Assert.IsNull (provider.Find ("namespace"), "keyword 'namespace' found.");
		}
		
		[Test()]
		public void ModifierKeywordTestCase2CrtlSpace ()
		{
			var provider = CodeCompletionBugTests.CreateCtrlSpaceProvider (
@"
class Test 
{
	$p$
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("public"), "keyword 'public' not found.");
			Assert.IsNull (provider.Find ("namespace"), "keyword 'namespace' found.");
		}
	}
}

