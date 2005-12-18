// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// Determines whether #develop is currently running NAnt.
	/// </summary>
	public class IsNAntRunningCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return NAntRunnerSingleton.Runner.IsRunning;
		}
	}
}
