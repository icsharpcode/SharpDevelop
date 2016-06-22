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
	/// Tests the SpecificResourceReferenceFinder in C#.
	/// </summary>
	[TestFixture]
	public sealed class SpecificResourceReferenceFinderTests
	{
		[TestFixtureSetUpAttribute]
		public void FixtureSetUp()
		{
			TestHelper.InitializeParsers();
			TestHelper.InitializeResolvers();
		}
		
		// ********************************************************************************************************************************
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullResourceFileName()
		{
			SpecificResourceReferenceFinder finder = new SpecificResourceReferenceFinder(null, "TestKey");
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullKey()
		{
			SpecificResourceReferenceFinder finder = new SpecificResourceReferenceFinder("C:\\TestResources.resx", null);
		}
		
		// ********************************************************************************************************************************
		
		static SpecificResourceReferenceFinder CreateFinder()
		{
			SpecificResourceReferenceFinder finder = new SpecificResourceReferenceFinder("C:\\TestResources.resx", "TestKey");
			Assert.AreEqual(finder.ResourceFileName, "C:\\TestResources.resx");
			Assert.AreEqual(finder.Key, "TestKey");
			return finder;
		}
		
		[Test]
		public void NoReference()
		{
			const string Code = @"class A {
	void B() {
		DoSomething();
	}
}";
			SpecificResourceReferenceFinder finder = CreateFinder();
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
			SpecificResourceReferenceFinder finder = CreateFinder();
			Assert.AreEqual(44, finder.GetNextPossibleOffset("a.cs", Code, -1), "Incorrect offset.");
			Assert.AreEqual(-1, finder.GetNextPossibleOffset("a.cs", Code, 44), "Incorrect offset.");
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
			SpecificResourceReferenceFinder finder = CreateFinder();
			Assert.AreEqual(44, finder.GetNextPossibleOffset("a.cs", Code, -1), "Incorrect offset.");
			Assert.AreEqual(66, finder.GetNextPossibleOffset("a.cs", Code, 44), "Incorrect offset.");
			Assert.AreEqual(-1, finder.GetNextPossibleOffset("a.cs", Code, 66), "Incorrect offset.");
		}
		
		// ********************************************************************************************************************************
		
		[Test]
		public void ResultNoMatchWrongFile()
		{
			SpecificResourceReferenceFinder finder = CreateFinder();
			Assert.IsFalse(finder.IsReferenceToResource(new ResourceResolveResult(null, null, null, new ResourceSetReference("SomeResources", "C:\\SomeResources.resx"), "TestKey")));
		}
		
		[Test]
		public void ResultNoMatchWrongKey()
		{
			SpecificResourceReferenceFinder finder = CreateFinder();
			Assert.IsFalse(finder.IsReferenceToResource(new ResourceResolveResult(null, null, null, new ResourceSetReference("TestResources", "C:\\TestResources.resx"), "SomeKey")));
		}
		
		[Test]
		public void ResultMatch()
		{
			SpecificResourceReferenceFinder finder = CreateFinder();
			Assert.IsTrue(finder.IsReferenceToResource(new ResourceResolveResult(null, null, null, new ResourceSetReference("TestResources", "C:\\TestResources.resx"), "TestKey")));
		}
	}
}
