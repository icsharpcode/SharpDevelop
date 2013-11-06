//
// RedundantEnumerableCastCallIssueTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
	public class RedundantEnumerableCastCallIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestRedundantCast()
		{
			Test<RedundantEnumerableCastCallIssue>(@"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		var a = args.Cast<string> ();
	}
}
", @"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		var a = args;
	}
}
");
		}

		[Test]
		public void TestRedundantCastCase2()
		{
			Test<RedundantEnumerableCastCallIssue>(@"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (string[] args)
	{
		var a = args.Cast<string> ();
	}
}
", @"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (string[] args)
	{
		var a = args;
	}
}
");
		}

		[Test]
		public void TestRedundantOfType()
		{
			Test<RedundantEnumerableCastCallIssue>(@"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		var a = args.OfType<string> ();
	}
}
", @"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		var a = args.Where (i => i != null);
	}
}
");
		}

		[Test]
		public void TestRedundantOfTypeResolution2()
		{
			Test<RedundantEnumerableCastCallIssue>(@"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		var a = args.OfType<string> ();
	}
}
", @"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		var a = args;
	}
}
", 1);
		}


		[Test]
		public void TestInvalid()
		{
			TestWrongContext<RedundantEnumerableCastCallIssue>(@"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		var a = args.Cast<object> ();
	}
}
");
		}


		[Test]
		public void TestDisable()
		{
			TestWrongContext<RedundantEnumerableCastCallIssue>(@"
using System;
using System.Linq;
using System.Collections.Generic;

class Test
{
	static void Main (IEnumerable<string> args)
	{
		// ReSharper disable once RedundantEnumerableCastCall
		var a = args.Cast<string> ();
	}
}
");
		}

	}
}

