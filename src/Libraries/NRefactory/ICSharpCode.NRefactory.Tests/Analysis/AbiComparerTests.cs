//
// AbiComparerTests.cs
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
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeCompletion;

namespace ICSharpCode.NRefactory.Analysis
{
	[TestFixture]
	public class AbiComparerTests
	{
		AbiCompatibility Check (string before, string after)
		{
			IProjectContent oldPctx, newPctx;
			SyntaxTree tree;
			ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile file;
			CodeCompletionBugTests.CreateCompilation (before, out oldPctx, out tree, out file, false);
			CodeCompletionBugTests.CreateCompilation (after, out newPctx, out tree, out file, false);
			return new AbiComparer ().Check (oldPctx.CreateCompilation (), newPctx.CreateCompilation ());
		}

		[Test]
		public void CheckEquality()
		{
			string a1 = @"namespace Foo { public class Bar { public void FooBar () {} public int Bar2 { get; set;} int removed; } class Removed {} }";
			string a2 = @"namespace Foo { public class Bar { public void FooBar () {} public int Bar2 { get {} set{}} void Added () {} } class Added {} } namespace Added { class Test { } }";
			Assert.AreEqual (AbiCompatibility.Equal, Check (a1, a2));
		}

		[Test]
		public void CheckBigger()
		{
			string a1 = @"namespace Foo { public class Bar { public void FooBar () {} } }";
			string a2 = @"namespace Foo { public class Bar { public void FooBar () {} public void BarFoo () {} } }";
			Assert.AreEqual (AbiCompatibility.Bigger, Check (a1, a2));
		}

		[Test]
		public void CheckIncompatible()
		{
			string a1 = @"namespace Foo { public class Bar { public void FooBar () {} } }";
			string a2 = @"namespace Foo { public class Bar { public void FooBar (int bar) {} } }";
			Assert.AreEqual (AbiCompatibility.Incompatible, Check (a1, a2));
		}

		[Test]
		public void CheckIncompatibleInterfaceChange()
		{
			string a1 = @"public interface IFoo {}";
			string a2 = @"public interface IFoo { void Bar (); }";
			Assert.AreEqual (AbiCompatibility.Incompatible, Check (a1, a2));
		}

		[Test]
		public void CheckTypeConstraintChange()
		{
			string a1 = @"public class IFoo<T> {}";
			string a2 = @"public class IFoo<T> where T : System.IDisposable {}";
			Assert.AreEqual (AbiCompatibility.Incompatible, Check (a1, a2));
		}

		[Test]
		public void CheckTypeConstraintChangeCase2()
		{
			string a1 = @"public class IFoo<T> {}";
			string a2 = @"public class IFoo<T> where T : class {}";
			Assert.AreEqual (AbiCompatibility.Incompatible, Check (a1, a2));
		}

		[Test]
		public void CheckMethodConstraintChange()
		{
			string a1 = @"public class IFoo { public void Bar<T> () {} }";
			string a2 = @"public class IFoo { public void Bar<T> () where T : System.IDisposable {} }";
			Assert.AreEqual (AbiCompatibility.Incompatible, Check (a1, a2));
		}
	}
}
