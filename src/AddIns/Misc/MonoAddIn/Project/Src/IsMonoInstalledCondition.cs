// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Build.Tasks;
using ICSharpCode.Core;

namespace ICSharpCode.MonoAddIn
{
	/// <summary>
	/// Determines whether Mono is installed.
	/// </summary>
	public class IsMonoInstalledCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return MonoToolLocationHelper.IsMonoInstalled;
		}
	}
}
