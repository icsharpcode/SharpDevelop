// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using MSBuild = Microsoft.Build.Evaluation;
using StringPair = System.Tuple<string, string>;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project that is based on an MSBuild project file.
	/// 
	/// Thread-safety: most members are thread-safe, but direct accesses on the underlying MSBuildProject
	/// require locking on the SyncRoot. Methods that return underlying MSBuild objects require that
	/// the caller locks on the SyncRoot.
	/// </summary>
	public class MSBuildBasedProject : AbstractProject, IProjectItemListProvider, IProjectAllowChangeConfigurations
	{
		/// <summary>
		/// The project collection that contains this project.
		/// </summary>
		ProjectCollection projectCollection;
		
		internal ProjectCollection MSBuildProjectCollection {
			get { return projectCollection; }
		}
		
		/// <summary>
		/// The underlying MSBuild project.
		/// </summary>
		ProjectRootElement projectFile;
		
		/// <summary>
		/// The '.user' part of the project.
		/// </summary>
		ProjectRootElement userProjectFile;
		
		/// <summary>
		/// A list of project properties that are saved after the normal properties.
		/// Use this for properties that could reference other properties, e.g.
		/// PostBuildEvent references OutputPath.
		/// </summary>
		protected readonly ISet<string> saveAfterImportsProperties = new SortedSet<string> {
			"PostBuildEvent",
			"PreBuildEvent"
		};
		
		public override void Dispose()
		{
			base.Dispose();
			UnloadCurrentlyOpenProject();
			// unload project + userProject:
			projectFile = null;
			userProjectFile = null;
		}
		
		/// <summary>
		/// Gets the MSBuild.Construction project file.
		/// You must lock on the project's SyncRoot before accessing the MSBuild project file!
		/// </summary>
		[Browsable(false)]
		public ProjectRootElement MSBuildProjectFile {
			get {
				if (projectFile == null)
					throw new ObjectDisposedException("MSBuildBasedProject");
				return projectFile;
			}
		}
		
		/// <summary>
		/// Gets the MSBuild.Construction project file.
		/// You must lock on the project's SyncRoot before accessing the MSBuild project file!
		/// </summary>
		[Browsable(false)]
		public ProjectRootElement MSBuildUserProjectFile {
			get {
				if (projectFile == null)
					throw new ObjectDisposedException("MSBuildBasedProject");
				return userProjectFile;
			}
		}
		
		public override int MinimumSolutionVersion {
			get {
				lock (SyncRoot) {
					// This property is called by CSharpProject.StartBuild (and other derived StartBuild methods),
					// so it's important that we throw an ObjectDisposedException for disposed projects.
					// The build engine will handle this exception (occurs when unloading a project while a build is running)
					if (projectFile == null)
						throw new ObjectDisposedException("MSBuildBasedProject");
					if (string.IsNullOrEmpty(projectFile.ToolsVersion) || projectFile.ToolsVersion == "2.0") {
						return Solution.SolutionVersionVS2005;
					} else if (projectFile.ToolsVersion == "3.0" || projectFile.ToolsVersion == "3.5") {
						return Solution.SolutionVersionVS2008;
					} else {
						return Solution.SolutionVersionVS2010;
					}
				}
			}
		}
		
		public event EventHandler MinimumSolutionVersionChanged;
		
		protected internal void SetToolsVersion(string newToolsVersion)
		{
			PerformUpdateOnProjectFile(
				delegate {
					projectFile.ToolsVersion = newToolsVersion;
					userProjectFile.ToolsVersion = newToolsVersion;
				});
			
			if (MinimumSolutionVersionChanged != null)
				MinimumSolutionVersionChanged(this, EventArgs.Empty);
		}
		
		public void PerformUpdateOnProjectFile(Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			lock (SyncRoot) {
				UnloadCurrentlyOpenProject();
				action();
				CreateItemsListFromMSBuild();
			}
		}
		
		public override void ResolveAssemblyReferences()
		{
			MSBuildInternals.ResolveAssemblyReferences(this);
		}
		
		public override IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(CancellationToken cancellationToken)
		{
			ReferenceProjectItem[] additionalItems = {
				new ReferenceProjectItem(this, "mscorlib")
			};
			return MSBuildInternals.ResolveAssemblyReferences(this, additionalItems);
		}
		
		#region Create new project
		public MSBuildBasedProject(ProjectCreateInformation information)
		{
			this.projectCollection = information.Solution.MSBuildProjectCollection;
			this.projectFile = ProjectRootElement.Create(projectCollection);
			this.userProjectFile = ProjectRootElement.Create(projectCollection);
			this.ActivePlatform = information.Platform;
			
			Name = information.ProjectName;
			FileName = information.OutputProjectFileName;
			
			projectFile.FullPath = information.OutputProjectFileName;
			projectFile.ToolsVersion = "4.0";
			projectFile.DefaultTargets = "Build";
			userProjectFile.FullPath = information.OutputProjectFileName + ".user";
			
			base.IdGuid = "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
			projectFile.AddProperty(ProjectGuidPropertyName, IdGuid);
			AddGuardedProperty("Configuration", "Debug");
			AddGuardedProperty("Platform", information.Platform);
			
			this.ActiveConfiguration = "Debug";
			this.ActivePlatform = information.Platform;
			if (information.Platform == "x86")
				SetProperty(null, information.Platform, "PlatformTarget", "x86", PropertyStorageLocations.PlatformSpecific, false);
			else
				SetProperty(null, information.Platform, "PlatformTarget", "AnyCPU", PropertyStorageLocations.PlatformSpecific, false);
		}
		
		/// <summary>
		/// The MSBuild property used to store the project's IdGuid.
		/// The IdGuid is only stored in the project file to make multiple solutions use the same
		/// GUID for the project when the project is added to multiple solutions. However, the actual
		/// GUID used for the project in the solution can differ from the GUID in the project file -
		/// SharpDevelop does not try to correct mismatches but simply always use the value from the solution.
		/// SharpDevelop creates a new GUID for the solution when the project GUID cannot be used because it
		/// would conflict with another project. This happens when one project is created by copying another project.
		/// </summary>
		public const string ProjectGuidPropertyName = "ProjectGuid";
		
		/// <summary>
		/// Adds a guarded property:
		/// &lt;<paramref name="name"/> Condition=" '$(<paramref name="name"/>)' == '' "
		/// </summary>
		protected void AddGuardedProperty(string name, string value)
		{
			AddConditionalProperty(name, value, " '$(" + name + ")' == '' ");
		}
		
		protected void AddConditionalProperty(string name, string value, string condition)
		{
			lock (SyncRoot) {
				projectFile.AddProperty(name, value).Condition = condition;
			}
		}
		
		/// <summary>
		/// Adds an MSBuild import to the project.
		/// </summary>
		protected void AddImport(string importedProjectFile, string condition)
		{
			lock (SyncRoot) {
				projectFile.AddImport(importedProjectFile).Condition = condition;
			}
		}
		#endregion
		
		#region Get Property
		/// <summary>
		/// Retrieves the evaluated property '<paramref name="propertyName"/>' from the
		/// active configuration/platform. This method can retrieve any MSBuild property, including those
		/// defined in imported .target files.
		/// </summary>
		/// <param name="propertyName">The name of the MSBuild property to read.</param>
		/// <returns>The evaluated value of the property, or null if the property doesn't exist</returns>
		public string GetEvaluatedProperty(string propertyName)
		{
			using (var c = OpenCurrentConfiguration()) {
				return c.Project.GetPropertyValue(propertyName);
			}
		}
		
		/// <summary>
		/// Retrieves the evaluated property '<paramref name="propertyName"/>' from the
		/// specified configuration/platform.
		/// </summary>
		/// <param name="configuration">The configuration to use.</param>
		/// <param name="platform">The platform to use.</param>
		/// <param name="propertyName">The name of the MSBuild property to read.</param>
		/// <returns>The evaluated value of the property, or null if the property doesn't exist</returns>
		public string GetProperty(string configuration, string platform, string propertyName)
		{
			PropertyStorageLocations tmp;
			return GetProperty(configuration, platform, propertyName, out tmp);
		}
		
		/// <summary>
		/// Retrieves the evaluated property '<paramref name="propertyName"/>' from the
		/// specified configuration/platform.
		/// </summary>
		/// <param name="configuration">The configuration to use.</param>
		/// <param name="platform">The platform to use.</param>
		/// <param name="propertyName">The name of the MSBuild property to read.</param>
		/// <param name="location">[Out], the storage location where the property was found</param>
		/// <returns>The evaluated value of the property, or null if the property doesn't exist</returns>
		public string GetProperty(string configuration, string platform, string propertyName,
		                          out PropertyStorageLocations location)
		{
			using (var c = OpenConfiguration(configuration, platform)) {
				bool wasHiddenByImportedProperty;
				var prop = c.GetNonImportedProperty(propertyName, out wasHiddenByImportedProperty);
				if (prop != null) {
					location = c.GetLocation(prop);
					return prop.EvaluatedValue;
				} else if (wasHiddenByImportedProperty) {
					string unevaluated = GetAnyUnevaluatedPropertyValue(configuration, platform, propertyName, out location);
					if (unevaluated != null) {
						return c.Project.ExpandString(unevaluated);
					}
					return null;
				} else {
					location = PropertyStorageLocations.Unknown;
					return null;
				}
			}
		}
		
		/// <summary>
		/// Retrieves the raw value of the property '<paramref name="propertyName"/>' from the
		/// current configuration/platform.
		/// </summary>
		/// <param name="propertyName">The name of the MSBuild property to read.</param>
		/// <returns>The raw value of the property, or null if the property doesn't exist</returns>
		public string GetUnevalatedProperty(string propertyName)
		{
			return GetUnevalatedProperty(this.ActiveConfiguration, this.ActivePlatform, propertyName);
		}
		
		/// <summary>
		/// Retrieves the raw value of the property '<paramref name="propertyName"/>' from the
		/// specified configuration/platform.
		/// </summary>
		/// <param name="configuration">The configuration to use.</param>
		/// <param name="platform">The platform to use.</param>
		/// <param name="propertyName">The name of the MSBuild property to read.</param>
		/// <returns>The raw value of the property, or null if the property doesn't exist</returns>
		public string GetUnevalatedProperty(string configuration, string platform,
		                                    string propertyName)
		{
			PropertyStorageLocations tmp;
			return GetUnevalatedProperty(configuration, platform, propertyName, out tmp);
		}
		
		/// <summary>
		/// Retrieves the raw value of the property '<paramref name="propertyName"/>' from the
		/// specified configuration/platform.
		/// </summary>
		/// <param name="configuration">The configuration to use.</param>
		/// <param name="platform">The platform to use.</param>
		/// <param name="propertyName">The name of the MSBuild property to read.</param>
		/// <param name="location">[Out], the storage location where the property was found</param>
		/// <returns>The raw value of the property, or null if the property doesn't exist</returns>
		public string GetUnevalatedProperty(string configuration, string platform,
		                                    string propertyName,
		                                    out PropertyStorageLocations location)
		{
			using (var c = OpenConfiguration(configuration, platform)) {
				bool wasHiddenByImportedProperty;
				var prop = c.GetNonImportedProperty(propertyName, out wasHiddenByImportedProperty);
				if (prop != null) {
					location = c.GetLocation(prop);
					return prop.UnevaluatedValue;
				} else if (wasHiddenByImportedProperty) {
					return GetAnyUnevaluatedPropertyValue(configuration, platform, propertyName, out location);
				} else {
					location = PropertyStorageLocations.Unknown;
					return null;
				}
			}
		}
		
		protected void ReevaluateIfNecessary()
		{
			using (var c = OpenCurrentConfiguration()) {
				c.Project.ReevaluateIfNecessary();
			}
		}
		
		MSBuild.Project currentlyOpenProject;
		
		void UnloadCurrentlyOpenProject()
		{
			if (currentlyOpenProject != null) {
				MSBuildInternals.UnloadProject(projectCollection, currentlyOpenProject);
				currentlyOpenProject = null;
			}
		}
		
		/// <summary>
		/// Creates an MSBuild project instance.
		/// This method is thread-safe.
		/// </summary>
		public Microsoft.Build.Execution.ProjectInstance CreateProjectInstance()
		{
			using (var c = OpenCurrentConfiguration()) {
				return c.Project.CreateProjectInstance();
			}
		}
		
		/// <summary>
		/// calls OpenConfiguration for the current configuration
		/// </summary>
		ConfiguredProject OpenCurrentConfiguration()
		{
			return OpenConfiguration(null, null);
		}
		
		/// <summary>
		/// Provides access to the underlying MSBuild.Evaluation project.
		/// Usage:
		/// using (ConfiguredProject c = OpenCurrentConfiguration()) {
		///    // access c.Project only in this block
		/// }
		/// This method is thread-safe: calling it locks the SyncRoot. You have to dispose
		/// the ConfiguredProject instance to unlock the SyncRoot.
		/// </summary>
		ConfiguredProject OpenConfiguration(string configuration, string platform)
		{
			bool lockTaken = false;
			try {
				System.Threading.Monitor.Enter(this.SyncRoot, ref lockTaken);
				
				if (projectFile == null)
					throw new ObjectDisposedException("MSBuildBasedProject");
				
				if (configuration == null)
					configuration = this.ActiveConfiguration;
				if (platform == null)
					platform = this.ActivePlatform;
				
				bool openCurrentConfiguration = configuration == this.ActiveConfiguration && platform == this.ActivePlatform;
				
				if (currentlyOpenProject != null && openCurrentConfiguration) {
					// use currently open project
					currentlyOpenProject.ReevaluateIfNecessary();
					return new ConfiguredProject(this, currentlyOpenProject, false);
				}
				
				Dictionary<string, string> globalProps = new Dictionary<string, string>();
				InitializeMSBuildProjectProperties(globalProps);
				globalProps["Configuration"] = configuration;
				
				//HACK: the ActivePlatform property should be set properly before entering here, but sometimes it does not
				if (platform != null)
					globalProps["Platform"] = platform;
				MSBuild.Project project = MSBuildInternals.LoadProject(projectCollection, projectFile, globalProps);
				if (openCurrentConfiguration)
					currentlyOpenProject = project;
				return new ConfiguredProject(this, project, !openCurrentConfiguration);
			} catch {
				// Leave lock only on exceptions.
				// If there's no exception, the lock will be left when the ConfiguredProject
				// is disposed.
				if (lockTaken)
					System.Threading.Monitor.Exit(this.SyncRoot);
				throw;
			}
		}
		
		sealed class ConfiguredProject : IDisposable
		{
			readonly MSBuildBasedProject p;
			readonly bool unloadProjectOnDispose;
			public readonly MSBuild.Project Project;
			
			internal ConfiguredProject(MSBuildBasedProject parent, MSBuild.Project project, bool unloadProjectOnDispose)
			{
				this.p = parent;
				this.Project = project;
				this.unloadProjectOnDispose = unloadProjectOnDispose;
			}
			
			public MSBuild.ProjectProperty GetNonImportedProperty(string name, out bool wasHiddenByImportedProperty)
			{
				wasHiddenByImportedProperty = false;
				var prop = Project.GetProperty(name);
				if (prop != null && prop.Xml != null) {
					if (prop.Xml.ContainingProject == p.projectFile || prop.Xml.ContainingProject == p.userProjectFile)
						return prop;
					else
						wasHiddenByImportedProperty = true;
				}
				return null;
			}
			
			public PropertyStorageLocations GetLocation(MSBuild.ProjectProperty prop)
			{
				var location = MSBuildInternals.GetLocationFromCondition(prop.Xml);
				if (prop.Xml.ContainingProject == p.userProjectFile)
					location |= PropertyStorageLocations.UserFile;
				return location;
			}
			
			public void Dispose()
			{
				try {
					if (unloadProjectOnDispose) {
						MSBuildInternals.UnloadProject(p.projectCollection, this.Project);
					}
				} finally {
					System.Threading.Monitor.Exit(p.SyncRoot);
				}
			}
		}
		
		/// <summary>
		/// Finds the <c>BuildProperty</c> object used to store <paramref name="propertyName"/>
		/// in the specified configuration/platform.
		/// </summary>
		/// 
		/// Warning: you need to lock(project.SyncRoot) around calls to GetAllProperties
		/// until you no longer need to access the BuildProperty objects!
		/// <param name="configuration">The configuration to use.</param>
		/// <param name="platform">The platform to use.</param>
		/// <param name="propertyName">The property to look for.</param>
		/// <param name="group">[Out], the property group in which the property was found</param>
		/// <param name="location">[Out], the storage location the condition of the property
		/// group was referring to</param>
		protected ProjectPropertyElement FindPropertyObject(string configuration, string platform,
		                                                    string propertyName,
		                                                    out ProjectPropertyGroupElement group,
		                                                    out PropertyStorageLocations location)
		{
			using (var c = OpenConfiguration(configuration, platform)) {
				bool wasHiddenByImportedProperty;
				var prop = c.GetNonImportedProperty(propertyName, out wasHiddenByImportedProperty);
				if (prop != null) {
					group = (ProjectPropertyGroupElement)prop.Xml.Parent;
					location = c.GetLocation(prop);
					return prop.Xml;
				} else {
					group = null;
					location = PropertyStorageLocations.Unknown;
					return null;
				}
			}
		}
		
		/// <summary>
		/// Gets the unevaluated value of any property with the name <paramref name="propertyName"/>
		/// </summary>
		/// <param name="configuration">Configuration filter. Only use properties from this
		/// configuration. Use <c>null</c> to allow properties from all configurations</param>
		/// <param name="platform">Platform filter. Only use properties from this platform.
		/// Use <c>null</c> to allow properties from all platforms.</param>
		/// <param name="propertyName">The name of the property</param>
		string GetAnyUnevaluatedPropertyValue(string configuration, string platform, string propertyName)
		{
			PropertyStorageLocations tmp;
			return GetAnyUnevaluatedPropertyValue(configuration, platform, propertyName, out tmp);
		}
		
		string GetAnyUnevaluatedPropertyValue(string configuration, string platform, string propertyName, out PropertyStorageLocations location)
		{
			// first try main project file, then try user project file
			ProjectPropertyElement p = GetAnyUnevaluatedProperty(projectFile, configuration, platform, propertyName, out location);
			if (p == null) {
				p = GetAnyUnevaluatedProperty(userProjectFile, configuration, platform, propertyName, out location);
				if (p != null)
					location |= PropertyStorageLocations.UserFile;
			}
			return p != null ? p.Value : null;
		}
		
		static ProjectPropertyElement GetAnyUnevaluatedProperty(ProjectRootElement project, string configuration, string platform, string propertyName, out PropertyStorageLocations location)
		{
			location = PropertyStorageLocations.Unknown;
			foreach (var g in project.PropertyGroups) {
				var property = g.Properties.FirstOrDefault(p => MSBuildInternals.PropertyNameComparer.Equals(p.Name, propertyName));
				if (property == null)
					continue;
				string gConfiguration, gPlatform;
				MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
				                                                          out gConfiguration,
				                                                          out gPlatform);
				if ((configuration == null || configuration == gConfiguration || gConfiguration == null)
				    && (platform == null || platform == gPlatform || gPlatform == null))
				{
					if (gConfiguration == null && gPlatform == null) {
						location = PropertyStorageLocations.Base;
					} else {
						location = 0;
						if (gConfiguration != null)
							location |= PropertyStorageLocations.ConfigurationSpecific;
						if (gPlatform != null)
							location |= PropertyStorageLocations.PlatformSpecific;
					};
					return property;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Get all instances of the specified property.
		/// 
		/// Warning: you need to lock(project.SyncRoot) around calls to GetAllUnevaluatedProperties
		/// until you no longer need to access the ProjectPropertyElement objects!
		/// </summary>
		IEnumerable<ProjectPropertyElement> GetAllUnevaluatedProperties()
		{
			return projectFile.Properties.Concat(userProjectFile.Properties);
		}
		
		/// <summary>
		/// Changes all instances of a property in the <paramref name="project"/> by applying a method to its unevaluated value.
		/// 
		/// The method will be called within a <code>lock (project.SyncRoot)</code> block.
		/// </summary>
		public void ChangeProperty(string propertyName, Func<string, string> method)
		{
			lock (this.SyncRoot) {
				foreach (ProjectPropertyElement p in GetAllUnevaluatedProperties()) {
					if (MSBuildInternals.PropertyNameComparer.Equals(p.Name, propertyName)) {
						p.Value = method(p.Value);
					}
				}
			}
		}
		#endregion
		
		#region SetProperty
		public event EventHandler<ProjectPropertyChangedEventArgs> PropertyChanged;
		
		protected virtual void OnPropertyChanged(ProjectPropertyChangedEventArgs e)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}
		
		/// <summary>
		/// Tries to find an existing property in all configurations/platforms.
		/// Used for getting the old storage location before changing it.
		/// </summary>
		PropertyStorageLocations FindExistingPropertyInAllConfigurations(string propertyName)
		{
			foreach (var g in projectFile.PropertyGroups) {
				if (g.Properties.Any(p => MSBuildInternals.PropertyNameComparer.Equals(p.Name, propertyName))) {
					return MSBuildInternals.GetLocationFromCondition(g.Condition);
				}
			}
			foreach (var g in userProjectFile.PropertyGroups) {
				if (g.Properties.Any(p => MSBuildInternals.PropertyNameComparer.Equals(p.Name, propertyName))) {
					return MSBuildInternals.GetLocationFromCondition(g.Condition)
						| PropertyStorageLocations.UserFile;
				}
			}
			return PropertyStorageLocations.Unknown;
		}
		
		/// <summary>
		/// Sets an MSBuild property in the active configuration and platform, keeping the
		/// old storage location.
		/// </summary>
		/// <param name="propertyName">The name of the property to set.</param>
		/// <param name="newValue">The new value of the property.
		/// Use <c>null</c> to remove the property.
		/// The value is treated as literal (special MSBuild-characters are escaped)</param>
		public void SetProperty(string propertyName, string newValue)
		{
			SetProperty(propertyName, newValue, true);
		}
		
		/// <summary>
		/// Sets an MSBuild property in the active configuration and platform, keeping the
		/// old storage location.
		/// </summary>
		/// <param name="propertyName">The name of the property to set.</param>
		/// <param name="newValue">The new value of the property.
		/// Use <c>null</c> to remove the property.</param>
		/// <param name="treatPropertyValueAsLiteral"><c>True</c> to treat the
		/// <paramref name="newValue"/> as literal (escape it before saving).
		/// </param>
		public void SetProperty(string propertyName, string newValue, bool treatPropertyValueAsLiteral)
		{
			SetProperty(ActiveConfiguration, ActivePlatform, propertyName, newValue,
			            PropertyStorageLocations.Unchanged, treatPropertyValueAsLiteral);
		}
		
		/// <summary>
		/// Sets an MSBuild property.
		/// </summary>
		/// <param name="configuration">The configuration to change the property in.</param>
		/// <param name="platform">The platform to change the property in.</param>
		/// <param name="propertyName">The name of the property to set.</param>
		/// <param name="newValue">The new value of the property.
		/// Use <c>null</c> to remove the property.</param>
		/// <param name="location">The location to save the property in.
		/// Use PropertyStorageLocations.Unchanged to keep the old location.</param>
		/// <param name="treatPropertyValueAsLiteral"><c>True</c> to treat the
		/// <paramref name="newValue"/> as literal (escape it before saving).
		/// </param>
		public void SetProperty(string configuration, string platform,
		                        string propertyName, string newValue,
		                        PropertyStorageLocations location,
		                        bool treatPropertyValueAsLiteral)
		{
			ProjectPropertyChangedEventArgs args;
			lock (SyncRoot) {
				args = SetPropertyInternal(configuration, platform, propertyName, newValue, location, treatPropertyValueAsLiteral);
			}
			if (args.NewValue != args.OldValue || args.NewLocation != args.OldLocation) {
				OnPropertyChanged(args);
			}
		}
		
		ProjectPropertyChangedEventArgs SetPropertyInternal(string configuration, string platform,
		                                                    string propertyName, string newValue,
		                                                    PropertyStorageLocations location,
		                                                    bool treatPropertyValueAsLiteral)
		{
			PropertyStorageLocations oldLocation;
			ProjectPropertyGroupElement existingPropertyGroup;
			ProjectPropertyElement existingProperty = FindPropertyObject(configuration, platform,
			                                                             propertyName,
			                                                             out existingPropertyGroup,
			                                                             out oldLocation);
			// Try to get accurate oldLocation
			if (oldLocation == PropertyStorageLocations.Unknown) {
				oldLocation = FindExistingPropertyInAllConfigurations(propertyName);
				if (oldLocation == PropertyStorageLocations.Unknown) {
					oldLocation = PropertyStorageLocations.Base;
				}
			}
			// Set new location to old location if storage location should remain unchanged
			if (location == PropertyStorageLocations.Unchanged) {
				location = oldLocation;
			}
			// determine the insertion position for the property
			PropertyPosition propertyInsertionPosition;
			if (saveAfterImportsProperties.Contains(propertyName)) {
				propertyInsertionPosition = PropertyPosition.UseExistingOrCreateAfterLastImport;
			} else {
				propertyInsertionPosition = PropertyPosition.UseExistingOrCreateAfterLastPropertyGroup;
			}
			// get the project file where the property should be stored
			ProjectRootElement targetProject;
			if ((location & PropertyStorageLocations.UserFile) == PropertyStorageLocations.UserFile)
				targetProject = userProjectFile;
			else
				targetProject = projectFile;
			
			if (oldLocation != location) {
				// move existing properties to new location, then use the normal property
				// setting code at end of this method
				
				switch (location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
					case 0:
						// Set base property - remove all previous copies of the property
						RemovePropertyCompletely(propertyName);
						break;
					case PropertyStorageLocations.ConfigurationSpecific:
						// Get any value usable as existing property value (once per configuration)
						Dictionary<string, string> oldValuesConf = new Dictionary<string, string>();
						foreach (string conf in this.ConfigurationNames) {
							oldValuesConf[conf] = GetAnyUnevaluatedPropertyValue(conf, null, propertyName);
						}
						
						// Remove the property
						RemovePropertyCompletely(propertyName);
						
						// Recreate the property using the saved value
						foreach (KeyValuePair<string, string> pair in oldValuesConf) {
							if (pair.Value != null) {
								MSBuildSetProperty(targetProject, propertyName, pair.Value,
								                   CreateCondition(pair.Key, null, location),
								                   propertyInsertionPosition,
								                   false);
							}
						}
						break;
					case PropertyStorageLocations.PlatformSpecific:
						// Get any value usable as existing property value (once per platform)
						Dictionary<string, string> oldValuesPlat = new Dictionary<string, string>();
						foreach (string plat in this.PlatformNames) {
							oldValuesPlat[plat] = GetAnyUnevaluatedPropertyValue(null, plat, propertyName);
						}
						
						// Remove the property
						RemovePropertyCompletely(propertyName);
						
						// Recreate the property using the saved value
						foreach (KeyValuePair<string, string> pair in oldValuesPlat) {
							if (pair.Value != null) {
								MSBuildSetProperty(targetProject, propertyName, pair.Value,
								                   CreateCondition(null, pair.Key, location),
								                   propertyInsertionPosition,
								                   false);
							}
						}
						break;
					case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
						// Get any value usable as existing property value (once per configuration+platform)
						Dictionary<StringPair, string> oldValues = new Dictionary<StringPair, string>();
						foreach (string conf in this.ConfigurationNames) {
							foreach (string plat in this.PlatformNames) {
								oldValues[new StringPair(conf, plat)] = GetAnyUnevaluatedPropertyValue(conf, plat, propertyName);
							}
						}
						
						// Remove the property
						RemovePropertyCompletely(propertyName);
						
						// Recreate the property using the saved value
						foreach (KeyValuePair<StringPair, string> pair in oldValues) {
							if (pair.Value != null) {
								MSBuildSetProperty(targetProject, propertyName, pair.Value,
								                   CreateCondition(pair.Key.Item1, pair.Key.Item2, location),
								                   propertyInsertionPosition,
								                   false);
							}
						}
						break;
					default:
						throw new NotSupportedException();
				}
				
				// update existingProperty and existingPropertyGroup after the move operation
				PropertyStorageLocations tmpLocation;
				existingProperty = FindPropertyObject(configuration,
				                                      platform,
				                                      propertyName,
				                                      out existingPropertyGroup,
				                                      out tmpLocation);
			}
			ProjectPropertyChangedEventArgs args;
			args = new ProjectPropertyChangedEventArgs(propertyName);
			args.Configuration = configuration;
			args.Platform = platform;
			args.NewLocation = location;
			args.OldLocation = oldLocation;
			if (newValue != null) {
				args.NewValue = treatPropertyValueAsLiteral ? MSBuildInternals.Escape(newValue) : newValue;
			}
			
			if (existingPropertyGroup != null && existingProperty != null) {
				// update or delete existing property
				args.OldValue = existingProperty.Value;
				MSBuildSetProperty(targetProject, propertyName, newValue ?? "",
				                   existingPropertyGroup.Condition,
				                   propertyInsertionPosition,
				                   treatPropertyValueAsLiteral);
				if (newValue == null) {
					// delete existing property
					existingPropertyGroup.RemoveChild(existingProperty);
					
					if (existingPropertyGroup.Count == 0) {
						targetProject.RemoveChild(existingPropertyGroup);
					}
				}
			} else if (newValue != null) {
				// create new property
				MSBuildSetProperty(targetProject, propertyName, newValue,
				                   CreateCondition(configuration, platform, location),
				                   propertyInsertionPosition,
				                   treatPropertyValueAsLiteral);
			}
			return args;
		}
		
		enum PropertyPosition
		{
			UseExistingOrCreateAfterLastPropertyGroup,
			UseExistingOrCreateAfterLastImport
		}
		
		void MSBuildSetProperty(ProjectRootElement targetProject, string propertyName, string newValue,
		                        string groupCondition, PropertyPosition position,
		                        bool treatPropertyValueAsLiteral)
		{
			if (treatPropertyValueAsLiteral)
				newValue = MSBuildInternals.Escape(newValue);
			if (groupCondition == null) {
				// MSBuild uses an empty string when there's no condition, so we need to do the same
				// for the comparison to succeed.
				groupCondition = string.Empty;
			}
			foreach (var propertyGroup in targetProject.PropertyGroups) {
				if (propertyGroup.Condition == groupCondition) {
					foreach (var property in propertyGroup.Properties.ToList()) {
						if (MSBuildInternals.PropertyNameComparer.Equals(property.Name, propertyName)) {
							property.Value = newValue;
							return;
						}
					}
				}
			}
			
			var matchedPropertyGroup = FindPropertyGroup(targetProject, groupCondition, position);
			if (matchedPropertyGroup != null) {
				matchedPropertyGroup.AddProperty(propertyName, newValue);
				return;
			}
			
			var newGroup = AddNewPropertyGroup(targetProject, position);
			newGroup.Condition = groupCondition;
			newGroup.AddProperty(propertyName, newValue);
		}
		
		ProjectPropertyGroupElement FindPropertyGroup(ProjectRootElement targetProject, string groupCondition, PropertyPosition position)
		{
			ProjectPropertyGroupElement matchedPropertyGroup = null;
			foreach (var projectItem in targetProject.Children) {
				ProjectPropertyGroupElement propertyGroup = projectItem as ProjectPropertyGroupElement;
				if (propertyGroup != null) {
					if (propertyGroup.Condition == groupCondition) {
						matchedPropertyGroup = propertyGroup;
						if (position != PropertyPosition.UseExistingOrCreateAfterLastImport) {
							return matchedPropertyGroup;
						}
					}
				}
				if (position == PropertyPosition.UseExistingOrCreateAfterLastImport) {
					if (projectItem is ProjectImportElement) {
						matchedPropertyGroup = null;
					}
				}
			}
			return matchedPropertyGroup;
		}
		
		ProjectPropertyGroupElement AddNewPropertyGroup(ProjectRootElement targetProject, PropertyPosition position)
		{
			if (position == PropertyPosition.UseExistingOrCreateAfterLastImport) {
				var propertyGroup = targetProject.CreatePropertyGroupElement();
				targetProject.AppendChild(propertyGroup);
				return propertyGroup;
			}
			return targetProject.AddPropertyGroup();
		}
		
		/// <summary>
		/// Removes the property from all configurations and platforms.
		/// </summary>
		void RemovePropertyCompletely(string propertyName)
		{
			RemovePropertyCompletely(projectFile, propertyName);
			RemovePropertyCompletely(userProjectFile, propertyName);
		}
		
		static void RemovePropertyCompletely(ProjectRootElement project, string propertyName)
		{
			foreach (var propertyGroup in project.PropertyGroups.ToList()) {
				bool propertyRemoved = false;
				foreach (var property in propertyGroup.Properties.ToList()) {
					if (MSBuildInternals.PropertyNameComparer.Equals(property.Name, propertyName)) {
						propertyGroup.RemoveChild(property);
						propertyRemoved = true;
					}
				}
				if (propertyRemoved && propertyGroup.Children.Count() == 0)
					project.RemoveChild(propertyGroup);
			}
		}
		
		/// <summary>
		/// Creates an MSBuild condition string.
		/// At most one of configuration and platform can be null.
		/// </summary>
		internal static string CreateCondition(string configuration, string platform)
		{
			if (configuration == null)
				return CreateCondition(configuration, platform, PropertyStorageLocations.PlatformSpecific);
			else if (platform == null)
				return CreateCondition(configuration, platform, PropertyStorageLocations.ConfigurationSpecific);
			else
				return CreateCondition(configuration, platform, PropertyStorageLocations.ConfigurationAndPlatformSpecific);
		}
		
		/// <summary>
		/// Creates an MSBuild condition string.
		/// configuration and platform may be only <c>null</c> if they are not required (as specified by the
		/// storage location), otherwise an ArgumentNullException is thrown.
		/// </summary>
		internal static string CreateCondition(string configuration, string platform, PropertyStorageLocations location)
		{
			switch (location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
				case PropertyStorageLocations.ConfigurationSpecific:
					if (configuration == null)
						throw new ArgumentNullException("configuration");
					return " '$(Configuration)' == '" + configuration + "' ";
				case PropertyStorageLocations.PlatformSpecific:
					if (platform == null)
						throw new ArgumentNullException("platform");
					return " '$(Platform)' == '" + platform + "' ";
				case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
					if (platform == null)
						throw new ArgumentNullException("platform");
					if (configuration == null)
						throw new ArgumentNullException("configuration");
					return " '$(Configuration)|$(Platform)' == '" + configuration + "|" + platform + "' ";
				default:
					return null;
			}
		}
		#endregion
		
		#region IProjectItemListProvider interface
		List<ProjectItem> items = new List<ProjectItem>();
		volatile ReadOnlyCollection<ProjectItem> itemsReadOnly;
		volatile ICollection<ItemType> availableFileItemTypes = ItemType.DefaultFileItems;
		
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is guaranteed not to change - adding new items or removing existing items
		/// will create a new collection.
		/// </summary>
		public override ReadOnlyCollection<ProjectItem> Items {
			get {
				ReadOnlyCollection<ProjectItem> c = itemsReadOnly;
				if (c == null) {
					lock (SyncRoot) {
						c = Array.AsReadOnly(items.ToArray());
					}
					itemsReadOnly = c;
				}
				return c;
			}
		}
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		public override ICollection<ItemType> AvailableFileItemTypes {
			get {
				return availableFileItemTypes;
			}
		}
		
		/// <summary>
		/// re-creates the list of project items and the list of available item types
		/// </summary>
		internal void CreateItemsListFromMSBuild()
		{
			WorkbenchSingleton.AssertMainThread();
			
			using (var c = OpenCurrentConfiguration()) {
				foreach (ProjectItem item in items) {
					item.Dispose();
				}
				items.Clear();
				itemsReadOnly = null; // remove readonly variant of item list - will regenerate on next Items call
				
				SortedSet<ItemType> availableFileItemTypes = new SortedSet<ItemType>();
				availableFileItemTypes.AddRange(ItemType.DefaultFileItems);
				foreach (var item in c.Project.GetItems("AvailableItemName")) {
					availableFileItemTypes.Add(new ItemType(item.EvaluatedInclude));
				}
				this.availableFileItemTypes = availableFileItemTypes.AsReadOnly();
				
				foreach (var item in c.Project.AllEvaluatedItems) {
					if (item.IsImported) continue;
					
					items.Add(CreateProjectItem(new MSBuildItemWrapper(this, item)));
				}
				
				ClearFindFileCache();
			}
			
			// refresh project browser to make sure references and other project items are still valid
			// after TargetFramework or other properties changed. Fixes SD-1876
			if (!isLoading)
				ProjectBrowserPad.RefreshViewAsync();
		}
		
		void IProjectItemListProvider.AddProjectItem(ProjectItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.Project != this)
				throw new ArgumentException("item does not belong to this project", "item");
			if (item.IsAddedToProject)
				throw new ArgumentException("item is already added to project", "item");
			
			WorkbenchSingleton.AssertMainThread();
			using (var c = OpenCurrentConfiguration()) {
				items.Add(item);
				itemsReadOnly = null; // remove readonly variant of item list - will regenerate on next Items call
				/*foreach (var g in projectFile.ItemGroups) {
					if (!string.IsNullOrEmpty(g.Condition) || g.Count == 0)
						continue;
					var firstItemInGroup = g.Items.First();
					if (firstItemInGroup.Name == item.ItemType.ItemName) {
						MSBuildInternals.AddItemToGroup(g, item);
						return;
					}
					if (firstItemInGroup.ItemType == ItemType.Reference.ItemName)
						continue;
					if (ItemType.DefaultFileItems.Contains(new ItemType(firstItemInGroup.ItemType))) {
						if (ItemType.DefaultFileItems.Contains(item.ItemType)) {
							MSBuildInternals.AddItemToGroup(g, item);
							return;
						} else {
							continue;
						}
					}
					
					MSBuildInternals.AddItemToGroup(g, item);
					return;
				}
				var newGroup = projectFile.AddItemGroup();
				MSBuildInternals.AddItemToGroup(newGroup, item);*/
				
				
				string newInclude = item.TreatIncludeAsLiteral ? MSBuildInternals.Escape(item.Include) : item.Include;
				var newMetadata = new Dictionary<string, string>();
				foreach (string name in item.MetadataNames) {
					newMetadata[name] = item.GetMetadata(name);
				}
				var newItems = c.Project.AddItem(item.ItemType.ItemName, newInclude, newMetadata);
				if (newItems.Count != 1)
					throw new InvalidOperationException("expected one new item, but got " + newItems.Count);
				item.BuildItem = new MSBuildItemWrapper((MSBuildBasedProject)item.Project, newItems[0]);
				Debug.Assert(item.IsAddedToProject);
			}
		}
		
		bool IProjectItemListProvider.RemoveProjectItem(ProjectItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.Project != this)
				throw new ArgumentException("item does not belong to this project", "item");
			if (!item.IsAddedToProject)
				return false;
			MSBuildItemWrapper backend = (MSBuildItemWrapper)item.BuildItem;
			
			using (var c = OpenCurrentConfiguration()) {
				if (items.Remove(item)) {
					itemsReadOnly = null; // remove readonly variant of item list - will regenerate on next Items call
					c.Project.RemoveItem(backend.MSBuildItem);
					item.BuildItem = null; // make the item free again
					return true;
				} else {
					throw new InvalidOperationException("Expected that the item is added to this project!");
				}
			}
		}
		#endregion
		
		#region Wrapped MSBuild Properties
		public override string AppDesignerFolder {
			get { return GetEvaluatedProperty("AppDesignerFolder"); }
			set { SetProperty("AppDesignerFolder", value); }
		}
		#endregion
		
		#region Building
		public override ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			ICollection<IBuildable> result = base.GetBuildDependencies(buildOptions);
			foreach (ProjectItem item in GetItemsOfType(ItemType.ProjectReference)) {
				ProjectReferenceProjectItem prpi = item as ProjectReferenceProjectItem;
				if (prpi != null && prpi.ReferencedProject != null)
					result.Add(prpi.ReferencedProject);
			}
			return result;
		}
		
		public override void StartBuild(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			MSBuildEngine.StartBuild(this, options, feedbackSink, MSBuildEngine.AdditionalTargetFiles);
		}
		
		public override ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
		{
			ProjectBuildOptions projectOptions = base.CreateProjectBuildOptions(options, isRootBuildable);
			Solution solution = this.ParentSolution;
			var configMatchings = solution.GetActiveConfigurationsAndPlatformsForProjects(options.SolutionConfiguration, options.SolutionPlatform);
			// Find the project configuration, and build an XML string containing all configurations from the solution
			StringWriter solutionConfigurationXml = new StringWriter();
			using (XmlTextWriter solutionConfigurationWriter = new XmlTextWriter(solutionConfigurationXml)) {
				solutionConfigurationWriter.WriteStartElement("SolutionConfiguration");
				foreach (var matching in configMatchings) {
					solutionConfigurationWriter.WriteStartElement("ProjectConfiguration");
					solutionConfigurationWriter.WriteAttributeString("Project", matching.Project.IdGuid);
					solutionConfigurationWriter.WriteValue(matching.Configuration + "|" + MSBuildInternals.FixPlatformNameForProject(matching.Platform));
					solutionConfigurationWriter.WriteEndElement();
				}
				solutionConfigurationWriter.WriteEndElement();
			}
			// Set property for solution configuration. This allows MSBuild to know the correct configuration for project references,
			// which is necessary to resolve the referenced project's OutputPath.
			projectOptions.Properties["CurrentSolutionConfigurationContents"] = solutionConfigurationXml.ToString();
			
			solution.AddMSBuildSolutionProperties(projectOptions.Properties);
			
			return projectOptions;
		}
		#endregion
		
		#region Loading
		protected bool isLoading;
		
		/// <summary>
		/// Set compilation properties (MSBuildProperties and AddInTree/AdditionalPropertiesPath).
		/// </summary>
		internal static void InitializeMSBuildProjectProperties(IDictionary<string, string> globalProperties)
		{
			foreach (KeyValuePair<string, string> entry in MSBuildEngine.MSBuildProperties) {
				globalProperties[entry.Key] = entry.Value;
			}
			// re-load these properties from AddInTree every time because "text" might contain
			// SharpDevelop properties resolved by the StringParser (e.g. ${property:FxCopPath})
			AddInTreeNode node = AddInTree.GetTreeNode(MSBuildEngine.AdditionalPropertiesPath, false);
			if (node != null) {
				foreach (Codon codon in node.Codons) {
					object item = node.BuildChildItem(codon, null);
					if (item != null) {
						string text = item.ToString();
						globalProperties[codon.Id] = text;
					}
				}
			}
		}
		
		public MSBuildBasedProject(ProjectLoadInformation loadInformation)
		{
			if (loadInformation == null)
				throw new ArgumentNullException("loadInformation");
			this.Name = loadInformation.ProjectName;
			isLoading = true;
			try {
				try {
					LoadProjectInternal(loadInformation);
				} catch (InvalidProjectFileException ex) {
					if (!(ex.ErrorCode == "MSB4132" && UpgradeToolsVersion(loadInformation))) {
						throw;
					}
				}
			} catch (InvalidProjectFileException ex) {
				LoggingService.Warn(ex);
				LoggingService.Warn("ErrorCode = " + ex.ErrorCode);
				Dispose();
				throw new ProjectLoadException(ex.Message, ex);
			} finally {
				isLoading = false;
			}
		}
		
		const string autoUpgradeNewToolsVersion = "4.0";
		
		bool UpgradeToolsVersion(ProjectLoadInformation loadInformation)
		{
			if (loadInformation.upgradeToolsVersion != null)
				return false;
			if (!CanUpgradeToolsVersion())
				return false;
			loadInformation.ProgressMonitor.ShowingDialog = true;
			StringTagPair[] tags = {
				new StringTagPair("ProjectName", loadInformation.ProjectName),
				new StringTagPair("OldToolsVersion", projectFile.ToolsVersion),
				new StringTagPair("NewToolsVersion", autoUpgradeNewToolsVersion)
			};
			string message = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Project.UpgradeView.UpdateOnLoadDueToMissingMSBuild}", tags);
			string upgradeButton = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Project.UpgradeView.UpdateToMSBuildButton}", tags);
			int result = MessageService.ShowCustomDialog(
				"${res:ICSharpCode.SharpDevelop.Project.UpgradeView.Title}",
				message,
				0, 1, upgradeButton, "${res:Global.CancelButtonText}");
			loadInformation.ProgressMonitor.ShowingDialog = false;
			if (result == 0) {
				loadInformation.upgradeToolsVersion = true;
				LoadProjectInternal(loadInformation);
				return true;
			} else {
				loadInformation.upgradeToolsVersion = false;
				return false;
			}
		}
		
		bool CanUpgradeToolsVersion()
		{
			if (projectFile == null)
				return false;
			if (string.IsNullOrEmpty(projectFile.ToolsVersion))
				return true;
			return projectFile.ToolsVersion == "2.0" || projectFile.ToolsVersion == "3.5";
		}
		
		void LoadProjectInternal(ProjectLoadInformation loadInformation)
		{
			this.projectCollection = loadInformation.ParentSolution.MSBuildProjectCollection;
			this.FileName = loadInformation.FileName;
			this.ActivePlatform = loadInformation.Platform;
			
			projectFile = ProjectRootElement.Open(loadInformation.FileName, projectCollection);
			if (loadInformation.upgradeToolsVersion == true && CanUpgradeToolsVersion()) {
				projectFile.ToolsVersion = autoUpgradeNewToolsVersion;
			}
			
			string userFileName = loadInformation.FileName + ".user";
			if (File.Exists(userFileName)) {
				try {
					userProjectFile = ProjectRootElement.Open(userFileName, projectCollection);
				} catch (InvalidProjectFileException ex) {
					throw new ProjectLoadException("Error loading user part " + userFileName + ":\n" + ex.Message, ex);
				}
			} else {
				userProjectFile = ProjectRootElement.Create(userFileName, projectCollection);
			}
			
			this.ActiveConfiguration = GetEvaluatedProperty("Configuration") ?? this.ActiveConfiguration;
			this.ActivePlatform = GetEvaluatedProperty("Platform") ?? this.ActivePlatform;
			
			CreateItemsListFromMSBuild();
			LoadConfigurationPlatformNamesFromMSBuild();
			
			base.IdGuid = GetEvaluatedProperty(ProjectGuidPropertyName);
		}
		
		[Browsable(false)]
		public override string IdGuid {
			get { return base.IdGuid; }
			set {
				if (base.IdGuid == null) {
					// Save the GUID in the project if the project does not yet have a GUID.
					SetPropertyInternal(null, null, ProjectGuidPropertyName, value, PropertyStorageLocations.Base, true);
				}
				base.IdGuid = value;
			}
		}
		#endregion
		
		#region Saving
		public override void Save(string fileName)
		{
			lock (SyncRoot) {
				watcher.Disable();
				watcher.Rename(fileName);
				// we need the global lock - if the file is being renamed,
				// MSBuild will update the global project collection
				lock (MSBuildInternals.SolutionProjectCollectionLock) {
					projectFile.Save(fileName);
					//bool userProjectDirty = userProjectFile.HasUnsavedChanges;
					string userFile = fileName + ".user";
					if (File.Exists(userFile) || userProjectFile.Count > 0) {
						userProjectFile.Save(userFile);
					}
				}
				watcher.Enable();
			}
			FileUtility.RaiseFileSaved(new FileNameEventArgs(fileName));
		}
		#endregion
		
		#region Active Configuration / Platform changed
		protected override void OnActiveConfigurationChanged(EventArgs e)
		{
			if (!isLoading) {
				lock (SyncRoot) {
					UnloadCurrentlyOpenProject();
					CreateItemsListFromMSBuild();
				}
			}
			base.OnActiveConfigurationChanged(e);
		}
		
		protected override void OnActivePlatformChanged(EventArgs e)
		{
			if (!isLoading) {
				lock (SyncRoot) {
					UnloadCurrentlyOpenProject();
					CreateItemsListFromMSBuild();
				}
			}
			base.OnActivePlatformChanged(e);
		}
		#endregion
		
		#region GetConfigurationNames / GetPlatformNames
		ICollection<string> configurationNames, platformNames;
		
		public override ICollection<string> ConfigurationNames {
			get {
				lock (SyncRoot) {
					if (configurationNames == null) {
						LoadConfigurationPlatformNamesFromMSBuild();
					}
					return configurationNames;
				}
			}
		}
		
		public override ICollection<string> PlatformNames {
			get {
				lock (SyncRoot) {
					if (platformNames == null) {
						LoadConfigurationPlatformNamesFromMSBuild();
					}
					return platformNames;
				}
			}
		}
		
		protected void InvalidateConfigurationPlatformNames()
		{
			lock (SyncRoot) {
				configurationNames = null;
				platformNames = null;
			}
		}
		
		/// <summary>
		/// Load available configurations and platforms from the project file
		/// by looking at which conditions are used.
		/// </summary>
		void LoadConfigurationPlatformNamesFromMSBuild()
		{
			ISet<string> configurationNames = new SortedSet<string>();
			ISet<string> platformNames = new SortedSet<string>();
			
			LoadConfigurationPlatformNamesFromMSBuildInternal(projectFile, configurationNames, platformNames);
			LoadConfigurationPlatformNamesFromMSBuildInternal(userProjectFile, configurationNames, platformNames);
			
			if (configurationNames.Count == 0) {
				configurationNames.Add("Debug");
				configurationNames.Add("Release");
			}
			if (platformNames.Count == 0) {
				platformNames.Add("AnyCPU");
			}
			
			this.configurationNames = configurationNames.AsReadOnly();
			this.platformNames      = platformNames.AsReadOnly();
		}

		static void LoadConfigurationPlatformNamesFromMSBuildInternal(
			ProjectRootElement project,
			ICollection<string> configurationNames, ICollection<string> platformNames)
		{
			foreach (var g in project.PropertyGroups) {
				if (string.IsNullOrEmpty(g.Condition)) {
					var prop = g.Properties.FirstOrDefault(p => MSBuildInternals.PropertyNameComparer.Equals(p.Name, "Configuration"));
					if (prop != null && !string.IsNullOrEmpty(prop.Value)) {
						configurationNames.Add(prop.Value);
					}
					prop = g.Properties.FirstOrDefault(p => MSBuildInternals.PropertyNameComparer.Equals(p.Name, "Platform"));
					if (prop != null && !string.IsNullOrEmpty(prop.Value)) {
						platformNames.Add(prop.Value);
					}
				} else {
					string gConfiguration, gPlatform;
					MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition, out gConfiguration, out gPlatform);
					if (gConfiguration != null) {
						configurationNames.Add(gConfiguration);
					}
					if (gPlatform != null) {
						platformNames.Add(gPlatform);
					}
				}
			}
		}
		#endregion
		
		#region IProjectAllowChangeConfigurations interface implementation
		bool IProjectAllowChangeConfigurations.RenameProjectConfiguration(string oldName, string newName)
		{
			lock (SyncRoot) {
				foreach (ProjectPropertyGroupElement g in projectFile.PropertyGroups.Concat(userProjectFile.PropertyGroups)) {
					// Rename the default configuration setting
					var prop = g.Properties.FirstOrDefault(p => p.Name == "Configuration");
					if (prop != null && prop.Value == oldName) {
						prop.Value = newName;
					}
					
					// Rename the configuration in conditions
					string gConfiguration, gPlatform;
					MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
					                                                          out gConfiguration,
					                                                          out gPlatform);
					if (gConfiguration == oldName) {
						g.Condition = CreateCondition(newName, gPlatform);
					}
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		
		bool IProjectAllowChangeConfigurations.RenameProjectPlatform(string oldName, string newName)
		{
			lock (SyncRoot) {
				foreach (ProjectPropertyGroupElement g in projectFile.PropertyGroups.Concat(userProjectFile.PropertyGroups)) {
					// Rename the default platform setting
					var prop = g.Properties.FirstOrDefault(p => p.Name == "Platform");
					if (prop != null && prop.Value == oldName) {
						prop.Value = newName;
					}
					
					// Rename the platform in conditions
					string gConfiguration, gPlatform;
					MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
					                                                          out gConfiguration,
					                                                          out gPlatform);
					if (gPlatform == oldName) {
						g.Condition = CreateCondition(gConfiguration, newName);
					}
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		
		bool IProjectAllowChangeConfigurations.AddProjectConfiguration(string newName, string copyFrom)
		{
			lock (SyncRoot) {
				bool copiedGroupInMainFile = false;
				if (copyFrom != null) {
					foreach (ProjectPropertyGroupElement g in projectFile.PropertyGroups.ToList()) {
						string gConfiguration, gPlatform;
						MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
						                                                          out gConfiguration,
						                                                          out gPlatform);
						if (gConfiguration == copyFrom) {
							CopyProperties(projectFile, g, newName, gPlatform);
							copiedGroupInMainFile = true;
						}
					}
					foreach (ProjectPropertyGroupElement g in userProjectFile.PropertyGroups.ToList()) {
						string gConfiguration, gPlatform;
						MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
						                                                          out gConfiguration,
						                                                          out gPlatform);
						if (gConfiguration == copyFrom) {
							CopyProperties(userProjectFile, g, newName, gPlatform);
						}
					}
				}
				if (!copiedGroupInMainFile) {
					projectFile.AddPropertyGroup().Condition = CreateCondition(newName, null);
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		
		bool IProjectAllowChangeConfigurations.AddProjectPlatform(string newName, string copyFrom)
		{
			lock (SyncRoot) {
				bool copiedGroupInMainFile = false;
				if (copyFrom != null) {
					foreach (ProjectPropertyGroupElement g in projectFile.PropertyGroups.ToList()) {
						string gConfiguration, gPlatform;
						MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
						                                                          out gConfiguration,
						                                                          out gPlatform);
						if (gPlatform == copyFrom) {
							CopyProperties(projectFile, g, gConfiguration, newName);
							copiedGroupInMainFile = true;
						}
					}
					foreach (ProjectPropertyGroupElement g in userProjectFile.PropertyGroups.ToList()) {
						string gConfiguration, gPlatform;
						MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
						                                                          out gConfiguration,
						                                                          out gPlatform);
						if (gPlatform == copyFrom) {
							CopyProperties(userProjectFile, g, gConfiguration, newName);
						}
					}
				}
				if (!copiedGroupInMainFile) {
					projectFile.AddPropertyGroup().Condition = CreateCondition(null, newName);
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		
		/// <summary>
		/// copy properties from g into a new property group for newConfiguration and newPlatform
		/// </summary>
		void CopyProperties(ProjectRootElement project, ProjectPropertyGroupElement g, string newConfiguration, string newPlatform)
		{
			ProjectPropertyGroupElement ng = project.AddPropertyGroup();
			ng.Condition = CreateCondition(newConfiguration, newPlatform);
			foreach (var p in g.Properties) {
				ng.AddProperty(p.Name, p.Value).Condition = p.Condition;
			}
		}
		
		bool IProjectAllowChangeConfigurations.RemoveProjectConfiguration(string name)
		{
			lock (SyncRoot) {
				string otherConfigurationName = null;
				foreach (string configName in this.ConfigurationNames) {
					if (configName != name) {
						otherConfigurationName = name;
						break;
					}
				}
				if (otherConfigurationName == null) {
					throw new InvalidOperationException("cannot remove the last configuration");
				}
				foreach (ProjectPropertyGroupElement g in projectFile.PropertyGroups.Concat(userProjectFile.PropertyGroups).ToList()) {
					ProjectPropertyElement prop = g.Properties.FirstOrDefault(p => p.Name == "Configuration");
					if (prop != null && prop.Value == name) {
						prop.Value = otherConfigurationName;
					}
					
					string gConfiguration, gPlatform;
					MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
					                                                          out gConfiguration,
					                                                          out gPlatform);
					if (gConfiguration == name) {
						g.Parent.RemoveChild(g);
					}
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		
		bool IProjectAllowChangeConfigurations.RemoveProjectPlatform(string name)
		{
			lock (SyncRoot) {
				string otherPlatformName = null;
				foreach (string platformName in this.PlatformNames) {
					if (platformName != name) {
						otherPlatformName = name;
						break;
					}
				}
				if (otherPlatformName == null) {
					throw new InvalidOperationException("cannot remove the last platform");
				}
				foreach (ProjectPropertyGroupElement g in projectFile.PropertyGroups.Concat(userProjectFile.PropertyGroups).ToList()) {
					ProjectPropertyElement prop = g.Properties.FirstOrDefault(p => p.Name == "Platform");
					if (prop != null && prop.Value == name) {
						prop.Value = otherPlatformName;
					}
					
					string gConfiguration, gPlatform;
					MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
					                                                          out gConfiguration,
					                                                          out gPlatform);
					if (gPlatform == name) {
						g.Parent.RemoveChild(g);
					}
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		#endregion
		
		#region ProjectExtensions
		public override bool ContainsProjectExtension(string name)
		{
			ProjectExtensionsElement projectExtensions = GetExistingProjectExtensionsElement();
			if (projectExtensions != null) {
				return projectExtensions[name] != null;
			}
			return false;
		}

		public override void SaveProjectExtensions(string name, XElement element)
		{
			lock (SyncRoot) {
				ProjectExtensionsElement existing = GetExistingProjectExtensionsElement();
				if (existing == null) {
					existing = projectFile.CreateProjectExtensionsElement();
					projectFile.AppendChild(existing);
				}
				existing[name] = FormatProjectExtension(element);
			}
		}
		
		public override XElement LoadProjectExtensions(string name)
		{
			lock (SyncRoot) {
				ProjectExtensionsElement existing = GetExistingProjectExtensionsElement();
				if (existing == null) {
					existing = projectFile.CreateProjectExtensionsElement();
					return new XElement(name);
				}
				return XElement.Parse(existing[name]);
			}
		}
		
		ProjectExtensionsElement GetExistingProjectExtensionsElement()
		{
			return projectFile.Children.OfType<ProjectExtensionsElement>().FirstOrDefault();
		}
		
		string FormatProjectExtension(XElement element)
		{
			var settings = new XmlWriterSettings {
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n      ",
				NewLineHandling = NewLineHandling.Replace,
				OmitXmlDeclaration = true
			};
			var formattedText = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(new StringWriter(formattedText), settings)) {
				element.WriteTo(writer);
			}
			return formattedText.ToString();
		}
		#endregion
		
		#region HasProjectType
		public override bool HasProjectType(Guid projectTypeGuid)
		{
			string guidList = GetEvaluatedProperty("ProjectTypeGuids");
			if (string.IsNullOrEmpty(guidList))
				return base.HasProjectType(projectTypeGuid);
			foreach (string guid in guidList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)) {
				Guid result;
				if (Guid.TryParse(guid, out result) && projectTypeGuid == result)
					return true;
			}
			return false;
		}
		
		public void AddProjectType(Guid projectTypeGuid)
		{
			string guidList = GetEvaluatedProperty("ProjectTypeGuids");
			if (string.IsNullOrEmpty(guidList))
				guidList = this.TypeGuid;
			SetProperty("ProjectTypeGuids", guidList + ";" + projectTypeGuid.ToString("B").ToUpperInvariant(), false);
			InvalidateBehavior();
		}
		
		public void RemoveProjectType(Guid projectTypeGuid)
		{
			string guidList = GetEvaluatedProperty("ProjectTypeGuids");
			if (!string.IsNullOrEmpty(guidList)) {
				List<string> remainingGuids = new List<string>();
				foreach (string guid in guidList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)) {
					Guid result;
					if (!(Guid.TryParse(guid, out result) && projectTypeGuid == result))
						remainingGuids.Add(guid);
				}
				SetProperty("ProjectTypeGuids", string.Join(";", remainingGuids), false);
				InvalidateBehavior();
			}
		}
		#endregion
	}
}
