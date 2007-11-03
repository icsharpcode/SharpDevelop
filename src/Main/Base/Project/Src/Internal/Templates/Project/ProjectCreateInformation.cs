// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class holds all information the language binding need to create
	/// a predefined project for their language, if no project template for a
	/// specific language is avaiable, the language binding shouldn't care about
	/// this stuff.
	/// </summary>
	public class ProjectCreateInformation
	{
		internal List<IProject> createdProjects = new List<IProject>();
		
		public ReadOnlyCollection<IProject> CreatedProjects {
			get { return createdProjects.AsReadOnly(); }
		}
		
		public string OutputProjectFileName { get; set; }
		public string ProjectName { get; set; }
		public string SolutionName { get; set; }
		public string RootNamespace { get; set; }
		public string SolutionPath { get; set; }
		public string ProjectBasePath { get; set; }
		public string TargetFramework { get; set; }
		public Solution Solution { get; set; }
	}
}
