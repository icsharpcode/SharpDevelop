// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Base interface for projects.
	/// Thread-safe members lock on the SyncRoot. Non-thread-safe members may only be called from the main thread.
	/// 
	/// When you implement IProject, you should also implement IProjectItemListProvider and IProjectAllowChangeConfigurations
	/// </summary>
	public interface IProject
		: IBuildable, ISolutionFolder, IDisposable, IMementoCapable
	{
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is guaranteed not to change - adding new items or removing existing items
		/// will create a new collection.
		/// </summary>
		ReadOnlyCollection<ProjectItem> Items {
			get;
		}
		
		/// <summary>
		/// Gets all items in the project that have the specified item type.
		/// This member is thread-safe.
		/// </summary>
		IEnumerable<ProjectItem> GetItemsOfType(ItemType type);
		
		/// <summary>
		/// Gets the default item type the specified file should have.
		/// </summary>
		/// <param name="fileName">The full path to the file to determine the item type for</param>
		ItemType GetDefaultItemType(string fileName);
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		ICollection<ItemType> AvailableFileItemTypes {
			get;
		}
		
		/// <summary>
		/// Gets a list of project sections stored in the solution file for this project.
		/// </summary>
		List<ProjectSection> ProjectSections {
			get;
		}
		
		/// <summary>
		/// Gets the language properties used for this project. This member is thread-safe.
		/// </summary>
		ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get;
		}
		
		/// <summary>
		/// Gets the ambience used for the project. This member is thread-safe.
		/// Because the IAmbience interface is not thread-safe, every call returns a new instance.
		/// </summary>
		ICSharpCode.SharpDevelop.Dom.IAmbience GetAmbience();
		
		/// <summary>
		/// Gets the name of the project file.
		/// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		string FileName {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the name of the project.
		/// </summary>
		/// <remarks>
		/// Name already exists in ISolutionFolder, it's repeated here to prevent
		/// the ambiguity with IBuildable.Name.
		/// </remarks>
		new string Name {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the directory of the project file.
		/// This is equivalent to Path.GetDirectoryName(project.FileName);
		/// (Example: @"D:\Serralongue\SharpDevelop\samples\CustomPad")
		/// 
		/// This member is thread-safe.
		/// </summary>
		string Directory {
			get;
		}
		
		/// <summary>
		/// <para>
		/// True if the project is readonly. For project based files this means
		/// the project file has the readonly attribute set. For solution folder
		/// based projects this means that the sln file containing the project
		/// has the readonly attribute set.
		/// </para>
		/// <para>This member is thread-safe.</para>
		/// </summary>
		bool ReadOnly {
			get;
		}
		
		#region MSBuild properties used inside SharpDevelop base
		/// <summary>
		/// Gets/Sets the assembly name of the assembly created when building this project.
		/// Equivalent to MSBuild property "AssemblyName".
		/// </summary>
		string AssemblyName {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the root namespace of the project.
		/// </summary>
		string RootNamespace {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the full path of the output assembly.
		/// Returns null when the project does not output any assembly.
		/// </summary>
		string OutputAssemblyFullPath {
			get;
		}
		
		/// <summary>
		/// Gets the name of the language binding used for the project.
		/// </summary>
		string Language {
			get;
		}
		
		/// <summary>
		/// Gets the name of the directory being the "Properties" folder of the application,
		/// relative to the project directory.
		/// This folder gets a node type in the project browser.
		/// Equivalent to MSBuild property "AppDesignerFolder".
		/// </summary>
		string AppDesignerFolder {
			get;
		}
		#endregion
		
		#region Configuration / Platform management
		/// <summary>
		/// Gets/Sets the active configuration. MSBuild properties
		/// </summary>
		string ActiveConfiguration {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the active platform.
		/// </summary>
		string ActivePlatform {
			get;
			set;
		}
		/// <summary>
		/// Gets the list of available configuration names.
		/// </summary>
		ICollection<string> ConfigurationNames { get; }
		
		/// <summary>
		/// Gets the list of available platform names.
		/// </summary>
		ICollection<string> PlatformNames { get; }
		#endregion
		
		/// <summary>
		/// Saves the project using it's current file name.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Returns true, if a specific file (given by it's name) is inside this project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		bool IsFileInProject(string fileName);
		
		/// <summary>
		/// Returns the project item for a specific file; or null if the file is not found in the project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		FileProjectItem FindFile(string fileName);
		
		/// <summary>
		/// Gets if the project can be started.
		/// </summary>
		bool IsStartable { get; }
		
		/// <summary>
		/// Starts the project.
		/// </summary>
		/// <param name="withDebugging">True, if a debugger should be used for the project.</param>
		void Start(bool withDebugging);
		
		/// <summary>
		/// Creates a new project content for this project.
		/// This method should only be called by ParserService.LoadSolutionProjectsInternal()!
		/// Return null if you don't want to create any project content.
		/// </summary>
		ParseProjectContent CreateProjectContent();
		
		/// <summary>
		/// Creates a new ProjectItem for the passed MSBuild item.
		/// </summary>
		ProjectItem CreateProjectItem(Microsoft.Build.BuildEngine.BuildItem item);
		
		/// <summary>
		/// Gets the minimum version the solution must have to support this project type.
		/// </summary>
		int MinimumSolutionVersion { get; }
		
		/// <summary>
		/// Retrieve the fully qualified assembly names and file location of referenced assemblies.
		/// This method is thread safe.
		/// </summary>
		void ResolveAssemblyReferences();
	}
	
	/// <summary>
	/// A project or solution.
	/// The IBuildable interface members are thread-safe.
	/// </summary>
	public interface IBuildable
	{
		/// <summary>
		/// Gets the list of projects on which this project depends.
		/// </summary>
		ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions);
		
		/// <summary>
		/// Starts building the project using the specified options.
		/// This member must be implemented thread-safe.
		/// </summary>
		void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink);
		
		/// <summary>
		/// Gets the name of the buildable item.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets the parent solution.
		/// </summary>
		Solution ParentSolution { get; }
	}
	
	// We cannot extend an existing interface in 3.x because we don't want to break addin compatibility.
	// TODO: merge this interface with IBuildable in SharpDevelop 4.0
	public interface IBuildable2 : IBuildable
	{
		/// <summary>
		/// Creates the project-specific build options.
		/// </summary>
		/// <param name="options">The global build options.</param>
		/// <param name="isRootBuildable">Specifies whether this project is the main buildable item
		/// (i.e. the ).</param>
		/// <returns>The project-specific build options.</returns>
		ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable);
	}
	
	/// <summary>
	/// Provides the IBuildable2 methods in IBuildable.
	/// For classes not implementing IBuildable2, a default implementation is used.
	/// </summary>
	public static class IBuildableExtension
	{
		public static ProjectBuildOptions CreateProjectBuildOptions(this IBuildable buildable, BuildOptions options, bool isRootBuildable)
		{
			IBuildable2 buildable2 = buildable as IBuildable2;
			if (buildable2 != null) {
				return buildable2.CreateProjectBuildOptions(options, isRootBuildable);
			}
			// start of default implementation
			var configMatchings = buildable.ParentSolution.GetActiveConfigurationsAndPlatformsForProjects(options.SolutionConfiguration, options.SolutionPlatform);
			ProjectBuildOptions projectOptions = new ProjectBuildOptions(isRootBuildable ? options.ProjectTarget : options.TargetForDependencies);
			// Find the project configuration, and build an XML string containing all configurations from the solution
			StringWriter solutionConfigurationXml = new StringWriter();
			using (XmlTextWriter solutionConfigurationWriter = new XmlTextWriter(solutionConfigurationXml)) {
				solutionConfigurationWriter.WriteStartElement("SolutionConfiguration", "");
				foreach (var matching in configMatchings) {
					if (matching.Project == buildable) {
						projectOptions.Configuration = matching.Configuration;
						projectOptions.Platform = matching.Platform;
					}
					
					solutionConfigurationWriter.WriteStartElement("ProjectConfiguration");
					solutionConfigurationWriter.WriteAttributeString("Project", matching.Project.IdGuid);
					solutionConfigurationWriter.WriteValue(matching.Configuration + "|" + MSBuildInternals.FixPlatformNameForProject(matching.Platform));
					solutionConfigurationWriter.WriteEndElement();
				}
				solutionConfigurationWriter.WriteEndElement();
			}
			
			// fall back to solution config if we don't find any entries for the project
			if (string.IsNullOrEmpty(projectOptions.Configuration))
				projectOptions.Configuration = options.SolutionConfiguration;
			if (string.IsNullOrEmpty(projectOptions.Platform))
				projectOptions.Platform = options.SolutionPlatform;
			
			// copy properties to project options
			options.GlobalAdditionalProperties.ForEach(projectOptions.Properties.Add);
			if (isRootBuildable) {
				foreach (var pair in options.ProjectAdditionalProperties) {
					projectOptions.Properties[pair.Key] = pair.Value;
				}
			}
			// Set property for solution configuration. This allows MSBuild to know the correct configuration for project references,
			// which is necessary to resolve the referenced project's OutputPath.
			projectOptions.Properties["CurrentSolutionConfigurationContents"] = solutionConfigurationXml.ToString();
			return projectOptions;
		}
	}
	
	/// <summary>
	/// Interface for adding and removing items from a project. Not part of the IProject
	/// interface because in nearly all cases, ProjectService.Add/RemoveProjectItem should
	/// be used instead!
	/// So IProject implementors should implement this interface, but only the SharpDevelop methods
	/// ProjectService.AddProjectItem and RemoveProjectItem may call the interface members.
	/// </summary>
	public interface IProjectItemListProvider
	{
		/// <summary>
		/// Gets a list of items in the project.
		/// </summary>
		ReadOnlyCollection<ProjectItem> Items {
			get;
		}
		
		/// <summary>
		/// Adds a new entry to the Items-collection
		/// </summary>
		void AddProjectItem(ProjectItem item);
		
		/// <summary>
		/// Removes an entry from the Items-collection
		/// </summary>
		bool RemoveProjectItem(ProjectItem item);
	}
	
	/// <summary>
	/// Interface for changing project or solution configuration.
	/// IProject implementors should implement this interface, but only the SharpDevelop methods
	/// Solution.RenameProjectPlatform etc. may call the interface members.
	/// </summary>
	public interface IProjectAllowChangeConfigurations
	{
		bool RenameProjectConfiguration(string oldName, string newName);
		bool RenameProjectPlatform(string oldName, string newName);
		bool AddProjectConfiguration(string newName, string copyFrom);
		bool AddProjectPlatform(string newName, string copyFrom);
		bool RemoveProjectConfiguration(string name);
		bool RemoveProjectPlatform(string name);
	}
}
