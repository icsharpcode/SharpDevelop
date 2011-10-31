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
		
		[Test()]
		public void GetSetKeywordTest ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"class Test
{
	public int MyProperty {
		$g$
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("public"), "keyword 'public' not found.");
			Assert.IsNotNull (provider.Find ("get"), "keyword 'get' not found.");
			Assert.IsNotNull (provider.Find ("set"), "keyword 'set' not found.");
		}
		
		
		[Test()]
		public void GetSetKeywordTestCtrlSpace ()
		{
			var provider = CodeCompletionBugTests.CreateCtrlSpaceProvider (
@"class Test
{
	public int MyProperty {
		$$
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("public"), "keyword 'public' not found.");
			Assert.IsNotNull (provider.Find ("get"), "keyword 'get' not found.");
			Assert.IsNotNull (provider.Find ("set"), "keyword 'set' not found.");
		}
		

		[Test()]
		public void AddRemoveKeywordTest ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"
using System;
class Test
{
	public event EventHandler MyProperty {
		$g$
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.AreEqual (2, provider.Count);
			Assert.IsNotNull (provider.Find ("add"), "keyword 'add' not found.");
			Assert.IsNotNull (provider.Find ("remove"), "keyword 'remove' not found.");
		}
		
		[Test()]
		public void AddRemoveKeywordTestCtrlSpace ()
		{
			var provider = CodeCompletionBugTests.CreateCtrlSpaceProvider (
@"
using System;
class Test
{
	public event EventHandler MyProperty {
		$g$
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.AreEqual (2, provider.Count);
			Assert.IsNotNull (provider.Find ("add"), "keyword 'add' not found.");
			Assert.IsNotNull (provider.Find ("remove"), "keyword 'remove' not found.");
		}
		
		
		[Test()]
		public void IsAsKeywordTest ()
		{
			var provider = CodeCompletionBugTests.CreateProvider (
@"
using System;
class Test
{
	public void Method ()
	{
		void TestMe (object o)
		{
			if (o $i$
		}
	}
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("is"), "keyword 'is' not found.");
			Assert.IsNotNull (provider.Find ("as"), "keyword 'as' not found.");
		}
		
		[Test()]
		public void IsAsKeywordTestCtrlSpace ()
		{
			var provider = CodeCompletionBugTests.CreateCtrlSpaceProvider (
@"
using System;
class Test
{
	public void Method ()
	{
		void TestMe (object o)
		{
			if (o i$$
		}
	}
}
");
			Assert.IsNotNull (provider, "provider == null");
			Assert.IsNotNull (provider.Find ("is"), "keyword 'is' not found.");
			Assert.IsNotNull (provider.Find ("as"), "keyword 'as' not found.");
		}
	}
}

