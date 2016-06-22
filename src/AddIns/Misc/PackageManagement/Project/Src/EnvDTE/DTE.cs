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
