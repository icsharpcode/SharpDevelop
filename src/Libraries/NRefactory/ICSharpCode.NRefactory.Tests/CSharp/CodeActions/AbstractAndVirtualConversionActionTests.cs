//
// AbstractAndVirtualConversionActionTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class AbstractAndVirtualConversionActionTests : ContextActionTestBase
	{
		[Test]
		public void VirtualToNonVirtualTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"class Test
{
	public $virtual void Foo ()
	{
	}
}", @"class Test
{
	public void Foo ()
	{
	}
}");
		}

		[Test]
		public void VirtualToAbstractTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"abstract class Test
{
	public $virtual void Foo ()
	{
	}
}", @"abstract class Test
{
	public abstract void Foo ();
}");
		}


		[Test]
		public void VirtualIndexerToAbstractTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"abstract class MainClass
{
	public virtual int $this [int i] {
		get {
			;
		}
	}
}", @"abstract class MainClass
{
	public abstract int this [int i] {
		get;
	}
}");
		}


		[Test]
		public void NonVirtualStaticToVirtualTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"class Test
{
	public static void $Foo ()
	{
	}
}", @"class Test
{
	public virtual void Foo ()
	{
	}
}");
		}


		[Test]
		public void NonVirtualToVirtualTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"class Test
{
	public void $Foo ()
	{
	}
}", @"class Test
{
	public virtual void Foo ()
	{
	}
}");
		}

		[Test]
		public void InvalidPrivateImplementationTypeTest()
		{
			TestWrongContext<AbstractAndVirtualConversionAction>(
				@"using System;
class Test : IDisposable
{
	void IDisposable.$Dispose ()
	{
	}
}");
		}

		[Test]
		public void AbstractToNonAbstractTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"abstract class Test
{
	public $abstract void Foo ();
}", @"abstract class Test
{
	public void Foo ()
	{
		throw new System.NotImplementedException ();
	}
}");
		}

		[Test]
		public void AbstractToVirtualTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"abstract class Test
{
	public $abstract void Foo ();
}", @"abstract class Test
{
	public virtual void Foo ()
	{
		throw new System.NotImplementedException ();
	}
}", 1);
		}

		[Test]
		public void AbstractPropertyToNonAbstractTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"abstract class Test
{
	public abstract int $Foo {
		get;
		set;
	}
}", @"abstract class Test
{
	public int Foo {
		get {
			throw new System.NotImplementedException ();
		}
		set {
			throw new System.NotImplementedException ();
		}
	}
}");
		}

		[Test]
		public void AbstractEventToNonAbstractTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"using System;
abstract class Test
{
	public abstract event EventHandler $Foo;
}", @"using System;
abstract class Test
{
	public event EventHandler Foo;
}");
		}

		[Test]
		public void NonAbstractToAbstractTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"abstract class Test
{
	public void $Foo ()
	{
		throw new System.NotImplementedException ();
	}
}", @"abstract class Test
{
	public abstract void Foo ();
}");
		}

		[Test]
		public void NonAbstractEventToAbstractTest()
		{
			Test<AbstractAndVirtualConversionAction>(
				@"abstract class Test
{
	public event EventHandler $Foo  {
		add {
			throw new System.NotImplementedException ();
		}
		remove {
			throw new System.NotImplementedException ();
		}
	}
}", @"abstract class Test
{
	public abstract event EventHandler Foo;
}");
		}


		[Test]
		public void InvalidLocalContext()
		{
			TestWrongContext<AbstractAndVirtualConversionAction>(
				@"using System;
class Test
{
	public static void Main (string[] args)
	{
		int $fooBar = 1;
	}
}");
		}


		[Test]
		public void InvalidOverrideTest()
		{
			TestWrongContext<AbstractAndVirtualConversionAction>(
				@"using System;
class Test
{
	public override string $ToString()
	{
	}
}");
		}

		[Test]
		public void InvalidMethodTest()
		{
			var actions = GetActions<AbstractAndVirtualConversionAction>(
				@"using System;
abstract class Test
{
	public virtual string $ToString()
	{
		Console.WriteLine (""Hello World"");
	}
}");
			// only virtual -> non virtual should be provided - no abstract conversion
			Assert.AreEqual(1, actions.Count); 
		}

	}
}

