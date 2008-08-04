// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Justin Dearing" email="zippy1981@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Description of WriteableSolutionEvaluator.
	/// </summary>
	public class WriteableSolutionConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			Solution solution = ProjectService.OpenSolution;
			return (solution != null && !solution.ReadOnly);
		}
	}
}
