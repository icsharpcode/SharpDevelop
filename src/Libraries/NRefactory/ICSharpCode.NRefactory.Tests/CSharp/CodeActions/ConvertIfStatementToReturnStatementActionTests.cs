// 
// ConvertIfStatementToReturnStatementAction.cs
//  
// Author:
//       Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ConvertIfStatementToReturnStatementActionTests : ContextActionTestBase
	{
		[Test]
		public void TestReturn ()
		{
			Test<ConvertIfStatementToReturnStatementAction> (@"
class TestClass
{
	int TestMethod (int i)
	{
		$if (i > 0)
			return 1;
		else
			return 0;
	}
}", @"
class TestClass
{
	int TestMethod (int i)
	{
		return i > 0 ? 1 : 0;
	}
}");
		}

		[Test]
		public void TestIfElseWithBlocks ()
		{
			Test<ConvertIfStatementToReturnStatementAction>(@"class Foo
{
	bool Bar (string str)
	{
		$if (str.Length > 10) {
			return true;
		} else {
			return false;
		}
	}
}", @"class Foo
{
	bool Bar (string str)
	{
		return str.Length > 10;
	}
}");
		}

		[Test]
		public void TestImplicitElse ()
		{

			Test<ConvertIfStatementToReturnStatementAction> (@"
class TestClass
{
	int TestMethod (int i)
	{
		$if (i > 0)
			return 1;
		return 0;
	}
}", @"
class TestClass
{
	int TestMethod (int i)
	{
		return i > 0 ? 1 : 0;
	}
}");
		}

	}
}
