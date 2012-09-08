// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Determines whether #develop is currently running unit tests.
	/// </summary>
	public class RunningTestsCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return AbstractRunTestCommand.IsRunningTest;
		}
	}
}
