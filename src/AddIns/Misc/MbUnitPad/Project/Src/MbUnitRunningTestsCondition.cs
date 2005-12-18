// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;

namespace ICSharpCode.MbUnitPad
{
	/// <summary>
	/// Determines whether #develop is currently running MbUnit tests.
	/// </summary>
	public class MbUnitRunningTestsCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			MbUnitPadContent pad = caller as MbUnitPadContent;
			if (pad != null) {
				return pad.IsRunningTests;
			}
			return false;
		}
	}
}
