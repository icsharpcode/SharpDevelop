﻿// 
// AutoAsyncTests.cs
//  
// Author:
//       Luís Reis <luiscubal@gmail.com>
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

using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class AutoAsyncTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimple() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		tcs.SetResult (1);
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public async Task<int> TestMethod ()
	{
		return 1;
	}
}");
		}

		[Test]
		public void TestIncorrectlyInitializedCompletionSource() {
			TestWrongContext<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	TaskCompletionSource<int> GetCompletionSource ()
	{
		return new TaskCompletionSource<int> ();
	}
	public Task<int> $TestMethod ()
	{
		var tcs = GetCompletionSource ();
		tcs.SetResult (1);
		return tcs.Task;
	}
}");
		}

		[Test]
		public void TestTaskWithoutResult() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		tcs.SetResult (1);
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public async Task TestMethod ()
	{
		int result = 1;
		return;
	}
}");
		}

		[Test]
		public void TestAsyncContinueWith() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task Foo () { return null; }
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (async (precedent) => {
			await Foo();
			tcs.SetResult (1);
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task Foo () { return null; }
	public async Task<int> TestMethod ()
	{
		await Foo ();
		await Foo ();
		return 1;
	}
}");
		}

		[Test]
		public void TestTaskWithoutResultExtraName() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		int result = 1;
		tcs.SetResult (1);
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public async Task TestMethod ()
	{
		int result = 1;
		int result1 = 1;
		return;
	}
}");
		}

		[Test]
		public void TestBasicContinueWith() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (precedent => {
			tcs.SetResult (precedent.Result);
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		int precedentResult = await Foo ();
		return precedentResult;
	}
}");
		}

		[Test]
		public void TestTaskWithoutCompletionSource() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		return Foo ().ContinueWith (precedent => {
			return 3;
		});
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		int precedentResult = await Foo ();
		return 3;
	}
}");
		}

		[Test]
		public void TestContinueWithUsingPrecedent() {
			Test<AutoAsyncIssue>(@"
using System;
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (precedent => {
			Console.WriteLine (precedent.IsFaulted);
			tcs.SetResult (precedent.Result);
		});
		return tcs.Task;
	}
}", @"
using System;
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		Task<int> precedent1 = Foo ();
		int precedentResult = await precedent1;
		Console.WriteLine (precedent1.IsFaulted);
		return precedentResult;
	}
}");
		}

		[Test]
		public void TestContinueWithUsingPrecedentTaskWithNoParameters() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (precedent => {
			Console.WriteLine (precedent.IsFaulted);
			tcs.SetResult (0);
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		Task precedent1 = Foo ();
		await precedent1;
		Console.WriteLine (precedent1.IsFaulted);
		return 0;
	}
}");
		}

		[Test]
		public void TestBasicContinueWithExtraName() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (precedent => {
			int precedentResult = precedent.Result;
			tcs.SetResult (precedentResult);
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		int precedentResult1 = await Foo ();
		int precedentResult = precedentResult1;
		return precedentResult;
	}
}");
		}

		[Test]
		public void TestConditionalReturn() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod (int i)
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (precedent => {
			if (i == 0)
				tcs.SetResult (precedent.Result);
			else
				tcs.SetResult (i);
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod (int i)
	{
		int precedentResult = await Foo ();
		if (i == 0)
			return precedentResult;
		else
			return i;
	}
}");
		}

		[Test]
		public void TestConditionalContinueWith() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod (int i)
	{
		var tcs = new TaskCompletionSource<int> ();
		var task1 = Foo ();
		var task2 = Foo ();
		if (i == 0) {
			task1.ContinueWith (precedent => {
				tcs.SetResult (1);
			});
		} else {
			task2.ContinueWith (precedent => {
				tcs.SetResult (2);
			});
		}
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod (int i)
	{
		var task1 = Foo ();
		var task2 = Foo ();
		if (i == 0) {
			int precedentResult = await task1;
			return 1;
		}
		else {
			int precedentResult1 = await task2;
			return 2;
		}
	}
}");
		}

		[Test]
		public void TestDelegate() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (delegate {
			tcs.SetResult (42);
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		await Foo ();
		return 42;
	}
}");
		}

		[Test]
		public void TestTwoContinueWith() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (precedent => {
			Foo ().ContinueWith (precedent2 => {
				tcs.SetResult (precedent.Result + precedent2.Result);
			});
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		int precedentResult = await Foo ();
		int precedent2Result = await Foo ();
		return precedentResult + precedent2Result;
	}
}");
		}

		[Test]
		public void TestChainedContinueWith() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public Task<int> $TestMethod ()
	{
		var tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith (precedent => {
			Foo ();
		}).ContinueWith (precedent => {
			tcs.SetResult (1);
		});
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public Task<int> Foo ()
	{
		return null;
	}
	public async Task<int> TestMethod ()
	{
		int precedentResult = await Foo ();
		Foo ();
		return 1;
	}
}");
		}

		[Test]
		public void TestRedundantReturn() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		tcs.SetResult (null);
		return tcs.Task;
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	public async Task TestMethod ()
	{
		object result = null;
		return;
	}
}");
		}

		[Test]
		public void TestNotInEnd() {
			TestWrongContext<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task Foo () {}
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		Foo ().ContinueWith (precedent => {
			tcs.SetResult (null);
			Console.WriteLine(42);
		});
		return tcs.Task;
	}
}");
		}

		[Test]
		public void TestInvalidChainedMethodAfterContinueWith() {
			TestWrongContext<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task Foo () {}
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		Foo ().ContinueWith (precedent => {
			tcs.SetResult (null);
		}).Wait ();
		return tcs.Task;
	}
}");
		}

		[Test]
		public void TestContinuationAfterSetResult() {
			TestWrongContext<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task Foo () {}
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		Foo ().ContinueWith (precedent => {
			tcs.SetResult (null);
		}).ContinueWith (precedent => {
		});
		return tcs.Task;
	}
}");
		}

		[Test]
		public void TestInvalidInLambda() {
			TestWrongContext<AutoAsyncIssue>(@"
using System;
using System.Threading.Tasks;
class TestClass
{
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		System.Action action = () => { tcs.SetResult (null); };
		return tcs.Task;
	}
}");
		}

		[Test]
		public void TestInvalidContinue() {
			Test<AutoAsyncIssue>(@"
using System;
using System.Threading.Tasks;
class TestClass
{
	public Task Foo() { return null; }
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		Foo ().ContinueWith (precedent => 1).ContinueWith (precedent => {
			Console.WriteLine (precedent.Result);
			tcs.SetResult (null);
		});
		return tcs.Task;
	}
}", @"
using System;
using System.Threading.Tasks;
class TestClass
{
	public Task Foo() { return null; }
	public async Task TestMethod ()
	{
		int precedentResult = await Foo ().ContinueWith (precedent => 1);
		Console.WriteLine (precedentResult);
		object result = null;
		return;
	}
}");
		}

		[Test]
		public void TestLongInvalidContinue() {
			Test<AutoAsyncIssue>(@"
using System;
using System.Threading.Tasks;
class TestClass
{
	public Task Foo() { return null; }
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		Foo ().ContinueWith (precedent => {
			return 1;
		}).ContinueWith (precedent => {
			Console.WriteLine (precedent.Result);
			tcs.SetResult (null);
		});
		return tcs.Task;
	}
}", @"
using System;
using System.Threading.Tasks;
class TestClass
{
	public Task Foo() { return null; }
	public async Task TestMethod ()
	{
		int precedentResult = await Foo ().ContinueWith (precedent =>  {
			return 1;
		});
		Console.WriteLine (precedentResult);
		object result = null;
		return;
	}
}");
		}

		[Test]
		public void TestDisabledForBadReturn() {
			TestWrongContext<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public Task $TestMethod ()
	{
		var tcs = new TaskCompletionSource<object> ();
		tcs.SetResult (null);
		return null;
	}
}");
		}

		[Test]
		public void TestDisabledForNoAction() {
			TestWrongContext<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	public void $TestMethod ()
	{
	}
}");
		}

		[Test]
		public void TestValidVoid() {
			Test<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	Task Foo() {}
	public void $TestMethod ()
	{
		Foo ().ContinueWith (precedent => {
			Console.WriteLine (1);
		});
	}
}", @"
using System.Threading.Tasks;
class TestClass
{
	Task Foo() {}
	public async void TestMethod ()
	{
		await Foo ();
		Console.WriteLine (1);
	}
}");
		}

		[Test]
		public void TestDisabledForBadMultipleReturns() {
			TestWrongContext<AutoAsyncIssue>(@"
using System.Threading.Tasks;
class TestClass
{
	Task Foo() { return null; }
	public Task<int> $TestMethod ()
	{
		TaskCompletionSource<int> tcs = new TaskCompletionSource<int> ();
		Foo ().ContinueWith(precedent => { tcs.SetResult(1); })
			.ContinueWith(precedent => { tcs.SetResult(1); });
		return tcs;
	}
}");
		}
	}
}

