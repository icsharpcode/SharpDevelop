// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;

using Hornung.ResourceToolkit.Refactoring;
using Hornung.ResourceToolkit.Resolver;
using NUnit.Framework;

namespace ResourceToolkit.Tests.CSharp
{
	/// <summary>
	/// Tests the AnyResourceReferenceFinder in C#.
	/// </summary>
	[TestFixture]
	public sealed class AnyResourceReferenceFinderTests
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			TestHelper.InitializeParsers();
			TestHelper.InitializeResolvers();
		}
		
		// ********************************************************************************************************************************
		
		static AnyResourceReferenceFinder CreateFinder()
		{
			return new AnyResourceReferenceFinder();
		}
		
		[Test]
		public void NoReference()
		{
			const string Code = @"class A {
	void B() {
		DoSomething();
	}
}";
			AnyResourceReferenceFinder finder = CreateFinder();
			Assert.AreEqual(-1, finder.GetNextPossibleOffset("a.cs", Code, -1), "Incorrect offset.");
		}
		
		[Test]
		public void OneReference()
		{
			const string Code = @"class A {
	void B() {
		resMgr.GetString(""TestKey"");
	}
}";
			AnyResourceReferenceFinder finder = CreateFinder();
			Assert.AreEqual(33, finder.GetNextPossibleOffset("a.cs", Code, -1), "Incorrect offset.");
			Assert.AreEqual(-1, finder.GetNextPossibleOffset("a.cs", Code, 33), "Incorrect offset.");
		}
		
		[Test]
		public void TwoReferences()
		{
			const string Code = @"class A {
	void B() {
		resMgr.GetString(""TestKey"");
		resMgr[""TestKey""];
	}
}";
			AnyResourceReferenceFinder finder = CreateFinder();
			Assert.AreEqual(33, finder.GetNextPossibleOffset("a.cs", Code, -1), "Incorrect offset.");
			Assert.AreEqual(64, finder.GetNextPossibleOffset("a.cs", Code, 33), "Incorrect offset.");
			Assert.AreEqual(-1, finder.GetNextPossibleOffset("a.cs", Code, 64), "Incorrect offset.");
		}
		
		[Test]
		public void FourReferencesMixed()
		{
			const string Code = @"class A {
	void B() {
		resMgr.GetString(""TestKey"");
		X(""${res:TestKey2}"");
		resMgr[""TestKey""];
		X(""${res:TestKey3}"");
	}
}";
			AnyResourceReferenceFinder finder = CreateFinder();
			Assert.AreEqual(33, finder.GetNextPossibleOffset("a.cs", Code, -1), "Incorrect offset.");
			Assert.AreEqual(61, finder.GetNextPossibleOffset("a.cs", Code, 33), "Incorrect offset.");
			Assert.AreEqual(89, finder.GetNextPossibleOffset("a.cs", Code, 61), "Incorrect offset.");
			Assert.AreEqual(108, finder.GetNextPossibleOffset("a.cs", Code, 89), "Incorrect offset.");
			Assert.AreEqual(-1, finder.GetNextPossibleOffset("a.cs", Code, 108), "Incorrect offset.");
		}
		
		// ********************************************************************************************************************************
		
		[Test]
		public void ResultMatch()
		{
			AnyResourceReferenceFinder finder = CreateFinder();
			Assert.IsTrue(finder.IsReferenceToResource(new ResourceResolveResult(null, null, null, new ResourceSetReference("SomeResources", "C:\\SomeResources.resx"), "SomeKey")));
		}
	}
}
