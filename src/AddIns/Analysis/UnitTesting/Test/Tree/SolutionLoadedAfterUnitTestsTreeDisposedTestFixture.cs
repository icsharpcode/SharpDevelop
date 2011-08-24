// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class SolutionLoadedAfterUnitTestsTreeDisposedTestFixture
	{
		DerivedUnitTestsPad pad;
		
		[SetUp]
		public void Init()
		{
			pad = new DerivedUnitTestsPad();
			pad.Dispose();
		}
		
		[Test]
		public void SolutionLoadedAfterTreeDisposedDoesNotThrowNullReferenceException()
		{
			Solution solution = new Solution(new MockProjectChangeWatcher());
			Assert.DoesNotThrow(delegate { pad.CallSolutionLoaded(solution); });
		}
	}
}
