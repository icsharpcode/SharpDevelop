//
// LinqFluentToQueryTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class LinqFluentToQueryTests : ContextActionTestBase
	{
		[Test]
		public void TestBasicCase()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$Select (t => t);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in new int[0]
		        select t;
	}
}");
		}

		[Test]
		public void TestAddedParenthesis()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$Select (t => t) + 1;
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = (from t in new int[0]
select t) + 1;
	}
}");
		}

		[Test]
		public void TestCast()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$Cast<int> ();
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from int _1 in new int[0]
		        select _1;
	}
}");
		}

		[Test]
		public void TestLet()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].Select (w => new { w, two = w * 2 }).$Select (_ => _.two);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from w in new int[0]
		        let two = w * 2
		        select two;
	}
}");
		}

		[Test]
		public void TestLet2()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].Select (w => new { two = w * 2, w }).$Select (_ => _.two);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from w in new int[0]
		        let two = w * 2
		        select two;
	}
}");
		}

		[Test]
		public void TestLongLetChain()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].Select (w => new { w, two = w * 2 })
			.Select (h => new { h, three = h.w * 3 })
			.Select (k => new { k, four = k.h.w * 4 })
			.$Select (_ => _.k.h.w + _.k.h.two + _.k.three + _.four)
			.Select (sum => sum * 2);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from w in new int[0]
		        let two = w * 2
		        let three = w * 3
		        let four = w * 4
		        select w + two + three + four into sum
		        select sum * 2;
	}
}");
		}

		[Test]
		public void TestLongLetChain2()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].Select (w => new { two = w * 2, w })
			.Select (h => new { three = h.w * 3, h })
			.Select (k => new { four = k.h.w * 4, k })
			.$Select (_ => _.k.h.w + _.k.h.two + _.k.three + _.four)
			.Select (sum => sum * 2);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from w in new int[0]
		        let two = w * 2
		        let three = w * 3
		        let four = w * 4
		        select w + two + three + four into sum
		        select sum * 2;
	}
}");
		}

		[Test]
		public void TestSelectMany()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$SelectMany (elem => new int[0], (elem1, elem2) => elem1 + elem2);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from elem1 in new int[0]
		        from elem2 in new int[0]
		        select elem1 + elem2;
	}
}");
		}

		[Test]
		public void TestSelectManyLet()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$SelectMany (elem => new int[0], (elem1, elem2) => new { elem1, elem2 }).Select(i => new { i, sum = i.elem1 + i.elem2 })
			.Select(j => j.i.elem1 + j.i.elem2 + j.sum);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from elem1 in new int[0]
		        from elem2 in new int[0]
		        let sum = elem1 + elem2
		        select elem1 + elem2 + sum;
	}
}");
		}

		[Test]
		public void TestSelectManyLet2()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$SelectMany (elem => new int[0], (elem1, elem2) => new { elem1, elem2 = elem2 + 1 }).Select(i => new { i, sum = i.elem1 + i.elem2 })
			.Select(j => j.i.elem1 + j.i.elem2 + j.sum);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from elem1 in new int[0]
		        from elem2 in new int[0]
		        select new {
	elem1,
	elem2 = elem2 + 1
} into i
		        let sum = i.elem1 + i.elem2
		        select i.elem1 + i.elem2 + sum;
	}
}");
		}

		[Test]
		public void TestCastSelect()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$Cast<int> ().Select (t => t * 2);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from int t in new int[0]
		        select t * 2;
	}
}");
		}

		[Test]
		public void TestSelectWhere ()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$Where (t => t > 0).Select (t => t * 2);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in new int[0]
		        where t > 0
		        select t * 2;
	}
}");
		}

		[Test]
		public void TestSorting ()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$OrderBy (t => t).ThenByDescending (t => t);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in new int[0]
		        orderby t, t descending
		        select t;
	}
}");
		}

		[Test]
		public void TestDegenerateWhere ()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$Where (t => t > 0);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in new int[0]
		        where t > 0
		        select t;
	}
}");
		}

		[Test]
		public void TestChain ()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].Where (t => t > 0).$Where (u => u > 0);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in new int[0]
		        where t > 0
		        select t into u
		        where u > 0
		        select u;
	}
}");
		}

		[Test]
		public void TestJoin()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].Cast<char> ().$Join(new int[0].Cast<float> (), a => a * 2, b => b, (l, r) => l * r);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from char a in new int[0]
		        join float b in new int[0] on a * 2 equals b
		        select a * b;
	}
}");
		}

		[Test]
		public void TestGroupJoin()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].Cast<char> ().$GroupJoin(new int[0].Cast<float> (), a => a * 2, b => b, (l, r) => l * r [0]);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from char a in new int[0]
		        join float b in new int[0] on a * 2 equals b into r
		        select a * r [0];
	}
}");
		}

		[Test]
		public void TestNonRecursive()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = Enumerable.Empty<int[]> ().$Select (t => t.Select (v => v));
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in Enumerable.Empty<int[]> ()
		        select t.Select (v => v);
	}
}");
		}

		[Test]
		public void TestNonRecursiveCombineQueries()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = Enumerable.Empty<int[]> ().$Select (t => (from g in t select g));
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in Enumerable.Empty<int[]> ()
		        select (from g in t
		      select g);
	}
}");
		}

		[Test]
		public void TestGroupBy()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$GroupBy (t => t, t => new int[0]);
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from t in new int[0]
		        group new int[0] by t;
	}
}");
		}

		[Test]
		public void Test_1AlreadyUsed()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		int _1;
		var x = new int[0].$Cast<float> ();
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		int _1;
		var x = from float _2 in new int[0]
		        select _2;
	}
}");
		}

		[Test]
		public void TestDoubleCasts()
		{
			Test<LinqFluentToQueryAction>(@"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = new int[0].$Cast<float> ().Cast<int> ();
	}
}", @"
using System.Linq;

class TestClass
{
	void TestMethod ()
	{
		var x = from int _1 in
		            from float _2 in new int[0]
		select _2
		        select _1;
	}
}");
		}
	}
}

