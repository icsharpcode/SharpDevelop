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
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using MSBuild = Microsoft.Build.BuildEngine;
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
	public class MSBuildBasedProject : AbstractProject, IProjectItemListProvider, IProjectAllowChangeConfigurations
	{
		/// <summary>
		/// The underlying MSBuild project.
		/// </summary>
		MSBuild.Project project;
		
		/// <summary>
		/// The '.user' part of the project.
		/// </summary>
		MSBuild.Project userProject;
		
		/// <summary>
		/// A list of project properties that are saved after the normal properties.
		/// Use this for properties that could reference other properties, e.g.
		/// PostBuildEvent references OutputPath.
		/// </summary>
		protected readonly Set<string> saveAfterImportsProperties = new Set<string>(
			"PostBuildEvent",
			"PreBuildEvent"
		);
		
		public MSBuildBasedProject(MSBuild.Engine engine)
		{
			if (engine == null)
				throw new ArgumentNullException("engine");
			this.project = engine.CreateNewProject();
			this.userProject = engine.CreateNewProject();
		}
		
		/// <summary>
		/// Gets the underlying MSBuild project.
		/// </summary>
		[Browsable(false)]
		public MSBuild.Project MSBuildProject {
			get { return project; }
		}
		
		public override void Dispose()
		{
			base.Dispose();
			// unload evaluatingTempProject if necessary:
			MSBuildInternals.EnsureCorrectTempProject(project, null, null, ref evaluatingTempProject);
			// unload project + userProject:
			project.ParentEngine.UnloadProject(project);
			userProject.ParentEngine.UnloadProject(userProject);
		}
		
		public override int MinimumSolutionVersion {
			get {
				lock (SyncRoot) {
					if (string.IsNullOrEmpty(project.DefaultToolsVersion)
					    || project.DefaultToolsVersion == "2.0")
					{
						return Solution.SolutionVersionVS2005;
					} else {
						return Solution.SolutionVersionVS2008;
					}
				}
			}
		}
		
		public virtual void ConvertToMSBuild35(bool changeTargetFrameworkToNet35)
		{
			lock (SyncRoot) {
				project.DefaultToolsVersion = "3.5";
			}
		}
		
		public override void ResolveAssemblyReferences()
		{
			MSBuildInternals.ResolveAssemblyReferences(this, null);
		}
		
		#region CreateProjectItem
		/// <summary>
		/// Creates a new projectItem for the passed itemType
		/// </summary>
		public override ProjectItem CreateProjectItem(MSBuild.BuildItem item)
		{
			switch (item.Name) {
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
					if (this.AvailableFileItemTypes.Contains(new ItemType(item.Name))
					    || SafeFileExists(this.Directory, item.FinalItemSpec))
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
			InitializeMSBuildProject(project);
			
			Name = information.ProjectName;
			FileName = information.OutputProjectFileName;
			
			project.FullFileName = information.OutputProjectFileName;
			project.DefaultToolsVersion = "3.5";
			
			base.IdGuid = "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
			MSBuild.BuildPropertyGroup group = project.AddNewPropertyGroup(false);
			group.AddNewProperty(ProjectGuidPropertyName, IdGuid, true);
			group.AddNewProperty("Configuration", "Debug", true).Condition = " '$(Configuration)' == '' ";
			group.AddNewProperty("Platform", "x86", true).Condition = " '$(Platform)' == '' ";
			
			this.ActiveConfiguration = "Debug";
			this.ActivePlatform = "x86";
			SetProperty(null, "x86", "PlatformTarget", "x86", PropertyStorageLocations.PlatformSpecific, false);
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
		protected void AddGuardedProperty(string name, string value, bool treatValueAsLiteral)
		{
			foreach (MSBuild.BuildPropertyGroup pg in project.PropertyGroups) {
				if (pg.IsImported)
					continue;
				if (string.IsNullOrEmpty(pg.Condition)) {
					pg.AddNewProperty(name, value, treatValueAsLiteral).Condition = " '$(" + name + ")' == '' ";
					return;
				}
			}
			MSBuild.BuildPropertyGroup newGroup = project.AddNewPropertyGroup(false);
			newGroup.AddNewProperty(name, value, treatValueAsLiteral).Condition = " '$(" + name + ")' == '' ";
		}
		
		/// <summary>
		/// Adds an MSBuild import to the project, refreshes the list of available item names
		/// and recreates the project items.
		/// </summary>
		protected void AddImport(string projectFile, string condition)
		{
			lock (SyncRoot) {
				project.AddNewImport(projectFile, condition);
				CreateItemsListFromMSBuild();
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
			lock (SyncRoot) {
				return project.GetEvaluatedProperty(propertyName);
			}
		}
		
		MSBuild.Project evaluatingTempProject;
		
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
			lock (SyncRoot) {
				MSBuild.BuildPropertyGroup group;
				MSBuild.BuildProperty prop = FindPropertyObject(configuration, platform, propertyName,
				                                                out group, out location);
				if (prop == null)
					return null;
				else
					return prop.FinalValue;
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
			lock (SyncRoot) {
				MSBuild.BuildPropertyGroup group;
				MSBuild.BuildProperty prop = FindPropertyObject(configuration, platform, propertyName,
				                                                out group, out location);
				if (prop == null)
					return null;
				else
					return prop.Value;
			}
		}
		
		/// <summary>
		/// Evaluates a MSBuild condition in this project.
		/// 
		/// WARNING: EvaluateMSBuildCondition might add a temporary property group to the project
		/// and remove it again, which invalidates enumerators over the list of property groups!
		/// </summary>
		/// <param name="configuration">The configuration to use for evaluating the condition</param>
		/// <param name="platform">The platform to use for evaluating the condition</param>
		/// <param name="condition">The MSBuild condition string to evaluate</param>
		/// <returns>The result of the condition</returns>
		protected bool EvaluateMSBuildCondition(string configuration, string platform,
		                                        string condition)
		{
			lock (SyncRoot) {
				return MSBuildInternals.EvaluateCondition(project, configuration, platform, condition,
				                                          ref evaluatingTempProject);
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
		protected MSBuild.BuildProperty FindPropertyObject(string configuration, string platform,
		                                                   string propertyName,
		                                                   out MSBuild.BuildPropertyGroup group,
		                                                   out PropertyStorageLocations location)
		{
			if (string.IsNullOrEmpty(configuration)) configuration = ActiveConfiguration;
			if (string.IsNullOrEmpty(platform))      platform = ActivePlatform;
			
			// First try main project file
			MSBuild.BuildProperty p = FindPropertyObjectInternal(project, configuration, platform, propertyName, out group);
			if (p != null) {
				location = MSBuildInternals.GetLocationFromCondition(group.Condition);
				return p;
			} else {
				// try user project file
				p = FindPropertyObjectInternal(userProject, configuration, platform, propertyName, out group);
				if (p != null) {
					location = PropertyStorageLocations.UserFile
						| MSBuildInternals.GetLocationFromCondition(group.Condition);
					return p;
				} else {
					location = PropertyStorageLocations.Unknown;
					group = null;
					return null;
				}
			}
		}
		
		MSBuild.BuildProperty FindPropertyObjectInternal(MSBuild.Project project,
		                                                 string configuration, string platform,
		                                                 string propertyName,
		                                                 out MSBuild.BuildPropertyGroup group)
		{
			// We need to use ToList because EvaluateMSBuildCondition invalidates the list
			// of property groups.
			foreach (MSBuild.BuildPropertyGroup g
			         in project.PropertyGroups.Cast<MSBuild.BuildPropertyGroup>().ToList())
			{
				if (g.IsImported) {
					continue;
				}
				MSBuild.BuildProperty property = MSBuildInternals.GetProperty(g, propertyName);
				if (property == null)
					continue;
				if (EvaluateMSBuildCondition(configuration, platform, g.Condition)) {
					group = g;
					return property;
				}
			}
			group = null;
			return null;
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
			MSBuild.BuildProperty p = GetAnyUnevaluatedProperty(project, configuration, platform, propertyName);
			if (p == null)
				p = GetAnyUnevaluatedProperty(userProject, configuration, platform, propertyName);
			return p != null ? p.Value : null;
		}
		
		static MSBuild.BuildProperty GetAnyUnevaluatedProperty(MSBuild.Project project, string configuration, string platform, string propertyName)
		{
			foreach (MSBuild.BuildPropertyGroup g in project.PropertyGroups) {
				if (g.IsImported) {
					continue;
				}
				MSBuild.BuildProperty property = MSBuildInternals.GetProperty(g, propertyName);
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
			foreach (MSBuild.BuildPropertyGroup g in project.PropertyGroups) {
				if (g.IsImported) continue;
				if (MSBuildInternals.GetProperty(g, propertyName) != null) {
					return MSBuildInternals.GetLocationFromCondition(g.Condition);
				}
			}
			foreach (MSBuild.BuildPropertyGroup g in userProject.PropertyGroups) {
				if (g.IsImported) continue;
				if (MSBuildInternals.GetProperty(g, propertyName) != null) {
					return PropertyStorageLocations.UserFile |
						MSBuildInternals.GetLocationFromCondition(g.Condition);
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
			MSBuild.BuildPropertyGroup existingPropertyGroup;
			MSBuild.BuildProperty existingProperty = FindPropertyObject(configuration, platform,
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
			MSBuild.PropertyPosition propertyInsertionPosition;
			if (saveAfterImportsProperties.Contains(propertyName)) {
				propertyInsertionPosition = MSBuild.PropertyPosition.UseExistingOrCreateAfterLastImport;
			} else {
				propertyInsertionPosition = MSBuild.PropertyPosition.UseExistingOrCreateAfterLastPropertyGroup;
			}
			// get the project file where the property should be stored
			MSBuild.Project targetProject;
			if ((location & PropertyStorageLocations.UserFile) == PropertyStorageLocations.UserFile)
				targetProject = userProject;
			else
				targetProject = project;
			
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
					existingPropertyGroup.RemoveProperty(existingProperty);
					
					if (existingPropertyGroup.Count == 0) {
						targetProject.RemovePropertyGroup(existingPropertyGroup);
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
		
		void MSBuildSetProperty(MSBuild.Project targetProject, string propertyName, string newValue,
		                        string groupCondition, MSBuild.PropertyPosition position,
		                        bool treatPropertyValueAsLiteral)
		{
			if (targetProject == project) {
				project.SetProperty(propertyName, newValue, groupCondition, position, treatPropertyValueAsLiteral);
			} else if (targetProject == userProject) {
				project.SetImportedProperty(propertyName, newValue, groupCondition, userProject, position, treatPropertyValueAsLiteral);
			} else {
				throw new ArgumentException();
			}
		}
		
		/// <summary>
		/// Removes the property from all configurations and platforms.
		/// </summary>
		void RemovePropertyCompletely(string propertyName)
		{
			RemovePropertyCompletely(project, propertyName);
			RemovePropertyCompletely(userProject, propertyName);
		}
		
		static void RemovePropertyCompletely(MSBuild.Project project, string propertyName)
		{
			List<MSBuild.BuildPropertyGroup> emptiedGroups = new List<MSBuild.BuildPropertyGroup>();
			foreach (MSBuild.BuildPropertyGroup g in project.PropertyGroups) {
				if (g.IsImported) continue;
				g.RemoveProperty(propertyName);
				if (g.Count == 0) {
					emptiedGroups.Add(g);
				}
			}
			emptiedGroups.ForEach(project.RemovePropertyGroup);
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
			
			lock (SyncRoot) {
				foreach (ProjectItem item in items) {
					item.Dispose();
				}
				items.Clear();
				itemsReadOnly = null; // remove readonly variant of item list - will regenerate on next Items call
				
				Set<ItemType> availableFileItemTypes = new Set<ItemType>();
				availableFileItemTypes.AddRange(ItemType.DefaultFileItems);
				foreach (MSBuild.BuildItem item in project.GetEvaluatedItemsByName("AvailableItemName")) {
					availableFileItemTypes.Add(new ItemType(item.Include));
				}
				// HACK: MSBuild doesn't provide us with these because we don't specify BuildingInsideVisualStudio.
				// This is not a problem for SD 4.0 as we're back to using BuildingInsideVisualStudio there (so don't merge this!).
				availableFileItemTypes.Add(ItemType.ApplicationDefinition);
				availableFileItemTypes.Add(ItemType.Page);
				availableFileItemTypes.Add(ItemType.Resource);
				availableFileItemTypes.Add(new ItemType("SplashScreen"));
				
				this.availableFileItemTypes = availableFileItemTypes.AsReadOnly();
				
				foreach (MSBuild.BuildItem item in project.EvaluatedItems) {
					if (item.IsImported) continue;
					
					items.Add(CreateProjectItem(item));
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
			
			lock (SyncRoot) {
				items.Add(item);
				itemsReadOnly = null; // remove readonly variant of item list - will regenerate on next Items call
				foreach (MSBuild.BuildItemGroup g in project.ItemGroups) {
					if (g.IsImported || !string.IsNullOrEmpty(g.Condition) || g.Count == 0)
						continue;
					if (g[0].Name == item.ItemType.ItemName) {
						MSBuildInternals.AddItemToGroup(g, item);
						return;
					}
					if (g[0].Name == "Reference")
						continue;
					if (ItemType.DefaultFileItems.Contains(new ItemType(g[0].Name))) {
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
				MSBuild.BuildItemGroup newGroup = project.AddNewItemGroup();
				MSBuildInternals.AddItemToGroup(newGroup, item);
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
			
			lock (SyncRoot) {
				if (items.Remove(item)) {
					itemsReadOnly = null; // remove readonly variant of item list - will regenerate on next Items call
					project.RemoveItem(item.BuildItem);
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
		
		internal static void InitializeMSBuildProject(MSBuild.Project project)
		{
			InitializeMSBuildProjectProperties(project.GlobalProperties);
		}
		
		/// <summary>
		/// Set compilation properties (MSBuildProperties and AddInTree/AdditionalPropertiesPath).
		/// </summary>
		internal static void InitializeMSBuildProjectProperties(MSBuild.BuildPropertyGroup propertyGroup)
		{
			foreach (KeyValuePair<string, string> entry in MSBuildEngine.MSBuildProperties) {
				propertyGroup.SetProperty(entry.Key, entry.Value);
			}
			// re-load these properties from AddInTree every time because "text" might contain
			// SharpDevelop properties resolved by the StringParser (e.g. ${property:FxCopPath})
			AddInTreeNode node = AddInTree.GetTreeNode(MSBuildEngine.AdditionalPropertiesPath, false);
			if (node != null) {
				foreach (Codon codon in node.Codons) {
					object item = codon.BuildItem(null, new System.Collections.ArrayList());
					if (item != null) {
						bool escapeValue = !codon.Properties.Get("text", "").Contains("$(");
						propertyGroup.SetProperty(codon.Id, item.ToString(), escapeValue);
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
			
			InitializeMSBuildProject(project);
			
			try {
				project.Load(fileName);
			} catch (MSBuild.InvalidProjectFileException ex) {
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
						project.Load(fileName);
					} catch (System.Xml.XmlException ex2) {
						throw new ProjectLoadException(ex2.Message, ex2);
					} catch (MSBuild.InvalidProjectFileException ex2) {
						throw new ProjectLoadException(ex2.Message, ex2);
					}
				} else {
					throw new ProjectLoadException(ex.Message, ex);
				}
			}
			
			string userFileName = fileName + ".user";
			if (File.Exists(userFileName)) {
				try {
					userProject.Load(userFileName);
				} catch (MSBuild.InvalidProjectFileException ex) {
					throw new ProjectLoadException("Error loading user part " + userFileName + ":\n" + ex.Message);
				}
			}
			
			this.ActiveConfiguration = GetEvaluatedProperty("Configuration") ?? this.ActiveConfiguration;
			this.ActivePlatform = GetEvaluatedProperty("Platform") ?? this.ActivePlatform;
			
			// Some projects do not specify default configuration/platform, so we have to set
			// Configuration and Platform in the global properties to be sure these properties exist
			project.GlobalProperties.SetProperty("Configuration", this.ActiveConfiguration, true);
			project.GlobalProperties.SetProperty("Platform", this.ActivePlatform, true);
			
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
				project.Save(fileName);
				bool userProjectDirty = userProject.IsDirty;
				string userFile = fileName + ".user";
				if (File.Exists(userFile) || userProject.PropertyGroups.Count > 0) {
					userProject.Save(userFile);
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
					project.GlobalProperties.SetProperty("Configuration", this.ActiveConfiguration, true);
					CreateItemsListFromMSBuild();
				}
			}
			base.OnActiveConfigurationChanged(e);
		}
		
		protected override void OnActivePlatformChanged(EventArgs e)
		{
			if (!isLoading) {
				lock (SyncRoot) {
					project.GlobalProperties.SetProperty("Platform", this.ActivePlatform, true);
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
			Set<string> configurationNames = new Set<string>();
			Set<string> platformNames = new Set<string>();
			
			LoadConfigurationPlatformNamesFromMSBuildInternal(project, configurationNames, platformNames);
			LoadConfigurationPlatformNamesFromMSBuildInternal(userProject, configurationNames, platformNames);
			
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
			MSBuild.Project project,
			Set<string> configurationNames, Set<string> platformNames)
		{
			foreach (MSBuild.BuildPropertyGroup g in project.PropertyGroups) {
				if (g.IsImported) {
					continue;
				}
				MSBuild.BuildProperty prop = MSBuildInternals.GetProperty(g, "Configuration");
				if (prop != null && !string.IsNullOrEmpty(prop.FinalValue)) {
					configurationNames.Add(prop.FinalValue);
				}
				prop = MSBuildInternals.GetProperty(g, "Platform");
				if (prop != null && !string.IsNullOrEmpty(prop.FinalValue)) {
					platformNames.Add(prop.FinalValue);
				}

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
		#endregion
		
		#region IProjectAllowChangeConfigurations interface implementation
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
		#endregion
	}
}
