// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;


using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
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
