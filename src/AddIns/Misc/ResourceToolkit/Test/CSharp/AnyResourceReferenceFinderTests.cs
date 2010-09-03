// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
