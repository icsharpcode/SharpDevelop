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
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using Microsoft.Build.Construction;
using MSBuild = Microsoft.Build.Evaluation;
using StringPair = ICSharpCode.SharpDevelop.Pair<string, string>;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project that is based on an MSBuild project file.
	/// 
	/// Thread-safety: most members are thread-safe, but direct accesses on the underlying MSBuildProject
	/// require locking on the SyncRoot. Methods that return underlying MSBuild objects require that
	/// the caller locks on the SyncRoot.
	/// </summary>
	public class MSBuildBasedProject : AbstractProject, IProjectItemListProvider
	{
		readonly MSBuild.ProjectCollection projectCollection;
		
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
		protected readonly Set<string> saveAfterImportsProperties = new Set<string>(
			"PostBuildEvent",
			"PreBuildEvent"
		);
		
		public MSBuildBasedProject()
		{
			projectCollection = new MSBuild.ProjectCollection();
			this.projectFile = ProjectRootElement.Create(projectCollection);
			this.userProjectFile = ProjectRootElement.Create(projectCollection);
		}
		
		public override void Dispose()
		{
			base.Dispose();
			// unload evaluatingTempProject if necessary:
			//MSBuildInternals.EnsureCorrectTempProject(project, null, null, ref evaluatingTempProject);
			// unload project + userProject:
			projectCollection.UnloadAllProjects();
		}
		
		public override int MinimumSolutionVersion {
			get {
				lock (SyncRoot) {
					if (string.IsNullOrEmpty(projectFile.ToolsVersion) || projectFile.ToolsVersion == "2.0")
					{
						return Solution.SolutionVersionVS2005;
					} else if (projectFile.ToolsVersion == "3.0" || projectFile.ToolsVersion == "3.5") {
						return Solution.SolutionVersionVS2008;
					} else {
						return Solution.SolutionVersionVS2010;
					}
				}
			}
		}
		
		public virtual void ConvertToMSBuild40(bool changeTargetFrameworkToNet40)
		{
			lock (SyncRoot) {
				projectFile.ToolsVersion = "4.0";
				userProjectFile.ToolsVersion = "4.0";
				Reevaluate();
			}
		}
		
		void Reevaluate()
		{
			// ?
		}
		
		public override void ResolveAssemblyReferences()
		{
			MSBuildInternals.ResolveAssemblyReferences(this, null);
		}
		
		#region CreateProjectItem
		/// <summary>
		/// Creates a new projectItem for the passed itemType
		/// </summary>
		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			switch (item.ItemType.ItemName) {
				case "Reference":
					return new ReferenceProjectItem(this, item);
				case "ProjectReference":
					return new ProjectReferenceProjectItem(this, item);
				case "COMReference":
					return new ComReferenceProjectItem(this, item);
				case "Import":
					return new ImportProjectItem(this, item);
					
				case "None":
				case "Compile":
				case "EmbeddedResource":
				case "Resource":
				case "Content":
				case "Folder":
					return new FileProjectItem(this, item);
					
				case "WebReferenceUrl":
					return new WebReferenceUrl(this, item);
					
				case "WebReferences":
					return new WebReferencesProjectItem(this, item);
					
				default:
					if (this.AvailableFileItemTypes.Contains(item.ItemType)
					    || SafeFileExists(this.Directory, item.EvaluatedInclude))
					{
						return new FileProjectItem(this, item);
					} else {
						return base.CreateProjectItem(item);
					}
			}
		}
		
		static bool SafeFileExists(string directory, string fileName)
		{
			try {
				return File.Exists(Path.Combine(directory, fileName));
			} catch (Exception) {
				return false;
			}
		}
		#endregion
		
		#region Create new project
		protected virtual void Create(ProjectCreateInformation information)
		{
			InitializeMSBuildProjectProperties(projectCollection);
			
			Name = information.ProjectName;
			FileName = information.OutputProjectFileName;
			
			projectFile.FullPath = information.OutputProjectFileName;
			projectFile.ToolsVersion = "4.0";
			projectFile.DefaultTargets = "Build";
			
			base.IdGuid = "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
			projectFile.AddProperty(ProjectGuidPropertyName, IdGuid);
			AddGuardedProperty("Configuration", "Debug");
			AddGuardedProperty("Platform", "AnyCPU");
			
			this.ActiveConfiguration = "Debug";
			this.ActivePlatform = "AnyCPU";
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
			lock (SyncRoot) {
				projectFile.AddProperty(name, value).Condition = " '$(" + name + ")' == '' ";
			}
		}
		
		/// <summary>
		/// Adds an MSBuild import to the project, refreshes the list of available item names
		/// and recreates the project items.
		/// </summary>
		protected void AddImport(string importedProjectFile, string condition)
		{
			lock (SyncRoot) {
				projectFile.AddImport(importedProjectFile).Condition = condition;
				Reevaluate();
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
				var prop = c.GetNonImportedProperty(propertyName);
				if (prop != null) {
					location = c.GetLocation(prop);
					return prop.EvaluatedValue;
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
				var prop = c.GetNonImportedProperty(propertyName);
				if (prop != null) {
					location = c.GetLocation(prop);
					return prop.UnevaluatedValue;
				} else {
					location = PropertyStorageLocations.Unknown;
					return null;
				}
			}
		}
		
		ConfiguredProject OpenCurrentConfiguration()
		{
			return OpenConfiguration(null, null);
		}
		
		ConfiguredProject OpenConfiguration(string configuration, string platform)
		{
			bool lockTaken = false;
			try {
				System.Threading.Monitor.Enter(this.SyncRoot, ref lockTaken);
				
				if (configuration == null)
					configuration = this.ActiveConfiguration;
				if (platform == null)
					platform = this.ActivePlatform;
				Dictionary<string, string> globalProps = new Dictionary<string, string>();
				globalProps["Configuration"] = configuration;
				globalProps["Platform"] = platform;
				string toolsVersion = projectFile.ToolsVersion;
				if (string.IsNullOrEmpty(toolsVersion))
					toolsVersion = projectCollection.DefaultToolsVersion;
				var project = new MSBuild.Project(projectFile, globalProps, toolsVersion, projectCollection);
				return new ConfiguredProject(this, project);
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
			public readonly MSBuild.Project Project;
			
			public ConfiguredProject(MSBuildBasedProject parent, MSBuild.Project project)
			{
				this.p = parent;
				this.Project = project;
			}
			
			public MSBuild.ProjectProperty GetNonImportedProperty(string name)
			{
				var prop = Project.GetProperty(name);
				if (prop != null && prop.Xml != null) {
					if (prop.Xml.ContainingProject == p.projectFile || prop.Xml.ContainingProject == p.userProjectFile)
						return prop;
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
				p.projectCollection.UnloadProject(this.Project);
				System.Threading.Monitor.Exit(p.SyncRoot);
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
				var prop = c.GetNonImportedProperty(propertyName);
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
			// first try main project file, then try user project file
			ProjectPropertyElement p = GetAnyUnevaluatedProperty(projectFile, configuration, platform, propertyName);
			if (p == null)
				p = GetAnyUnevaluatedProperty(userProjectFile, configuration, platform, propertyName);
			return p != null ? p.Value : null;
		}
		
		static ProjectPropertyElement GetAnyUnevaluatedProperty(ProjectRootElement project, string configuration, string platform, string propertyName)
		{
			foreach (var g in project.PropertyGroups) {
				var property = g.Properties.FirstOrDefault(p => p.Name == propertyName);
				if (property == null)
					continue;
				string gConfiguration, gPlatform;
				MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
				                                                          out gConfiguration,
				                                                          out gPlatform);
				if ((configuration == null || configuration == gConfiguration || gConfiguration == null)
				    && (platform == null || platform == gPlatform || gPlatform == null))
				{
					return property;
				}
			}
			return null;
		}
		
		/*
		/// <summary>
		/// Get all instances of the specified property.
		/// 
		/// Warning: you need to lock(project.SyncRoot) around calls to GetAllProperties
		/// until you no longer need to access the BuildProperty objects!
		/// </summary>
		public IList<MSBuild.BuildProperty> GetAllProperties(string propertyName)
		{
			List<MSBuild.BuildProperty> l = new List<MSBuild.BuildProperty>();
			foreach (MSBuild.BuildPropertyGroup g in project.PropertyGroups) {
				if (g.IsImported) continue;
				MSBuild.BuildProperty property = MSBuildInternals.GetProperty(g, propertyName);
				if (property != null) {
					l.Add(property);
				}
			}
			foreach (MSBuild.BuildPropertyGroup g in userProject.PropertyGroups) {
				if (g.IsImported) continue;
				MSBuild.BuildProperty property = MSBuildInternals.GetProperty(g, propertyName);
				if (property != null) {
					l.Add(property);
				}
			}
			return l;
		}
		*/
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
				if (g.Properties.Any(p => p.Name == propertyName)) {
					return MSBuildInternals.GetLocationFromCondition(g.Condition);
				}
			}
			foreach (var g in userProjectFile.PropertyGroups) {
				if (g.Properties.Any(p => p.Name == propertyName)) {
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
								                   CreateCondition(pair.Key.First, pair.Key.Second, location),
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
			foreach (var propertyGroup in targetProject.PropertyGroups) {
				if (propertyGroup.Condition == groupCondition) {
					foreach (var property in propertyGroup.Properties.ToList()) {
						if (property.Name == propertyName) {
							property.Value = newValue;
							return;
						}
					}
				}
			}
			foreach (var propertyGroup in targetProject.PropertyGroups) {
				if (propertyGroup.Condition == groupCondition) {
					propertyGroup.AddProperty(propertyName, newValue);
				}
			}
			var newGroup = targetProject.AddPropertyGroup();
			newGroup.Condition = groupCondition;
			newGroup.AddProperty(propertyName, newValue);
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
					if (property.Name == propertyName) {
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
		static string CreateCondition(string configuration, string platform)
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
		static string CreateCondition(string configuration, string platform, PropertyStorageLocations location)
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
				
				Set<ItemType> availableFileItemTypes = new Set<ItemType>();
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
			
			WorkbenchSingleton.AssertMainThread();
			throw new NotImplementedException();
			/*
			lock (SyncRoot) {
				if (items.Remove(item)) {
					itemsReadOnly = null; // remove readonly variant of item list - will regenerate on next Items call
					projectFile.RemoveItem(item.BuildItem);
					item.BuildItem = null; // make the item free again
					return true;
				} else {
					throw new InvalidOperationException("Expected that the item is added to this project!");
				}
			}*/
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
		
		/*
		internal static void RunMSBuild(Solution solution, IProject project,
		                                string configuration, string platform, BuildOptions options)
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			oldMSBuildEngine engine = new oldMSBuildEngine();
			engine.Configuration = configuration;
			engine.Platform = platform;
			engine.MessageView = TaskService.BuildMessageViewCategory;
			engine.Run(solution, project, options);
		}
		 */
		#endregion
		
		#region Loading
		protected bool isLoading;
		
		/// <summary>
		/// Set compilation properties (MSBuildProperties and AddInTree/AdditionalPropertiesPath).
		/// </summary>
		internal static void InitializeMSBuildProjectProperties(MSBuild.ProjectCollection collection)
		{
			Dictionary<string, string> props = new Dictionary<string, string>();
			InitializeMSBuildProjectProperties(props);
			foreach (var pair in props)
				collection.SetGlobalProperty(pair.Key, pair.Value);
		}
		
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
					object item = codon.BuildItem(null, new System.Collections.ArrayList());
					if (item != null) {
						string text = item.ToString();
						if (!codon.Properties.Get("text", "").Contains("$("))
							text = MSBuildInternals.Escape(text);
						globalProperties[codon.Id] = text;
					}
				}
			}
		}
		
		protected virtual void LoadProject(string fileName)
		{
			lock (SyncRoot) {
				isLoading = true;
				try {
					LoadProjectInternal(fileName);
				} finally {
					isLoading = false;
				}
			}
		}
		
		void LoadProjectInternal(string fileName)
		{
			this.FileName = fileName;
			
			InitializeMSBuildProjectProperties(projectCollection);
			
			//try {
				projectFile = ProjectRootElement.Open(fileName, projectCollection);
			/*} catch (MSBuild.InvalidProjectFileException ex) {
				LoggingService.Warn(ex);
				LoggingService.Warn("ErrorCode = " + ex.ErrorCode);
				bool isVS2003ProjectWithInvalidEncoding = false;
				if (ex.ErrorCode == "MSB4025") {
					// Invalid XML.
					// This MIGHT be a VS2003 project in default encoding, so we have to use this
					// ugly trick to detect old-style projects
					using (StreamReader r = File.OpenText(fileName)) {
						if (r.ReadLine() == "<VisualStudioProject>") {
							isVS2003ProjectWithInvalidEncoding = true;
						}
					}
				}
				if (ex.ErrorCode == "MSB4075" || isVS2003ProjectWithInvalidEncoding) {
					// MSB4075 is:
					// "The project file must be opened in VS IDE and converted to latest version
					// before it can be build by MSBuild."
					try {
						Converter.PrjxToSolutionProject.ConvertVSNetProject(fileName);
						projectFile.Load(fileName);
					} catch (System.Xml.XmlException ex2) {
						throw new ProjectLoadException(ex2.Message, ex2);
					} catch (MSBuild.InvalidProjectFileException ex2) {
						throw new ProjectLoadException(ex2.Message, ex2);
					}
				} else {
					throw new ProjectLoadException(ex.Message, ex);
				}
			}*/
			
			string userFileName = fileName + ".user";
			if (File.Exists(userFileName)) {
				//try {
					userProjectFile = ProjectRootElement.Open(userFileName, projectCollection);
				/*} catch (MSBuild.InvalidProjectFileException ex) {
					throw new ProjectLoadException("Error loading user part " + userFileName + ":\n" + ex.Message);
				}*/
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
				projectFile.Save(fileName);
				bool userProjectDirty = userProjectFile.HasUnsavedChanges;
				string userFile = fileName + ".user";
				if (File.Exists(userFile) || userProjectFile.Count > 0) {
					userProjectFile.Save(userFile);
				}
			}
			FileUtility.RaiseFileSaved(new FileNameEventArgs(fileName));
		}
		#endregion
		
		#region Active Configuration / Platform changed
		protected override void OnActiveConfigurationChanged(EventArgs e)
		{
			if (!isLoading) {
				lock (SyncRoot) {
					CreateItemsListFromMSBuild();
				}
			}
			base.OnActiveConfigurationChanged(e);
		}
		
		protected override void OnActivePlatformChanged(EventArgs e)
		{
			if (!isLoading) {
				lock (SyncRoot) {
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
		
		/// <summary>
		/// Load available configurations and platforms from the project file
		/// by looking at which conditions are used.
		/// </summary>
		void LoadConfigurationPlatformNamesFromMSBuild()
		{
			Set<string> configurationNames = new Set<string>();
			Set<string> platformNames = new Set<string>();
			
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
			Set<string> configurationNames, Set<string> platformNames)
		{
			foreach (var g in project.PropertyGroups) {
				if (string.IsNullOrEmpty(g.Condition)) {
					var prop = g.Properties.FirstOrDefault(p => p.Name == "Configuration");
					if (prop != null && !string.IsNullOrEmpty(prop.Value)) {
						configurationNames.Add(prop.Value);
					}
					prop = g.Properties.FirstOrDefault(p => p.Name == "Platform");
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
		/*
		bool IProjectAllowChangeConfigurations.RenameProjectConfiguration(string oldName, string newName)
		{
			lock (SyncRoot) {
				foreach (MSBuild.BuildPropertyGroup g in project.PropertyGroups) {
					if (g.IsImported) {
						continue;
					}
					MSBuild.BuildProperty prop = MSBuildInternals.GetProperty(g, "Configuration");
					if (prop != null && prop.Value == oldName) {
						prop.Value = newName;
					}
					
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
				foreach (MSBuild.BuildPropertyGroup g in project.PropertyGroups) {
					if (g.IsImported) {
						continue;
					}
					MSBuild.BuildProperty prop = MSBuildInternals.GetProperty(g, "Platform");
					if (prop != null && prop.Value == oldName) {
						prop.Value = newName;
					}
					
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
				bool copiedGroup = false;
				if (copyFrom != null) {
					foreach (MSBuild.BuildPropertyGroup g
					         in project.PropertyGroups.Cast<MSBuild.BuildPropertyGroup>().ToList())
					{
						if (g.IsImported) {
							continue;
						}
						
						string gConfiguration, gPlatform;
						MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
						                                                          out gConfiguration,
						                                                          out gPlatform);
						if (gConfiguration == copyFrom) {
							CopyProperties(g, newName, gPlatform);
							copiedGroup = true;
						}
					}
				}
				if (!copiedGroup) {
					project.AddNewPropertyGroup(false).Condition = CreateCondition(newName, null);
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		
		bool IProjectAllowChangeConfigurations.AddProjectPlatform(string newName, string copyFrom)
		{
			lock (SyncRoot) {
				bool copiedGroup = false;
				if (copyFrom != null) {
					foreach (MSBuild.BuildPropertyGroup g
					         in project.PropertyGroups.Cast<MSBuild.BuildPropertyGroup>().ToList())
					{
						if (g.IsImported) {
							continue;
						}
						
						string gConfiguration, gPlatform;
						MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
						                                                          out gConfiguration,
						                                                          out gPlatform);
						if (gPlatform == copyFrom) {
							CopyProperties(g, gConfiguration, newName);
							copiedGroup = true;
						}
					}
				}
				if (!copiedGroup) {
					project.AddNewPropertyGroup(false).Condition = CreateCondition(null, newName);
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		
		/// <summary>
		/// copy properties from g into a new property group for newConfiguration and newPlatform
		/// </summary>
		void CopyProperties(MSBuild.BuildPropertyGroup g, string newConfiguration, string newPlatform)
		{
			MSBuild.BuildPropertyGroup ng = project.AddNewPropertyGroup(false);
			ng.Condition = CreateCondition(newConfiguration, newPlatform);
			foreach (MSBuild.BuildProperty p in g) {
				ng.AddNewProperty(p.Name, p.Value);
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
				foreach (MSBuild.BuildPropertyGroup g
				         in project.PropertyGroups.Cast<MSBuild.BuildPropertyGroup>().ToList())
				{
					if (g.IsImported) {
						continue;
					}
					
					MSBuild.BuildProperty prop = MSBuildInternals.GetProperty(g, "Configuration");
					if (prop != null && prop.Value == name) {
						prop.Value = otherConfigurationName;
					}
					
					string gConfiguration, gPlatform;
					MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
					                                                          out gConfiguration,
					                                                          out gPlatform);
					if (gConfiguration == name) {
						project.RemovePropertyGroup(g);
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
				foreach (MSBuild.BuildPropertyGroup g
				         in project.PropertyGroups.Cast<MSBuild.BuildPropertyGroup>().ToList())
				{
					if (g.IsImported) {
						continue;
					}
					
					MSBuild.BuildProperty prop = MSBuildInternals.GetProperty(g, "Platform");
					if (prop != null && prop.Value == name) {
						prop.Value = otherPlatformName;
					}
					
					string gConfiguration, gPlatform;
					MSBuildInternals.GetConfigurationAndPlatformFromCondition(g.Condition,
					                                                          out gConfiguration,
					                                                          out gPlatform);
					if (gPlatform == name) {
						project.RemovePropertyGroup(g);
					}
				}
				LoadConfigurationPlatformNamesFromMSBuild();
				return true;
			}
		}
		*/
		#endregion
	}
}
