// 
// DontUseLinqWhenItsVerboseAndInefficientTests.cs
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
	public class DontUseLinqWhenItsVerboseAndInefficientTests : InspectionActionTestBase
	{
		[Test]
		public void TestStringLength()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
class TestClass
{
	int TestMethod()
	{
		string x = ""Hello"";
		return x.Count ();
	}
}", @"
using System.Linq;
class TestClass
{
	int TestMethod()
	{
		string x = ""Hello"";
		return x.Length;
	}
}");
		}

		[Test]
		public void TestArrayCount()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
class TestClass
{
	int TestMethod()
	{
		int[] x = { 1, 2, 3 };
		return x.Count ();
	}
}", @"
using System.Linq;
class TestClass
{
	int TestMethod()
	{
		int[] x = { 1, 2, 3 };
		return x.Length;
	}
}");
		}

		[Test]
		public void TestStaticMethod()
		{
			TestWrongContext<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
class TestClass
{
	int TestMethod()
	{
		int[] x = { 1, 2, 3 };
		return Enumerable.Count<int> (x);
	}
}");
		}

		[Test]
		public void TestListCount()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		return x.Count ();
	}
}", @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		return x.Count;
	}
}");
		}

		[Test]
		public void TestListFirst()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		return x.First ();
	}
}", @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		return x [0];
	}
}");
		}

		[Test]
		public void TestListElementAt()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		return x.ElementAt (1);
	}
}", @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		return x [1];
	}
}");
		}

		[Test]
		public void TestListLast()
		{
			TestWrongContext<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		return x.Last ();
	}
}");
		}

		[Test]
		public void TestListSkippedReversedLast()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		int[] x = new int [10];
		return x.Skip (5).Reverse ().Last ();
	}
}", @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		int[] x = new int [10];
		return x [5];
	}
}");
		}

		[Test]
		public void TestListSkippedLast()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		int[] x = new int [10];
		return x.Skip (5).Reverse ().Skip (5).Reverse ().Last ();
	}
}", @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		int[] x = new int [10];
		return x [x.Length - 1 - (5)];
	}
}");
		}

		[Test]
		public void TestListSkip()
		{
			Test<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		int y = 5;
		return x.Skip (y).First ();
	}
}", @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	int TestMethod()
	{
		IList<int> x = new List<int> ();
		int y = 5;
		return x [y];
	}
}");
		}

		[Test]
		public void TestDisabledForNoCount()
		{
			TestWrongContext<DontUseLinqWhenItsVerboseAndInefficientIssue>(@"
using System.Linq;
using System.Collections;
using System.Collections.Generic;
class MyCollection : ICollection<int>
{
	int ICollection<int>.Count { get { return 0; } }
	bool ICollection<int>.IsReadOnly { get { return true; } }

	public IEnumerator<int> GetEnumerator()
	{
		return null;
	}
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(int item) { }
	public bool Contains(int item) { return false; }
	public void CopyTo(int[] array, int arrayIndex) {}
	public bool Remove(int item) { return false; }
}
class TestClass
{
	int TestMethod()
	{
		MyCollection x = new MyCollection ();
		return x.Count ();
	}
}");
		}
	}
}