//
// CS0659OverrideEqualsWithoutGetHashCodeTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeIssues;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS0659OverrideEqualsWithoutGetHashCodeTests : InspectionActionTestBase
	{
		[Test]
		public void WithoutGetHashCode()
		{
			var input = @"
namespace application
{
	public class BaseClass
	{
		public override bool Equals(object o)
		{
			return base.Equals(o);
		}
	}
}";
			var output = @"
namespace application
{
	public class BaseClass
	{
		public override bool Equals(object o)
		{
			return base.Equals(o);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
}";
			Test<CS0659ClassOverrideEqualsWithoutGetHashCode>(input, output);
		}

		[Test]
		public void WithoutEquals()
		{
			var input = @"

namespace application
{
	public class Program
	{
		public bool Equals(Program o)
		{
			return false;
		}
	}
}";
			Test<CS0659ClassOverrideEqualsWithoutGetHashCode>(input, 0);
		}

		[Test]
		public void PatialClass()
		{
			var input = @"
namespace application
{
	public partial class BaseClass
	{
		public override bool Equals(object o)
		{
			return base.Equals(o);
		}
	}
	public partial class BaseClass
	{
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
}";

			Test<CS0659ClassOverrideEqualsWithoutGetHashCode>(input, 0);
		}

		[Test]
		public void WithGetHashCode()
		{
			var input = @"
namespace application
{
	public class Program
	{
		public override int GetHashCode()
		{
			return 1;
		}
		public override bool Equals(Object o)
		{
			return false;
		}
	}
}";
			Test<CS0659ClassOverrideEqualsWithoutGetHashCode>(input, 0);
		}

		[Test]
		public void ResharperDisable()
		{
			var input = @"
namespace application
{
	public class Program
	{
//Resharper disable CSharpWarnings::CS0659
		public override bool Equals(Object o)
		{
			return false;
		}
//Resharper restore CSharpWarnings::CS0659
	}
}";
			Test<CS0659ClassOverrideEqualsWithoutGetHashCode>(input, 0);
		}

		[Test]
		public void TestPragmaSuppression()
		{
			var input = @"
namespace application
{
	public class Program
	{
#pragma warning disable 0659
		public override bool Equals(Object o)
		{
			return false;
		}
#pragma warning restore 0659
	}
}";
			Test<CS0659ClassOverrideEqualsWithoutGetHashCode>(input, 0);
		}
	}
}