// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Xml;


using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public class ProjectActiveAuswerter : IAuswerter
	{
		public bool IsValid(object caller, Condition condition)
		{
			string activeproject = condition.Properties["activeproject"];
				
			IProject project = ProjectService.CurrentProject;
			if (activeproject == "*") {
				return project != null;
			}
			
			return project != null && project.Language == activeproject;
		}
	}

}
