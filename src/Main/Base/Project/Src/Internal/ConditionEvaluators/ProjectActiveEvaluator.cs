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

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests if any project is active or if a project of a specific language is active.
	/// Can also be used to test the type of the project passed as caller to the condition
	/// - when a project is passed as caller, the language of that project is tested; otherwise
	/// the language of the active project is tested.
	/// </summary>
	/// <attribute name="activeproject">
	/// The language name the project should have.
	/// "*" to test if any project is active.
	/// </attribute>
	/// <example title="Test if any project is active">
	/// &lt;Condition name = "ProjectActive" activeproject="*"&gt;
	/// </example>
	/// <example title="Test if a C# project is active">
	/// &lt;Condition name = "ProjectActive" activeproject="C#"&gt;
	/// </example>
	public class ProjectActiveConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string activeproject = condition.Properties["activeproject"];
			
			IProject project = (caller as IProject) ?? ProjectService.CurrentProject;
			if (activeproject == "*") {
				return project != null;
			}
			
			return project != null && project.Language == activeproject;
		}
	}
}
