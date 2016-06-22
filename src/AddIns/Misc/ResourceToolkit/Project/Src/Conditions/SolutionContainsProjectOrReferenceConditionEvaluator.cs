// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
