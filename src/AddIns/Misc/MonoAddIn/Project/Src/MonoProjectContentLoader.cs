// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.Build.Tasks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using System;
using System.IO;
using System.Reflection;

namespace ICSharpCode.MonoAddIn
{
	/// <summary>
	/// Adds Mono specific GAC assembly references to the project content registry.
	/// </summary>
	public class MonoProjectContentLoader
	{
		string fileName = String.Empty;
		string include = String.Empty;
		
		MonoProjectContentLoader(string fileName, string include)
		{
			this.fileName = fileName;
			this.include = include;
		}
		
		public static bool IsMonoProject(MSBuildProject project)
		{
			if (project != null) {
				PropertyStorageLocations location;
				foreach (string configuration in project.GetConfigurationNames()) {
					foreach (string platform in project.GetPlatformNames()) {
						string propertyValue = project.GetProperty(configuration, platform, "TargetFrameworkVersion", String.Empty, out location);
						if (propertyValue != null && propertyValue.StartsWith("Mono", StringComparison.InvariantCultureIgnoreCase)) {
							return true;
						}
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Adds the Mono Gac reference to the Dom Cache.
		/// </summary>
		/// <param name="name">The full name of the Gac assembly.</param>
		public static void AddGacReference(string name)
		{
			try {
				MonoAssemblyName assemblyName = MonoGlobalAssemblyCache.FindAssemblyName(name);
				if (assemblyName != null) {
					CreateMonoProjectContent(assemblyName.FileName, name);
				}
			} catch (Exception ex) {
				LoggingService.Debug(ex.ToString());
			}
		}
		
		/// <summary>
		/// Creates any missing Dom cache items for the project.
		/// </summary>
		public static void CreateMonoProjectContent(IProject project)
		{
			foreach (ProjectItem item in project.Items) {
				ReferenceProjectItem referenceItem = item as ReferenceProjectItem;
				if (item.ItemType == ItemType.Reference) {
					CreateMonoProjectContent(item.FileName, item.Include);
				}
			}
		}
		
		static void CreateMonoProjectContent(string fileName, string include)
		{
			MonoProjectContentLoader loader = new MonoProjectContentLoader(fileName, include);
			ProjectContentRegistry.RunLocked(loader.CreateMonoProjectContent);
		}
		
		static bool IsProjectContentAdded(string fileName, string include)
		{
			AssemblyName assemblyName = new AssemblyName(include);
			if (assemblyName == null) {
				return true;
			}
			
			if (ProjectContentRegistry.GetExistingProjectContent(assemblyName) != null) {
				return true;
			}
			
			ReflectionProjectContent pc = DomPersistence.LoadProjectContentByAssemblyName(fileName);
			if (pc != null) {
				return true;
			}
			
			pc = DomPersistence.LoadProjectContentByAssemblyName(include);
			if (pc != null) {
				return true;
			}
			
			return false;	
		}
		
		void CreateMonoProjectContent()
		{
			if (IsProjectContentAdded(fileName, include)) {
				return;
			}
			
			AppDomainSetup setup = new AppDomainSetup();
			setup.DisallowCodeDownload = true;
			setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
			AppDomain domain = AppDomain.CreateDomain("MonoAssemblyLoadingDomain", AppDomain.CurrentDomain.Evidence, setup);
			try {
				object o = domain.CreateInstanceAndUnwrap(typeof(MonoReflectionLoader).Assembly.FullName, typeof(MonoReflectionLoader).FullName);
				MonoReflectionLoader loader = (MonoReflectionLoader)o;
				string database = loader.LoadAndCreateDatabase(fileName, include);
				if (database != null) {
					ReflectionProjectContent content = DomPersistence.LoadProjectContent(database);
					if (content != null) {
						DomPersistence.SaveProjectContent(content);
					}
				}
			} catch (Exception ex) {
				LoggingService.Debug("Error loading " + include + " from " + fileName + ": " + ex.ToString());
			} finally {
				AppDomain.Unload(domain);
			}
		}
	}
}
