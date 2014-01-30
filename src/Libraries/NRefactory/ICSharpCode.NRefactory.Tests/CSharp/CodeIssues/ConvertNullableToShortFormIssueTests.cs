//
// ConvertNullableToShortFormIssueTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ConvertNullableToShortFormIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleCase ()
		{
			Test<ConvertNullableToShortFormIssue>(@"using System;

class Foo
{
	Nullable<int> Bar ()
	{
		return 5;
	}
}", @"using System;

class Foo
{
	int? Bar ()
	{
		return 5;
	}
}");
		}

		[Test]
		public void TestFullyQualifiedNameCase ()
		{
			Test<ConvertNullableToShortFormIssue>(@"class Foo
{
	void Bar ()
	{
		System.Nullable<int> a;
	}
}", @"class Foo
{
	void Bar ()
	{
		int? a;
	}
}");
		}


		[Test]
		public void TestAlreadyShort ()
		{
			TestWrongContext<ConvertNullableToShortFormIssue>(@"class Foo
{
	int? Bar (int o)
	{
		return 5;
	}
}");
		}

		[Test]
		public void TestInvalid ()
		{
			TestWrongContext<ConvertNullableToShortFormIssue>(@"using System;
namespace NN {
	class Nullable<T> {}
	class Foo
	{
		void Bar ()
		{
			Nullable<int> a;
		}
	}
}");
		}

		[Test]
		public void TestInvalidTypeOf ()
		{
			TestWrongContext<ConvertNullableToShortFormIssue>(@"using System;
class Foo
{
	bool Bar (object o)
	{
		return o.GetType() == typeof (Nullable<>);
	}
	bool Bar2 (object o)
	{
		return o.GetType() == typeof (System.Nullable<>);
	}
}
");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<ConvertNullableToShortFormIssue>(@"class Foo
{
	void Bar ()
	{
		// ReSharper disable once ConvertNullableToShortForm
		System.Nullable<int> a;
	}
}");
		}
	}
}

