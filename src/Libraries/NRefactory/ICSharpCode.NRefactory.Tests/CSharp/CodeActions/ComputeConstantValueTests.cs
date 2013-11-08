// 
// ComputeConstantValueTests.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ComputeConstantValueTests : ContextActionTestBase
	{
		[Test]
		public void Rational1()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		int a = 1 $+ 1;
	}
}", @"
class TestClass
{
	public void F()
	{
		int a = 2;
	}
}");
		}
		[Test]
		public void Rational2()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		int b = 2 $* 2;
	}
}", @"
class TestClass
{
	public void F()
	{
		int b = 4;
	}
}");
		}
		[Test]
		public void Rational3()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		double c = 0.2 $/ 2;
	}
}", @"
class TestClass
{
	public void F()
	{
		double c = 0.1;
	}
}");
		}
		[Test]
		public void Rational4()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		double d = 2 $* (-0.2);
	}
}", @"
class TestClass
{
	public void F()
	{
		double d = -0.4;
	}
}");
		}
		[Test]
		public void Rational5()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		int e = 2 $* (1 << 2);
	}
}", @"
class TestClass
{
	public void F()
	{
		int e = 8;
	}
}");
		}
		[Test]
		public void Rational6()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		int f = 1 $+ (~4);
	}
}", @"
class TestClass
{
	public void F()
	{
		int f = -4;
	}
}");
		}

		[Test]
		public void Bool1()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		bool a = $!(true);
	}
}", @"
class TestClass
{
	public void F()
	{
		bool a = false;
	}
}");
		}

		[Test]
		public void Bool2()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		bool b = $!!(!!!(true & false));
	}
}", @"
class TestClass
{
	public void F()
	{
		bool b = true;
	}
}");
		}

		[Test]
		public void Bool3()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		bool c = 1 $> 0;
	}
}", @"
class TestClass
{
	public void F()
	{
		bool c = true;
	}
}");
		}

		[Test]
		public void String1()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		string a = ""a""$+""b"";
	}
}", @"
class TestClass
{
	public void F()
	{
		string a = ""ab"";
	}
}");
		}

		[Test]
		public void UseConstant()
		{
			Test<ComputeConstantValueAction>(@"
class TestClass
{
	const double pi = 3.141;
	public void F()
	{
		double pi2 = $2 * pi;
	}
}", @"
class TestClass
{
	const double pi = 3.141;
	public void F()
	{
		double pi2 = 6.282;
	}
}");
		}


		[Test]
		public void Invalid()
		{
			TestWrongContext<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		bool a = !(true);
		bool b = $!!(!!!(true & a));
	}
}");
		}


		[Test]
		public void TestWrongHotSpot()
		{
			TestWrongContext<ComputeConstantValueAction>(@"
class TestClass
{
	public void F()
	{
		int a = 1 +$ 1;
	}
}");
		}
	}
}
