// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace Hornung.ResourceToolkit.Conditions
{
	/// <summary>
	/// Checks whether a solution is open and contains a either a project or a
	/// reference with the specified name. 
	/// </summary>
	/// <attribute name="itemName">
	/// The name of the project or reference to find.
	/// </attribute>
	/// <example title="Check whether the open solution uses ICSharpCode.Core">
	/// &lt;Condition name = "SolutionContainsProjectOrReference" itemName = "ICSharpCode.Core"&gt;
	/// </example>
	public class SolutionContainsProjectOrReferenceConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (ProjectService.OpenSolution == null) {
				return false;
			}
			
			foreach (IProject p in ProjectService.OpenSolution.Projects) {
				
				// Check project name
				if (p.Name.Equals(condition.Properties["itemName"], StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
				
				// Check references
				foreach (ProjectItem pi in p.Items) {
					ReferenceProjectItem rpi = pi as ReferenceProjectItem;
					if (rpi != null) {
						if (rpi.Name.Equals(condition.Properties["itemName"], StringComparison.OrdinalIgnoreCase)) {
							return true;
						}
					}
				}
				
			}
			
			return false;
		}
		
		/// <summary>
		/// Initalizes a new instance of the <see cref="SolutionContainsProjectOrReferenceConditionEvaluator"/> class.
		/// </summary>
		public SolutionContainsProjectOrReferenceConditionEvaluator()
		{
		}
	}
}
