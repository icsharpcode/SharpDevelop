// 
// RedundantPartialTypeTests.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
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

using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class PartialTypeWithSinglePartIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestRedundantModifier()
		{
			var input = @"
partial class TestClass
{
}";
			var output = @"
class TestClass
{
}";
			Test<PartialTypeWithSinglePartIssue>(input, 1, output);
		}

		[Test]
		public void TestNecessaryModifier()
		{
			var input = @"
partial class TestClass
{
}
partial class TestClass
{
}";
			Test<PartialTypeWithSinglePartIssue>(input, 0);
		}

		[Test]
		public void TestDisable()
		{
			var input = @"
// ReSharper disable once PartialTypeWithSinglePart
partial class TestClass
{
}";
			TestWrongContext<PartialTypeWithSinglePartIssue>(input);
		}

		[Test]
		public void TestRedundantNestedPartial()
		{
			var input = @"
partial class TestClass
{
	partial class Nested
	{
	}
}
partial class TestClass
{
}";
			var output = @"
partial class TestClass
{
	class Nested
	{
	}
}
partial class TestClass
{
}";
			Test<PartialTypeWithSinglePartIssue>(input, output);
		}

		[Test]
		public void TestRedundantNestedPartialInNonPartialOuterClass()
		{
			var input = @"
class TestClass
{
	partial class Nested
	{
	}
}";
			var output = @"
class TestClass
{
	class Nested
	{
	}
}";
			Test<PartialTypeWithSinglePartIssue>(input, output);
		}

		[Test]
		public void TestRedundantNestedPartialDisable()
		{
			var input = @"
// ReSharper disable PartialTypeWithSinglePart
partial class TestClass
// ReSharper restore PartialTypeWithSinglePart
{
	partial class Nested
	{
	}
}
";
			var output = @"
// ReSharper disable PartialTypeWithSinglePart
partial class TestClass
// ReSharper restore PartialTypeWithSinglePart
{
	class Nested
	{
	}
}
";
			Test<PartialTypeWithSinglePartIssue>(input, output);
		}


		[Test]
		public void TestNeededNestedPartial()
		{
			var input = @"
partial class TestClass
{
	partial class Nested
	{
	}
}
partial class TestClass
{
	partial class Nested
	{
	}
}";
			Test<PartialTypeWithSinglePartIssue>(input, 0);
		}


	}
}