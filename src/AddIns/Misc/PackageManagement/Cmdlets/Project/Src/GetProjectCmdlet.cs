// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
