// 
// LinqQueryToFluentTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class LinqQueryToFluentTests : ContextActionTestBase
	{
		[Test]
		public void TestSimpleQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
                   select x;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().Select (x => x);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestName()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		int _;
		var _2 = $from x in System.Enumerable.Empty<int> ()
		         let _1 = x
		         select x;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		int _;
		var _2 = System.Enumerable.Empty<int> ().Select (x => new {
	x,
	_1 = x
}).Select (_3 => _3.x);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestPrecedence()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in true ? System.Enumerable.Empty<int> () : null
			       select x;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = (true ? System.Enumerable.Empty<int> () : null).Select (x => x);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestWhereQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
		           where x > 1
		           select x;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().Where (x => x > 1);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestOrderByQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
		           orderby x, x * 2 descending
		           select x;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().OrderBy (x => x).ThenByDescending (x => x * 2);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestDoubleFromWithSelectQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = System.Enumerable.Empty<int> ();
		var data = $from x in System.Enumerable.Empty<int> ()
		           from y in newEnumerable
		           select x * y;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = System.Enumerable.Empty<int> ();
		var data = System.Enumerable.Empty<int> ().SelectMany (x => newEnumerable, (x, y) => x * y);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestDoubleFromWithCastQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = System.Enumerable.Empty<int> ();
		var data = $from x in System.Enumerable.Empty<int> ()
		           from float y in newEnumerable
		           select x * y;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = System.Enumerable.Empty<int> ();
		var data = System.Enumerable.Empty<int> ().SelectMany (x => newEnumerable.Cast<float> (), (x, y) => x * y);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestDoubleFromWithIntermediateQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = System.Enumerable.Empty<int> ();
		var data = $from x in System.Enumerable.Empty<int> ()
		           from y in newEnumerable
		           where x > y
		           select x * y;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = System.Enumerable.Empty<int> ();
		var data = System.Enumerable.Empty<int> ().SelectMany (x => newEnumerable, (x, y) => new {
	x,
	y
}).Where (_ => _.x > _.y).Select (_1 => _1.x * _1.y);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestLetQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
		           let y = x * 2
		           select x * y;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().Select (x => new {
	x,
	y = x * 2
}).Select (_ => _.x * _.y);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestLongChainQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
		           let y = x * 2
		           let z = x * y * 2
		           select x * y * z;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().Select (x => new {
	x,
	y = x * 2
}).Select (_ => new {
	_,
	z = _.x * _.y * 2
}).Select (_1 => _1._.x * _1._.y * _1.z);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestCastQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from float x in System.Enumerable.Empty<int> ()
		           select x;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().Cast<float> ().Select (x => x);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestJoinQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 4, 5, 6 };
		var data = $from x in System.Enumerable.Empty<int> ()
		           join float yy in newEnumerable on x * 2 equals yy
		           select x * yy;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 4, 5, 6 };
		var data = System.Enumerable.Empty<int> ().Join (newEnumerable.Cast<float> (), x => x * 2, yy => yy, (x, yy) => x * yy);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestJoinWithIntermediateQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 4, 5, 6 };
		var data = $from x in System.Enumerable.Empty<int> ()
		           join float y in newEnumerable on x * 2 equals y
		           where x == 2
		           select x * y;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 4, 5, 6 };
		var data = System.Enumerable.Empty<int> ().Join (newEnumerable.Cast<float> (), x => x * 2, y => y, (x, y) => new {
	x,
	y
}).Where (_ => _.x == 2).Select (_1 => _1.x * _1.y);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestJoinWithIntoSelectQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 1, 2, 3 };
		var data = $from x in System.Enumerable.Empty<int> ()
		           join y in newEnumerable on x * 2 equals y
		           into g
		           select g;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 1, 2, 3 };
		var data = System.Enumerable.Empty<int> ().GroupJoin (newEnumerable, x => x * 2, y => y, (x, g) => g);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestJoinWithIntoIntermediateQuery()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 1, 2, 3 };
		var data = $from x in System.Enumerable.Empty<int> ()
		           join y in newEnumerable on x * 2 equals y
		           into g
		           where true
		           select g;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var newEnumerable = new int[] { 1, 2, 3 };
		var data = System.Enumerable.Empty<int> ().GroupJoin (newEnumerable, x => x * 2, y => y, (x, g) => new {
	x,
	g
}).Where (_ => true).Select (_1 => _1.g);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestSimpleGroup()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
		           group x by x % 10;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().GroupBy (x => x % 10);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestDifferentGroup()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
		           group x / 10 by x % 10;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().GroupBy (x => x % 10, x => x / 10);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestInto()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = $from x in System.Enumerable.Empty<int> ()
		           select x * 2 into y
		           select y * 3;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var data = System.Enumerable.Empty<int> ().Select (x => x * 2).Select (y => y * 3);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestDoubleFromWithLet()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var src = System.Enumerable.Empty<int> ();
		var data = $from x in src
		           from y in src
		           let k = x * y
		           select k;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var src = System.Enumerable.Empty<int> ();
		var data = src.SelectMany (x => src, (x, y) => new {
	x,
	y
}).Select (_ => new {
	_,
	k = _.x * _.y
}).Select (_1 => _1.k);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}

		[Test]
		public void TestDoubleFromWithMidLet()
		{
			string input = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var src = System.Enumerable.Empty<int> ();
		var data = $from x in src
		           let k = x * x
		           from y in src
		           select k * y;
	}
}
";

			string output = @"
using System.Linq;
public class TestClass
{
	public void TestMethod()
	{
		var src = System.Enumerable.Empty<int> ();
		var data = src.Select (x => new {
	x,
	k = x * x
}).SelectMany (_ => src, (_1, y) => _1.k * y);
	}
}
";

			Assert.AreEqual(output, RunContextAction(new LinqQueryToFluentAction(), input));
		}
	}
}

