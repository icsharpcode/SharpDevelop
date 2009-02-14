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
	/// Tests if the caller project is writable. If caller is not an IProject it tests
	/// Project.CurrentProject.
	/// </summary>
	public class WriteableProjectConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			IProject project = (caller as IProject) ?? ProjectService.CurrentProject;
			return project != null && !project.ReadOnly;
		}
	}
}
