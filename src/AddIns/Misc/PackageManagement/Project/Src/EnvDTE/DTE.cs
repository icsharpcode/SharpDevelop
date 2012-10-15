// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Shell;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class DTE : MarshalByRefObject, global::EnvDTE.DTE, IServiceProvider
	{
		IPackageManagementProjectService projectService;
		IPackageManagementFileService fileService;
		Solution solution;
		
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
		
		public global::EnvDTE.Solution Solution {
			get {
				if (IsSolutionOpen) {
					CreateSolution();
					return solution;
				}
				return null;
			}
		}
		
		bool IsSolutionOpen {
			get { return projectService.OpenSolution != null; }
		}
		
		void CreateSolution()
		{
			if (!IsOpenSolutionAlreadyCreated()) {
				solution = new Solution(projectService);
			}
		}
		
		bool IsOpenSolutionAlreadyCreated()
		{
			if (solution != null) {
				return solution.IsOpen;
			}
			return false;
		}
		
		public global::EnvDTE.ItemOperations ItemOperations { get; private set; }
		
		public global::EnvDTE.Properties Properties(string category, string page)
		{
			var properties = new DTEProperties();
			return properties.GetProperties(category, page);
		}
		
		public object ActiveSolutionProjects {
			get {
				if (IsSolutionOpen) {
					return Solution.Projects.OfType<Project>().ToArray();
				}
				return new Project[0];
			}
		}
		
		public global::EnvDTE.SourceControl SourceControl {
			get { return null; }
		}
		
		/// <summary>
		/// HACK - EnvDTE.DTE actually implements Microsoft.VisualStudio.OLE.Interop.IServiceProvider
		/// which is COM specific and has a QueryInterface method.
		/// </summary>
		object IServiceProvider.GetService(Type serviceType)
		{
			return Package.GetGlobalService(serviceType);
		}
	}
}
