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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Base interface for projects.
	/// Thread-safe members lock on the SyncRoot. Non-thread-safe members may only be called from the main thread.
	/// 
	/// When you implement IProject, you should also implement IProjectItemListProvider and IProjectAllowChangeConfigurations
	/// </summary>
	public interface IProject
		: IBuildable, ISolutionItem, IDisposable, IConfigurable
	{
		/// <summary>
		/// Gets the object used for thread-safe synchronization.
		/// Thread-safe members lock on this object, but if you manipulate underlying structures
		/// (such as the MSBuild project for MSBuildBasedProjects) directly, you will have to lock on this object.
		/// </summary>
		object SyncRoot { get; }
		
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is thread-safe; any accesses will synchronize with the project's <see cref="SyncRoot"/>.
		/// Enumerating the items collection will create a snapshot of the collection.
		/// </summary>
		IMutableModelCollection<ProjectItem> Items { get; }
		
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
		IReadOnlyCollection<ItemType> AvailableFileItemTypes { get; }
		
		/// <summary>
		/// Gets a list of project sections stored in the solution file for this project.
		/// </summary>
		IMutableModelCollection<SolutionSection> ProjectSections { get; }
		
		/// <summary>
		/// Gets the name of the project file.
		/// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		FileName FileName { get; set; }
		
		/// <summary>
		/// Gets/Sets the name of the project.
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		/// <remarks>
		/// Name already exists in IBuildable; we're adding the setter here.
		/// </remarks>
		new string Name { get; set; }
		
		/// <summary>
		/// Gets the directory of the project file.
		/// This is equivalent to Path.GetDirectoryName(project.FileName);
		/// (Example: @"D:\Serralongue\SharpDevelop\samples\CustomPad")
		/// 
		/// This member is thread-safe.
		/// </summary>
		DirectoryName Directory { get; }
		
		/// <summary>
		/// <para>
		/// True if the project is readonly. For project based files this means
		/// the project file has the readonly attribute set. For solution folder
		/// based projects this means that the sln file containing the project
		/// has the readonly attribute set.
		/// </para>
		/// <para>This member is thread-safe.</para>
		/// </summary>
		bool IsReadOnly { get; }
		
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
		FileName OutputAssemblyFullPath {
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
		
		/// <summary>
		/// Gets the configuration mapping.
		/// </summary>
		ConfigurationMapping ConfigurationMapping { get; }
		
		/// <summary>
		/// Saves the project using its current file name.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Returns true, if a specific file (given by its name) is inside this project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		bool IsFileInProject(FileName fileName);
		
		/// <summary>
		/// Returns the project item for a specific file; or null if the file is not found in the project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		FileProjectItem FindFile(FileName fileName);
		
		/// <summary>
		/// Gets if the project can be started.
		/// </summary>
		bool IsStartable { get; }
		
		/// <summary>
		/// Gets project specific properties.
		/// These are saved in as part of the SharpDevelop configuration in the AppData folder.
		/// </summary>
		/// <remarks>
		/// This property never returns null.
		/// 
		/// Use <see cref="LoadProjectExtensions"/> instead to store settings that are for multiple users.
		/// </remarks>
		Properties Preferences { get; }
		
		/// <summary>
		/// Saves the <see cref="Preferences"/> to disk.
		/// This method is called by SharpDevelop when the solution is closed.
		/// </summary>
		void SavePreferences();
		
		/// <summary>
		/// Starts the project.
		/// </summary>
		/// <param name="withDebugging">True, if a debugger should be used for the project.</param>
		void Start(bool withDebugging);
		
		/// <summary>
		/// Creates a new ProjectItem for the passed MSBuild item.
		/// </summary>
		ProjectItem CreateProjectItem(IProjectItemBackendStore item);
		
		/// <summary>
		/// Gets the minimum version the solution must have to support this project type.
		/// </summary>
		SolutionFormatVersion MinimumSolutionVersion { get; }
		
		/// <summary>
		/// Resolves assembly references for this project.
		/// The resulting list of resolved references will include project references.
		/// </summary>
		IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(CancellationToken cancellationToken);
		
		/// <summary>
		/// Notifies the project that it was succesfully created from a project template.
		/// </summary>
		/// <remarks>
		/// TODO This method is currently called before the project is added to the solution;
		/// but we might change that so that it is called later.
		/// </remarks>
		void ProjectCreationComplete();
		
		/// <summary>
		/// Notifies the project that it was loaded in the IDE.
		/// This method is called after the whole solution has finished loading; and when existing projects are added to the open solution.
		/// It is not called for newly created projects; and not if the solution was loaded in the background
		/// (<see cref="IProjectService.LoadSolutionFile"/> vs. <see cref="IProjectService.OpenSolution"/>).
		/// </summary>
		void ProjectLoaded();
		
		/// <summary>
		/// Loads the project extension content with the specified name.
		/// </summary>
		/// <remarks>
		/// Project extensions are custom XML elements that are stored within the .csproj file.
		/// They are intended for settings that are not specific to a user/machine.
		/// 
		/// Use <see cref="Preferences"/> instead to store per-user settings.
		/// </remarks>
		XElement LoadProjectExtensions(string name);
		
		/// <summary>
		/// Saves the project extension content with the specified name.
		/// </summary>
		/// <remarks>
		/// Project extensions are custom XML elements that are stored within the .csproj file.
		/// They are intended for settings that are not specific to a user/machine.
		/// 
		/// Use <see cref="Preferences"/> instead to store per-user settings.
		/// </remarks>
		void SaveProjectExtensions(string name, XElement element);
		
		/// <summary>
		/// Determines whether this project has the specified type.
		/// Projects may have multiple type GUIDs.
		/// </summary>
		bool HasProjectType(Guid projectTypeGuid);
		
		/// <summary>
		/// Gets the project content associated with this project.
		/// </summary>
		/// <remarks>
		/// This property must always return the same value for the same project.
		/// This property may return null.
		/// 
		/// This member is thread-safe.
		/// </remarks>
		IProjectContent ProjectContent { get; }
		
		/// <summary>
		/// Gets the default namespace to use for a file with the specified name.
		/// </summary>
		/// <param name="fileName">Full file name for a new file being added to the project.</param>
		/// <returns>Namespace name to use for the new file</returns>
		string GetDefaultNamespace(string fileName);
		
		/// <summary>
		/// Creates a CodeDomProvider for this project's language.
		/// Returns null when no CodeDomProvider is available for the language.
		/// </summary>
		System.CodeDom.Compiler.CodeDomProvider CreateCodeDomProvider();
		
		/// <summary>
		/// Generates code for a CodeDom compile unit.
		/// This method is used by CustomToolContext.WriteCodeDomToFile.
		/// </summary>
		void GenerateCodeFromCodeDom(System.CodeDom.CodeCompileUnit compileUnit, TextWriter writer);
		
		/// <summary>
		/// Creates a new ambience for this project.
		/// </summary>
		/// <remarks>
		/// This member is thread-safe.
		/// As ambiences are not thread-safe, this method always returns a new ambience instance.
		/// Never returns null.
		/// </remarks>
		IAmbience GetAmbience();
		
		/// <summary>
		/// Returns the ILanguageBinding implementation for this project.
		/// </summary>
		ILanguageBinding LanguageBinding { get; }
		
		/// <summary>
		/// Prepares searching for references to the specified entity.
		/// This method should calculate the amount of work to be done (e.g. using the number of files to search through),
		/// it should not perform the actual search.
		/// </summary>
		/// <returns>
		/// An object that can be used to perform the search; or null if this project does not support symbol searches.
		/// </returns>
		Refactoring.ISymbolSearch PrepareSymbolSearch(ISymbol entity);
		
		/// <summary>
		/// Occurs whenever parse information for this project was updated. This event is raised on the main thread.
		/// </summary>
		event EventHandler<ParseInformationEventArgs> ParseInformationUpdated;
		
		/// <summary>
		/// Notifies the project that the parse information was updated.
		/// This method is called by the parser service <b>within a per-file lock</b>.
		/// </summary>
		void OnParseInformationUpdated(ParseInformationEventArgs args);
		
		/// <summary>
		/// Gets the assembly model for the project. This property never returns null.
		/// </summary>
		IAssemblyModel AssemblyModel { get; }
		
		/// <summary>
		/// Gets whether this project was unloaded.
		/// </summary>
		bool IsDisposed { get; }
		
		event EventHandler Disposed;
	}
}
