// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
