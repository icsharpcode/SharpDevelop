// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ProjectWithServiceReferences : IProjectWithServiceReferences
	{
		IProject project;
		string serviceReferencesFolder;
		
		public static readonly string DefaultServiceReferencesFolderName = "Service References";
		
		public ProjectWithServiceReferences(IProject project)
			: this(project, new ServiceReferenceCodeDomProvider(project))
		{
		}
		
		public ProjectWithServiceReferences(IProject project, ICodeDomProvider codeDomProvider)
		{
			this.project = project;
			this.CodeDomProvider = codeDomProvider;
		}
		
		public string ServiceReferencesFolder {
			get {
				if (serviceReferencesFolder == null) {
					GetServiceReferencesFolder();
				}
				return serviceReferencesFolder;
			}
		}
		
		void GetServiceReferencesFolder()
		{
			serviceReferencesFolder = Path.Combine(project.Directory, DefaultServiceReferencesFolderName);
		}
		
		public ICodeDomProvider CodeDomProvider { get; private set; }
		
		public string GetServiceReferenceFileName(string serviceReferenceName)
		{
			return Path.Combine(ServiceReferencesFolder, serviceReferenceName, "Reference.cs");
		}
		
		public void AddServiceReferenceProxyFile(string fileName)
		{
			AddServiceReferenceFileToProject(fileName);
			AddServiceReferencesItemToProject();
		}
		
		void AddServiceReferenceFileToProject(string fileName)
		{
			var projectItem = new FileProjectItem(project, ItemType.Compile);
			projectItem.FileName = fileName;
			AddProjectItemToProject(projectItem);
		}
		
		void AddProjectItemToProject(ProjectItem item)
		{
			ProjectService.AddProjectItem(project, item);
		}
		
		void AddServiceReferencesItemToProject()
		{
			var projectItem = new ServiceReferencesProjectItem(project);
			projectItem.Include = "Service References";
			AddProjectItemToProject(projectItem);
		}
		
		public void Save()
		{
			project.Save();
		}
	}
}
