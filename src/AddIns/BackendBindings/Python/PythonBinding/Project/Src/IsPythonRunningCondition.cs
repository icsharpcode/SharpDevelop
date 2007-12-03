// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Indicates whether the python console is running.
	/// </summary>
	public class IsPythonRunningCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return RunPythonCommand.IsRunning;
		}
	}
}
