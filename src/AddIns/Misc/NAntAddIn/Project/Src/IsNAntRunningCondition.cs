// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
