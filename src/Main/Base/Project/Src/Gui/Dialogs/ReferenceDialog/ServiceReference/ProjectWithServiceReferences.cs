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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ProjectWithServiceReferences : IProjectWithServiceReferences
	{
		IProject project;
		string serviceReferencesFolder;
		
		public static readonly string DefaultServiceReferencesFolderName = "Service References";
		
		public ProjectWithServiceReferences(IProject project)
		{
			this.project = project;
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
		
		public string Language {
			get { return project.Language; }
		}
		
		public string RootNamespace {
			get {
				if (project.RootNamespace != null) {
					return project.RootNamespace;
				}
				return String.Empty;
			}
		}
		
		public ServiceReferenceFileName GetServiceReferenceFileName(string serviceReferenceName)
		{
			return new ServiceReferenceFileName(ServiceReferencesFolder, serviceReferenceName);
		}
		
		public ServiceReferenceMapFileName GetServiceReferenceMapFileName(string serviceReferenceName)
		{
			return new ServiceReferenceMapFileName(ServiceReferencesFolder, serviceReferenceName);
		}
		
		public void AddServiceReferenceProxyFile(ServiceReferenceFileName fileName)
		{
			AddServiceReferenceFileToProject(fileName);
			AddServiceReferencesItemToProject();
			AddServiceReferenceItemToProject(fileName);
		}
		
		void AddServiceReferenceFileToProject(ServiceReferenceFileName fileName)
		{
			var projectItem = new FileProjectItem(project, ItemType.Compile);
			projectItem.FileName = FileName.Create(fileName.Path);
			projectItem.DependentUpon = "Reference.svcmap";
			AddProjectItemToProject(projectItem);
		}
		
		void AddProjectItemToProject(ProjectItem item)
		{
			ProjectService.AddProjectItem(project, item);
		}
		
		void AddServiceReferencesItemToProject()
		{
			if (IsServiceReferencesItemMissingFromProject()) {
				var projectItem = new ServiceReferencesProjectItem(project);
				projectItem.Include = "Service References";
				AddProjectItemToProject(projectItem);
			}
		}
		
		bool IsServiceReferencesItemMissingFromProject()
		{
			return project.GetItemsOfType(ItemType.ServiceReferences).Count() == 0;
		}
		
		void AddServiceReferenceItemToProject(ServiceReferenceFileName fileName)
		{
			var projectItem = new ServiceReferenceProjectItem(project);
			projectItem.Include = @"Service References\" + fileName.ServiceName;
			AddProjectItemToProject(projectItem);
		}
		
		public void Save()
		{
			project.Save();
		}
		
		public void AddServiceReferenceMapFile(ServiceReferenceMapFileName fileName)
		{
			var projectItem = new ServiceReferenceMapFileProjectItem(project, fileName.Path);
			AddProjectItemToProject(projectItem);
		}
		
		public void AddAssemblyReference(string referenceName)
		{
			if (!AssemblyReferenceExists(referenceName)) {
				var projectItem = new ReferenceProjectItem(project, referenceName);
				AddProjectItemToProject(projectItem);
			}
		}
		
		bool AssemblyReferenceExists(string referenceName)
		{
			return project
				.GetItemsOfType(ItemType.Reference)
				.Any(item => IsAssemblyReferenceMatch((ReferenceProjectItem)item, referenceName));
		}
		
		bool IsAssemblyReferenceMatch(ReferenceProjectItem item, string referenceName)
		{
			return IsMatchIgnoringCase(item.AssemblyName.ShortName, referenceName);
		}
		
		static bool IsMatchIgnoringCase(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
		
		public bool HasAppConfigFile()
		{
			return GetAppConfigFile() != null;
		}
		
		ProjectItem GetAppConfigFile()
		{
			return project.Items.SingleOrDefault(item => IsAppConfigFile(item));
		}
		
		bool IsAppConfigFile(ProjectItem item)
		{
			string fileNameWithoutPath = Path.GetFileName(item.FileName);
			return String.Equals(fileNameWithoutPath, "app.config", StringComparison.OrdinalIgnoreCase);
		}
		
		public string GetAppConfigFileName()
		{
			ProjectItem item = GetAppConfigFile();
			if (item != null) {
				return item.FileName;
			}
			return GetDefaultAppConfigFileName();
		}
		
		public void AddAppConfigFile()
		{
			var item = new FileProjectItem(project, ItemType.None);
			item.FileName = GetDefaultAppConfigFileName();
			AddProjectItemToProject(item);
		}
		
		FileName GetDefaultAppConfigFileName()
		{
			return project.Directory.CombineFile("app.config");
		}
		
		public IEnumerable<ReferenceProjectItem> GetReferences()
		{
			return project.GetItemsOfType(ItemType.Reference).OfType<ReferenceProjectItem>();
		}
	}
}
