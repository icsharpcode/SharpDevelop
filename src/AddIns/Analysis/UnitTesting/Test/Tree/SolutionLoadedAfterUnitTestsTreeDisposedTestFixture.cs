// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			Solution solution = new Solution();
			Assert.DoesNotThrow(delegate { pad.CallSolutionLoaded(solution); });
		}
	}
}
