// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Supplies a "Unit test" menu item if the class is a test fixture.
	/// </summary>
	public class TestableCondition : IConditionEvaluator
	{
		readonly ITestService testService;
		
		public TestableCondition()
		{
			this.testService = SD.GetRequiredService<ITestService>();
		}
		
		public TestableCondition(ITestService testService)
		{
			this.testService = testService;
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			return GetTests(testService.OpenSolution, caller).Any();
		}
		
		public static IEnumerable<ITest> GetTests(ITestSolution testSolution, object caller)
		{
			ITestTreeView testTreeView = caller as ITestTreeView;
			if (testTreeView != null) {
				return testTreeView.SelectedTests;
			}
			if (testSolution != null) {
				IEntity entity = ResolveResultMenuCommand.GetSymbol(caller) as IEntity;
				return testSolution.GetTestsForEntity(entity);
			}
			return Enumerable.Empty<ITest>();
		}
	}
}
