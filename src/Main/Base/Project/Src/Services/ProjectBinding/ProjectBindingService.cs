// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public static class ProjectBindingService
	{
		static IList<ProjectBindingDescriptor> bindings;
		
		static ProjectBindingService()
		{
			bindings = AddInTree.BuildItems<ProjectBindingDescriptor>("/SharpDevelop/Workbench/ProjectBindings", null, false);
		}
		
		/// <summary>
		/// Overwrites the list of used bindings. Used for unit tests.
		/// </summary>
		public static void SetBindings(IList<ProjectBindingDescriptor> bindings)
		{
			ProjectBindingService.bindings = bindings;
		}
		
		public static string GetProjectFileExtension(string languageName)
		{
			ProjectBindingDescriptor descriptor = GetCodonPerLanguageName(languageName);
			return descriptor == null ? null : descriptor.ProjectFileExtension;
		}
		
		public static IProjectBinding GetBindingPerLanguageName(string languagename)
		{
			ProjectBindingDescriptor descriptor = GetCodonPerLanguageName(languagename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static IProjectBinding GetBindingCodePerFileName(string filename)
		{
			ProjectBindingDescriptor descriptor = GetCodonPerCodeFileName(filename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static IProjectBinding GetBindingPerProjectFile(string filename)
		{
			ProjectBindingDescriptor descriptor = GetCodonPerProjectFile(filename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static ProjectBindingDescriptor GetCodonPerLanguageName(string languagename)
		{
			foreach (ProjectBindingDescriptor binding in bindings) {
				if (binding.Language == languagename) {
					return binding;
				}
			}
			return null;
		}
		
		public static ProjectBindingDescriptor GetCodonPerCodeFileName(string filename)
		{
			string extension = Path.GetExtension(filename).ToLowerInvariant();
			foreach (ProjectBindingDescriptor binding in bindings) {
				if (Array.IndexOf(binding.CodeFileExtensions, extension) >= 0) {
					return binding;
				}
			}
			return null;
		}
		
		public static ProjectBindingDescriptor GetCodonPerProjectFile(string fileName)
		{
			string ext = Path.GetExtension(fileName).ToUpperInvariant();
			foreach (ProjectBindingDescriptor binding in bindings) {
				if (binding.ProjectFileExtension.ToUpperInvariant() == ext) {
					return binding;
				}
			}
			return null;
		}
		
		public static IProject LoadProject(ProjectLoadInformation loadInformation)
		{
			if (loadInformation == null)
				throw new ArgumentNullException("loadInformation");
			
			string location = FileUtility.NormalizePath(loadInformation.FileName);
			string title = loadInformation.ProjectName;
			IProgressMonitor progressMonitor = loadInformation.ProgressMonitor;
			
			progressMonitor.CancellationToken.ThrowIfCancellationRequested();
			
			IProjectBinding binding = ProjectBindingService.GetBindingPerProjectFile(location);
			IProject newProject;
			if (!(binding != null && binding.HandlingMissingProject) && !File.Exists(location)) {
				newProject = new MissingProject(location, title);
				newProject.TypeGuid = loadInformation.TypeGuid;
			} else {
				if (binding != null) {
					try {
						newProject = binding.LoadProject(loadInformation);
					} catch (ProjectLoadException ex) {
						LoggingService.Warn("Project load error", ex);
						progressMonitor.ShowingDialog = true;
						newProject = new UnknownProject(location, title, ex.Message, true);
						newProject.TypeGuid = loadInformation.TypeGuid;
						progressMonitor.ShowingDialog = false;
					} catch (UnauthorizedAccessException ex) {
						LoggingService.Warn("Project load error", ex);
						progressMonitor.ShowingDialog = true;
						newProject = new UnknownProject(location, title, ex.Message, true);
						newProject.TypeGuid = loadInformation.TypeGuid;
						progressMonitor.ShowingDialog = false;
					}
				} else {
					string ext = Path.GetExtension(location);
					if (".proj".Equals(ext, StringComparison.OrdinalIgnoreCase)
					    || ".build".Equals(ext, StringComparison.OrdinalIgnoreCase))
					{
						newProject = new MSBuildFileProject(location, title);
						newProject.TypeGuid = loadInformation.TypeGuid;
					} else {
						newProject = new UnknownProject(location, title);
						newProject.TypeGuid = loadInformation.TypeGuid;
					}
				}
			}
			return newProject;
		}
	}
}
