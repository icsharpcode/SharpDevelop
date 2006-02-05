// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Determines whether #develop is currently running unit tests.
	/// </summary>
	public class RunningTestsCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			// initializing PadContent might call IsValid
			if (caller is PadContent)
				return ((PadContent)caller).IsRunningTests;
			return PadContent.Instance.IsRunningTests;
		}
	}
	
	/// <summary>
	/// Determines whether starting unit tests by calling a command derived from
	/// AbstractRunTestCommand is possible.
	/// </summary>
	public class UnitCommonTestCommandsEnabledCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (PadContent.Instance.IsRunningTests)
				return false;
			TestTreeView view = caller as TestTreeView;
			if (view != null) {
				return view.SelectedProject != null;
			} else {
				return true;
			}
		}
	}
}
