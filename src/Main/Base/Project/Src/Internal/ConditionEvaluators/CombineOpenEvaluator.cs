// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests if a solution is open.
	/// </summary>
	/// <example title="Test if a solution is opened">
	/// &lt;Condition name = "SolutionOpen"&gt;
	/// </example>
	public class SolutionOpenConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return (ProjectService.OpenSolution != null);
		}
	}
}
