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
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsCommon.Get, "Project", DefaultParameterSetName = ParameterSetProjectsFilteredByName)]
	[OutputType(typeof(Project))]
	public class GetProjectCmdlet : PackageManagementCmdlet
	{
		const string ParameterSetAllProjects = "AllProjects";
		const string ParameterSetProjectsFilteredByName = "ProjectsFilteredByName";
		
		public GetProjectCmdlet()
			: this(
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public GetProjectCmdlet(
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
		}
		
		[Parameter(Mandatory = true, ParameterSetName = ParameterSetAllProjects)]
		public SwitchParameter All { get; set; }
		
		[Parameter(Position = 0, ParameterSetName = ParameterSetProjectsFilteredByName, ValueFromPipelineByPropertyName = true)]
		public string[] Name { get; set; }
		
		protected override void ProcessRecord()
		{
			ThrowErrorIfProjectNotOpen();
			
			if (All.IsPresent) {
				WriteAllProjectsToPipeline();
			} else if (Name != null) {
				WriteFilteredProjectsToPipeline();
			} else {
				WriteDefaultProjectToPipeline();
			}
		}
		
		void WriteAllProjectsToPipeline()
		{
			IEnumerable<Project> allProjects = GetAllProjects();
			WriteProjectsToPipeline(allProjects);
		}

		IEnumerable<Project> GetAllProjects()
		{
			var projects = new OpenProjects(ConsoleHost.Solution);
			return projects.GetAllProjects();
		}
		
		void WriteProjectsToPipeline(IEnumerable<Project> projects)
		{
			bool enumerateCollection = true;
			WriteObject(projects, enumerateCollection);
		}
		
		void WriteFilteredProjectsToPipeline()
		{
			IEnumerable<Project> projects = GetFilteredProjects();
			WriteProjectsToPipeline(projects);
		}
		
		IEnumerable<Project> GetFilteredProjects()
		{
			var projects = new OpenProjects(ConsoleHost.Solution);
			return projects.GetFilteredProjects(Name);
		}
		
		void WriteDefaultProjectToPipeline()
		{
			Project project = GetDefaultProject();
			WriteObject(project);
		}
		
		Project GetDefaultProject()
		{
			return new Project(DefaultProject);
		}
	}
}
