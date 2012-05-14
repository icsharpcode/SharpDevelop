// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class DTE : MarshalByRefObject
	{
		IPackageManagementProjectService projectService;
		IPackageManagementFileService fileService;
		
		public DTE()
			: this(new PackageManagementProjectService(), new PackageManagementFileService())
		{
		}
		
		public DTE(
			IPackageManagementProjectService projectService,
			IPackageManagementFileService fileService)
		{
			this.projectService = projectService;
			this.fileService = fileService;
			
			ItemOperations = new ItemOperations(fileService);
		}
		
		public string Version {
			get { return "10.0"; }
		}
		
		public Solution Solution {
			get {
				if (IsSolutionOpen) {
					return new Solution(projectService);
				}
				return null;
			}
		}
		
		bool IsSolutionOpen {
			get { return projectService.OpenSolution != null; }
		}
		
		public ItemOperations ItemOperations { get; private set; }
		
		public Properties Properties(string category, string page)
		{
			var properties = new DTEProperties();
			return properties.GetProperties(category, page);
		}
		
		public object ActiveSolutionProjects {
			get {
				if (IsSolutionOpen) {
					return Solution.Projects.ToArray();
				}
				return new Project[0];
			}
		}
		
		public SourceControl SourceControl {
			get { return null; }
		}
	}
}
