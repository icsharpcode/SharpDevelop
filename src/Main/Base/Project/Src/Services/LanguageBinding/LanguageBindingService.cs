// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public static class LanguageBindingService
	{
		static IList<LanguageBindingDescriptor> bindings;
		
		static LanguageBindingService()
		{
			bindings = AddInTree.BuildItems<LanguageBindingDescriptor>("/SharpDevelop/Workbench/LanguageBindings", null, false);
		}
		
		public static string GetProjectFileExtension(string languageName)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerLanguageName(languageName);
			return descriptor == null ? null : descriptor.ProjectFileExtension;
		}
		
		public static ILanguageBinding GetBindingPerLanguageName(string languagename)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerLanguageName(languagename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static ILanguageBinding GetBindingCodePerFileName(string filename)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerCodeFileName(filename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static ILanguageBinding GetBindingPerProjectFile(string filename)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerProjectFile(filename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static LanguageBindingDescriptor GetCodonPerLanguageName(string languagename)
		{
			foreach (LanguageBindingDescriptor binding in bindings) {
				if (binding.Binding.Language == languagename) {
					return binding;
				}
			}
			return null;
		}
		
		public static LanguageBindingDescriptor GetCodonPerCodeFileName(string filename)
		{
			string extension = Path.GetExtension(filename).ToLowerInvariant();
			foreach (LanguageBindingDescriptor binding in bindings) {
				if (Array.IndexOf(binding.CodeFileExtensions, extension) >= 0) {
					return binding;
				}
			}
			return null;
		}
		
		public static LanguageBindingDescriptor GetCodonPerProjectFile(string fileName)
		{
			string ext = Path.GetExtension(fileName).ToUpperInvariant();
			foreach (LanguageBindingDescriptor binding in bindings) {
				if (binding.ProjectFileExtension.ToUpperInvariant() == ext) {
					return binding;
				}
			}
			return null;
		}
		
		public static IProject LoadProject(string location, string title)
		{
			return LoadProject(location, title, "{" + new Guid().ToString() + "}");
		}
		
		public static IProject LoadProject(string location, string title, string projectTypeGuid)
		{
			IProject newProject;
			if (!File.Exists(location)) {
				newProject = new MissingProject(location, title);
				newProject.TypeGuid = projectTypeGuid;
			} else {
				ILanguageBinding binding = LanguageBindingService.GetBindingPerProjectFile(location);
				if (binding != null) {
					try {
						newProject = binding.LoadProject(location, title);
					} catch (XmlException ex) {
						MessageService.ShowError("Error loading " + location + ":\n" + ex.Message);
						newProject = new UnknownProject(location, title);
						newProject.TypeGuid = projectTypeGuid;
					} catch (UnauthorizedAccessException ex) {
						MessageService.ShowError("Error loading " + location + ":\n" + ex.Message);
						newProject = new UnknownProject(location, title);
						newProject.TypeGuid = projectTypeGuid;
					}
				} else {
					newProject = new UnknownProject(location, title);
					newProject.TypeGuid = projectTypeGuid;
				}
			}
			return newProject;
		}
	}
}
