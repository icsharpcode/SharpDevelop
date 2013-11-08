// 
// LowercaseLongLiteralSuffixTests.cs
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

using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class LongLiteralEndingLowerLIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestNormal()
		{
			Test<LongLiteralEndingLowerLIssue>(@"
class Test
{
	public long x = 3l;
}", @"
class Test
{
	public long x = 3L;
}");
		}

		[Test]
		public void TestDisabledForUnsignedFirst()
		{
			Test<LongLiteralEndingLowerLIssue>(@"
class Test
{
	public ulong x = 3ul;
}", 0);
		}

		[Test]
		public void TestUnsigned()
		{
			Test<LongLiteralEndingLowerLIssue>(@"
class Test
{
	public ulong x = 3lu;
}", @"
class Test
{
	public ulong x = 3LU;
}");
		}

		[Test]
		public void TestDisabledForUppercase()
		{
			Test<LongLiteralEndingLowerLIssue>(@"
class Test
{
	public long x = 3L;
}", 0);
		}

		[Test]
		public void TestDisabledForString()
		{
			Test<LongLiteralEndingLowerLIssue>(@"
class Test
{
	public string x = ""l"";
}", 0);
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<LongLiteralEndingLowerLIssue>(@"
class Test
{
	// ReSharper disable once LongLiteralEndingLowerL
	public long x = 3l;
}");
		}
	}
}

