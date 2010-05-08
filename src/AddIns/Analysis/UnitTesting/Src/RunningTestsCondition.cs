// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
