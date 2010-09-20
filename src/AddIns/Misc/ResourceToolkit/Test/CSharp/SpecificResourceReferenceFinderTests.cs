// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
