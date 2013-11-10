// 
// RedundantLambdaParameterTypeTests.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
	public class RedundantLambdaParameterTypeTests : InspectionActionTestBase
	{

		[Test]
		public void SimpleCase()
		{
			Test<RedundantLambdaParameterTypeIssue>(@"
class Program
{
	public delegate int IncreaseByANumber(int j);

	public static void ExecuteCSharp3_0()
	{
		IncreaseByANumber increase = (int j) => (j * 42);
	}
}
", @"
class Program
{
	public delegate int IncreaseByANumber(int j);

	public static void ExecuteCSharp3_0()
	{
		IncreaseByANumber increase = j => (j * 42);
	}
}
");
		}

		[Test]
		public void MultipleCases()
		{
			Test<RedundantLambdaParameterTypeIssue>(@"
class Program
{
	public delegate int MultipleIncreaseByANumber(int i, string j, int l);

	public static void ExecuteCSharp3_0()
	{
		MultipleIncreaseByANumber multiple = (int j, string k, int l) => ((j * 42) / k) % l;
	}
}
", 3, @"
class Program
{
	public delegate int MultipleIncreaseByANumber(int i, string j, int l);

	public static void ExecuteCSharp3_0()
	{
		MultipleIncreaseByANumber multiple = (j, k, l) => ((j * 42) / k) % l;
	}
}
", 0);
		}

		[Test]
		public void TestInspectorCase2()
		{
			Test<RedundantLambdaParameterTypeIssue>(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace application
{
	internal class Program
	{
		public void Foo(Action<int, string> act) {}
		public void Foo(Action<int> act) {}

		void Test ()
		{
			Foo((int i) => Console.WriteLine (i));
		}
	}
}", @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace application
{
	internal class Program
	{
		public void Foo(Action<int, string> act) {}
		public void Foo(Action<int> act) {}

		void Test ()
		{
			Foo(i => Console.WriteLine (i));
		}
	}
}");
		}

		[Test]
		public void TestInspectorCase3()
		{
			var input = @"using System;
using System.Collections.Generic;
using System.Linq;

namespace application
{
	internal class Program
	{
		public void Foo(Action<int> act, Action<int> act1) { }
		public void Foo(Action<string> act, Action<int> act1) { }

		void Test()
		{
			Foo(((int i) => Console.WriteLine(i)), (i => Console.WriteLine(i)));
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantLambdaParameterTypeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}


		[Test]
		public void TestInvalidContext()
		{
			var input = @"using System;
		using System.Collections.Generic;
		using System.Linq;

namespace application
{
	internal class Program
	{
		public void Foo(Action<int> act) {}
		public void Foo(Action<string> act) {}

		void Test ()
		{
			Foo((int i) => Console.WriteLine (i));
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantLambdaParameterTypeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestResharperDisableRestore()
		{
			var input = @"using System;
		using System.Collections.Generic;
		using System.Linq;

namespace application
{
	internal class Program
	{
		public delegate int IncreaseByANumber(int j);

		public delegate int MultipleIncreaseByANumber(int i, int j, int l);

		public static void ExecuteCSharp3_0()
		{
			// declare the lambda expression
			//Resharper disable RedundantLambdaParameterType
			IncreaseByANumber increase = (int j) => (j * 42);
			//Resharper restore RedundantLambdaParameterType
			// invoke the method and print 420 to the console
			Console.WriteLine(increase(10));

			MultipleIncreaseByANumber multiple = (j, k, l) => ((j * 42) / k) % l;
			Console.WriteLine(multiple(10, 11, 12));
		}
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues(new RedundantLambdaParameterTypeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
	}
}