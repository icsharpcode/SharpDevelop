// LockThisTests.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis <luiscubal@gmail.com>
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

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class LockThisTests : InspectionActionTestBase
	{
		[Test]
		public void TestLockThisInMethod ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		lock (this) {
		}
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	void TestMethod ()
	{
		lock (locker) {
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestLockThisInGetter ()
		{
			var input = @"
class TestClass
{
	int MyProperty {
		get {
			lock (this) {
				return 0;
			}
		}
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	int MyProperty {
		get {
			lock (locker) {
				return 0;
			}
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestLockThisInSetter ()
		{
			var input = @"
class TestClass
{
	int MyProperty {
		set {
			lock (this) {
			}
		}
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	int MyProperty {
		set {
			lock (locker) {
			}
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestLockThisInConstructor ()
		{
			var input = @"
class TestClass
{
	TestClass()
	{
		lock (this) {
		}
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	TestClass()
	{
		lock (locker) {
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestLockThisInDelegate ()
		{
			var input = @"
class TestClass
{
	TestClass()
	{
		Action lockThis = delegate ()
		{
			lock (this) {
			}
		};
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	TestClass()
	{
		Action lockThis = delegate ()
		{
			lock (locker) {
			}
		};
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestLockThisInLambda ()
		{
			var input = @"
class TestClass
{
	TestClass()
	{
		Action lockThis = () =>
		{
			lock (this) {
			}
		};
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	TestClass()
	{
		Action lockThis = () =>
		{
			lock (locker) {
			}
		};
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestLockParenthesizedThis ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		lock ((this)) {
		}
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	void TestMethod ()
	{
		lock (locker) {
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestFixMultipleLockThis ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		lock (this) {
		}
	}

	void TestMethod2 ()
	{
		lock (this) {
		}
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	void TestMethod ()
	{
		lock (locker) {
		}
	}

	void TestMethod2 ()
	{
		lock (locker) {
		}
	}
}";

			Test<LockThisIssue> (input, 2, output, 0);
		}
		[Test]
		public void TestFixMixedLocks ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		lock (this) {
		}
	}

	object locker2 = new object ();
	void TestMethod2 ()
	{
		lock (locker2) {
		}
	}
}";

			var output = @"
class TestClass
{
	object locker = new object ();
	void TestMethod ()
	{
		lock (locker) {
		}
	}

	object locker2 = new object ();
	void TestMethod2 ()
	{
		lock (locker2) {
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestLockNonThis ()
		{
			var input = @"
class TestClass
{
	object locker = new object ();

	TestClass()
	{
		lock (locker) {
		}
	}
}";

			Test<LockThisIssue> (input, 0);
		}

		[Test]
		public void TestNestedTypeLock ()
		{
			var input = @"
class TestClass
{
	class Nested
	{
		Nested()
		{
			lock (this) {
			}
		}
	}
}";

			var output = @"
class TestClass
{
	class Nested
	{
		object locker = new object ();
		Nested()
		{
			lock (locker) {
			}
		}
	}
}";

			Test<LockThisIssue>(input, 1, output);
		}

		[Test]
		public void TestMethodSynchronized ()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized)]
	void TestMethod ()
	{
		System.Console.WriteLine (""Foo"");
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
class TestClass
{
	object locker = new object ();
	void TestMethod ()
	{
		lock (locker) {
			System.Console.WriteLine (""Foo"");
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestMethodWithSynchronizedValue ()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	[MethodImpl (Value = MethodImplOptions.Synchronized)]
	void TestMethod ()
	{
		System.Console.WriteLine (""Foo"");
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
class TestClass
{
	object locker = new object ();
	void TestMethod ()
	{
		lock (locker) {
			System.Console.WriteLine (""Foo"");
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestMethodHasSynchronized ()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized | MethodImplOptions.NoInlining)]
	void TestMethod ()
	{
		System.Console.WriteLine (""Foo"");
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
class TestClass
{
	object locker = new object ();
	[MethodImpl (MethodImplOptions.NoInlining)]
	void TestMethod ()
	{
		lock (locker) {
			System.Console.WriteLine (""Foo"");
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestMethodNotSynchronized ()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	[MethodImpl (MethodImplOptions.NoInlining)]
	void TestMethod ()
	{
		System.Console.WriteLine (""Foo"");
	}
}";

			Test<LockThisIssue> (input, 0);
		}

		[Test]
		public void TestAbstractSynchronized ()
		{
			var input = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized)]
	public abstract void TestMethod ();
}";

			var output = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	public abstract void TestMethod ();
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestDoubleLocking ()
		{
			var input = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized)]
	public void TestMethod ()
	{
		lock (this) {
		}
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	object locker = new object ();
	public void TestMethod ()
	{
		lock (locker) {
			lock (locker) {
			}
		}
	}
}";

			Test<LockThisIssue> (input, 2, output, 0);
		}

		[Test]
		public void TestDelegateLocking ()
		{
			var input = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized)]
	public void TestMethod ()
	{
		Action action = delegate {
			lock (this) {
			}
		};
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	object locker = new object ();
	public void TestMethod ()
	{
		lock (locker) {
			Action action = delegate {
				lock (locker) {
				}
			};
		}
	}
}";

			Test<LockThisIssue> (input, 2, output, 0);
		}

		[Test]
		public void TestLambdaLocking ()
		{
			var input = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized)]
	public void TestMethod ()
	{
		Action action = () => {
			lock (this) {
			}
		};
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
abstract class TestClass
{
	object locker = new object ();
	public void TestMethod ()
	{
		lock (locker) {
			Action action = () => {
				lock (locker) {
				}
			};
		}
	}
}";

			Test<LockThisIssue> (input, 2, output, 0);
		}

		[Test]
		public void TestStaticMethod()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized)]
	public static void TestMethod ()
	{
		Console.WriteLine ();
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
class TestClass
{
	static object locker = new object ();
	public static void TestMethod ()
	{
		lock (locker) {
			Console.WriteLine ();
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestStaticProperty()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	public static int TestProperty
	{
		[MethodImpl (MethodImplOptions.Synchronized)]
		set {
			Console.WriteLine (value);
		}
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
class TestClass
{
	static object locker = new object ();
	public static int TestProperty
	{
		set {
			lock (locker) {
				Console.WriteLine (value);
			}
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}

		[Test]
		public void TestMixedStaticMethod()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	[MethodImpl (MethodImplOptions.Synchronized)]
	public void TestMethod ()
	{
		Console.WriteLine ();
	}

	[MethodImpl (MethodImplOptions.Synchronized)]
	public static void TestStaticMethod ()
	{
		Console.WriteLine ();
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
class TestClass
{
	object locker = new object ();
	public void TestMethod ()
	{
		lock (locker) {
			Console.WriteLine ();
		}
	}

	[MethodImpl (MethodImplOptions.Synchronized)]
	public static void TestStaticMethod ()
	{
		Console.WriteLine ();
	}
}";

			Test<LockThisIssue> (input, 2, output, 0);
		}

		[Test]
		public void TestNewNameLock()
		{
			var input = @"
using System.Runtime.CompilerServices;
class TestClass
{
	int locker;
	[MethodImpl (MethodImplOptions.Synchronized)]
	public void TestMethod ()
	{
		Console.WriteLine ();
	}
}";

			var output = @"
using System.Runtime.CompilerServices;
class TestClass
{
	int locker;
	object locker1 = new object ();
	public void TestMethod ()
	{
		lock (locker1) {
			Console.WriteLine ();
		}
	}
}";

			Test<LockThisIssue> (input, 1, output);
		}
	}
}
