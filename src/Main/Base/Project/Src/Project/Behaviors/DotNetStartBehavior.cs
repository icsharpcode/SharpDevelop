// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Project
{
	public class DotNetStartBehavior : ProjectBehavior
	{
		static string RemoveQuotes(string text)
		{
			if (text.StartsWith("\"") && text.EndsWith("\""))
				return text.Substring(1, text.Length - 2);
			else
				return text;
		}
		
		public DotNetStartBehavior(CompilableProject project, ProjectBehavior next = null)
			: base(project, next)
		{
			
		}
		
		new protected CompilableProject Project {
			get { return (CompilableProject)base.Project; }
		}
		
		public override bool IsStartable {
			get {
				switch (StartAction) {
					case StartAction.Project:
						return Project.OutputType == OutputType.Exe || Project.OutputType == OutputType.WinExe;
					case StartAction.Program:
						return StartProgram.Length > 0;
					case StartAction.StartURL:
						return StartUrl.Length > 0;
					default:
						return base.IsStartable;
				}
			}
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			switch (StartAction) {
				case StartAction.Project:
					return CreateStartInfo(Project.OutputAssemblyFullPath, Project.Directory, StartWorkingDirectory, StartArguments);
				case StartAction.Program:
					return CreateStartInfo(StartProgram, Project.Directory, StartWorkingDirectory, StartArguments);
				case StartAction.StartURL:
					string url = StartUrl;
					if (!FileUtility.IsUrl(url))
						url = "http://" + url;
					return new ProcessStartInfo(url);
				default:
					return base.CreateStartInfo();
			}
		}
		
		
		/// <summary>
		/// Creates a <see cref="ProcessStartInfo"/> for the specified program, using
		/// arguments and working directory from the project options.
		/// </summary>
		public static ProcessStartInfo CreateStartInfo(string program, string projectDirectory, string startWorkingDirectory, string startArguments)
		{
			program = RemoveQuotes(program);
			if (!FileUtility.IsValidPath(program)) {
				throw new ProjectStartException(program + " is not a valid path; the process cannot be started.");
			}
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = Path.Combine(projectDirectory, program);
			string workingDir = StringParser.Parse(startWorkingDirectory);
			
			if (workingDir.Length == 0) {
				psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);
			} else {
				workingDir = RemoveQuotes(workingDir);
				
				if (!FileUtility.IsValidPath(workingDir)) {
					throw new ProjectStartException("Working directory '" + workingDir + "' is invalid; the process cannot be started. You can specify the working directory in the project options.");
				}
				psi.WorkingDirectory = Path.Combine(projectDirectory, workingDir);
			}
			psi.Arguments = StringParser.Parse(startArguments);
			
			if (!File.Exists(psi.FileName)) {
				throw new ProjectStartException(psi.FileName + " does not exist and cannot be started.");
			}
			if (!System.IO.Directory.Exists(psi.WorkingDirectory)) {
				throw new ProjectStartException("Working directory " + psi.WorkingDirectory + " does not exist; the process cannot be started. You can specify the working directory in the project options.");
			}
			return psi;
		}
		
		public override void ProjectCreationComplete()
		{
			TargetFramework fx = Project.CurrentTargetFramework;
			if (fx != null && (fx.IsBasedOn(TargetFramework.Net35) || fx.IsBasedOn(TargetFramework.Net35Client))) {
				AddDotnet35References();
			}
			if (fx != null && (fx.IsBasedOn(TargetFramework.Net40) || fx.IsBasedOn(TargetFramework.Net40Client))) {
				AddDotnet40References();
			}
			if (fx != null)
				UpdateAppConfig(fx);
			if (Project.OutputType != OutputType.Library) {
				if (DotnetDetection.IsDotnet45Installed()) {
					Project.SetProperty(null, Project.ActivePlatform, "Prefer32Bit", "True", PropertyStorageLocations.PlatformSpecific, true);
				} else {
					Project.SetProperty(null, Project.ActivePlatform, "PlatformTarget", "x86", PropertyStorageLocations.PlatformSpecific, true);
				}
			}
			base.ProjectCreationComplete();
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (".resx".Equals(extension, StringComparison.OrdinalIgnoreCase)
			    || ".resources".Equals(extension, StringComparison.OrdinalIgnoreCase))
				return ItemType.EmbeddedResource;
			
			return base.GetDefaultItemType(fileName);
		}
		
		public override CompilerVersion CurrentCompilerVersion {
			get {
				switch (Project.MinimumSolutionVersion) {
					case Solution.SolutionVersionVS2005:
						return CompilerVersion.MSBuild20;
					case Solution.SolutionVersionVS2008:
						return CompilerVersion.MSBuild35;
					case Solution.SolutionVersionVS2010:
					case Solution.SolutionVersionVS11:
						return CompilerVersion.MSBuild40;
					default:
						throw new NotSupportedException();
				}
			}
		}
		
		public override IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			List<CompilerVersion> versions = new List<CompilerVersion>();
			if (DotnetDetection.IsDotnet35SP1Installed()) {
				versions.Add(CompilerVersion.MSBuild20);
				versions.Add(CompilerVersion.MSBuild35);
			}
			versions.Add(CompilerVersion.MSBuild40);
			return versions;
		}
		
		public override TargetFramework CurrentTargetFramework {
			get {
				string fxVersion = Project.TargetFrameworkVersion;
				string fxProfile = Project.TargetFrameworkProfile;
				if (string.Equals(fxProfile, "Client", StringComparison.OrdinalIgnoreCase)) {
					foreach (ClientProfileTargetFramework fx in TargetFramework.TargetFrameworks.OfType<ClientProfileTargetFramework>())
						if (fx.FullFramework.Name == fxVersion)
							return fx;
				} else {
					foreach (TargetFramework fx in TargetFramework.TargetFrameworks)
						if (fx.Name == fxVersion)
							return fx;
				}
				return null;
			}
		}
		
		public override IEnumerable<TargetFramework> GetAvailableTargetFrameworks()
		{
			return TargetFramework.TargetFrameworks.Where(fx => fx.IsAvailable());
		}
		
		public override void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			if (!Project.ReadOnly) {
				lock (Project.SyncRoot) {
					TargetFramework oldFramework = Project.CurrentTargetFramework;
					if (newVersion != null && GetAvailableCompilerVersions().Contains(newVersion)) {
						Project.SetToolsVersion(newVersion.MSBuildVersion.Major + "." + newVersion.MSBuildVersion.Minor);
					}
					if (newFramework != null) {
						UpdateAppConfig(newFramework);
						
						ClientProfileTargetFramework clientProfile = newFramework as ClientProfileTargetFramework;
						if (clientProfile != null) {
							newFramework = clientProfile.FullFramework;
							((MSBuildBasedProject)Project).SetProperty(null, null, "TargetFrameworkProfile", "Client", PropertyStorageLocations.Base, true);
						} else {
							((MSBuildBasedProject)Project).SetProperty(null, null, "TargetFrameworkProfile", "", PropertyStorageLocations.Base, true);
						}
						((MSBuildBasedProject)Project).SetProperty(null, null, "TargetFrameworkVersion", newFramework.Name, PropertyStorageLocations.Base, true);
						
						if (oldFramework is ClientProfileTargetFramework)
							oldFramework = ((ClientProfileTargetFramework)oldFramework).FullFramework;
						
						if (oldFramework != null && !oldFramework.IsBasedOn(TargetFramework.Net35) && newFramework.IsBasedOn(TargetFramework.Net35))
							AddDotnet35References();
						else if (oldFramework != null && oldFramework.IsBasedOn(TargetFramework.Net35) && !newFramework.IsBasedOn(TargetFramework.Net35))
							RemoveDotnet35References();
						
						if (oldFramework != null && !oldFramework.IsBasedOn(TargetFramework.Net40) && newFramework.IsBasedOn(TargetFramework.Net40))
							AddDotnet40References();
						else if (oldFramework != null && oldFramework.IsBasedOn(TargetFramework.Net40) && !newFramework.IsBasedOn(TargetFramework.Net40))
							RemoveDotnet40References();
					}
					AddOrRemoveExtensions();
					Project.Save();
					ResXConverter.UpdateResourceFiles(Project);
				}
			}
		}
		
		void AddDotnet35References()
		{
			AddReferenceIfNotExists("System.Core", "3.5");
			
			if (Project.GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, "System.Data", StringComparison.OrdinalIgnoreCase))) {
				AddReferenceIfNotExists("System.Data.DataSetExtensions", "3.5");
			}
			if (Project.GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, "System.Xml", StringComparison.OrdinalIgnoreCase))) {
				AddReferenceIfNotExists("System.Xml.Linq", "3.5");
			}
		}
		
		void RemoveDotnet35References()
		{
			// undo "AddDotnet35References"
			RemoveReference("System.Core");
			RemoveReference("System.Data.DataSetExtensions");
			RemoveReference("System.Xml.Linq");
		}
		
		void AddDotnet40References()
		{
			if (Project.GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, "WindowsBase", StringComparison.OrdinalIgnoreCase))) {
				AddReferenceIfNotExists("System.Xaml", "4.0");
			}
		}
		
		protected virtual void RemoveDotnet40References()
		{
			RemoveReference("System.Xaml");
		}
		
		void AddReferenceIfNotExists(string name, string requiredTargetFramework)
		{
			if (!(Project.GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, name, StringComparison.OrdinalIgnoreCase)))) {
				ReferenceProjectItem rpi = new ReferenceProjectItem(Project, name);
				if (requiredTargetFramework != null)
					rpi.SetMetadata("RequiredTargetFramework", requiredTargetFramework);
				ProjectService.AddProjectItem(Project, rpi);
			}
		}
		
		void RemoveReference(string name)
		{
			ProjectItem reference = Project.GetItemsOfType(ItemType.Reference).FirstOrDefault(r => string.Equals(r.Include, name, StringComparison.OrdinalIgnoreCase));
			if (reference != null)
				ProjectService.RemoveProjectItem(Project, reference);
		}
		
		void UpdateAppConfig(TargetFramework newFramework)
		{
			// When changing the target framework, update any existing app.config
			// Also, for applications (not libraries), create an app.config is it is required for the target framework
			bool createAppConfig = newFramework.RequiresAppConfigEntry && (Project.OutputType != OutputType.Library && Project.OutputType != OutputType.Module);
			
			string appConfigFileName = CompilableProject.GetAppConfigFile(Project, createAppConfig);
			if (appConfigFileName == null)
				return;
			
			using (FakeXmlViewContent xml = new FakeXmlViewContent(appConfigFileName)) {
				if (xml.Document != null) {
					XElement configuration = xml.Document.Root;
					XElement startup = configuration.Element("startup");
					if (startup == null) {
						startup = new XElement("startup");
						if (configuration.HasElements && configuration.Elements().First().Name == "configSections") {
							// <configSections> must be first element
							configuration.Elements().First().AddAfterSelf(startup);
						} else {
							startup = configuration.AddFirstWithIndentation(startup);
						}
					}
					XElement supportedRuntime = startup.Element("supportedRuntime");
					if (supportedRuntime == null) {
						supportedRuntime = startup.AddFirstWithIndentation(new XElement("supportedRuntime"));
					}
					supportedRuntime.SetAttributeValue("version", newFramework.SupportedRuntimeVersion);
					supportedRuntime.SetAttributeValue("sku", newFramework.SupportedSku);
				}
			}
		}
		
		protected virtual void AddOrRemoveExtensions()
		{
		}
		
		#region CreateProjectItem
		/// <summary>
		/// Creates a new projectItem for the passed itemType
		/// </summary>
		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			switch (item.ItemType.ItemName) {
				case "Reference":
					return new ReferenceProjectItem(Project, item);
				case "ProjectReference":
					return new ProjectReferenceProjectItem(Project, item);
				case "COMReference":
					return new ComReferenceProjectItem(Project, item);
				case "Import":
					return new ImportProjectItem(Project, item);
					
				case "None":
				case "Compile":
				case "EmbeddedResource":
				case "Resource":
				case "Content":
				case "Folder":
					return new FileProjectItem(Project, item);
					
				case "WebReferenceUrl":
					return new WebReferenceUrl(Project, item);
					
				case "WebReferences":
					return new WebReferencesProjectItem(Project, item);
					
				case "WCFMetadata":
					return new ServiceReferencesProjectItem(Project, item);
					
				case "WCFMetadataStorage":
					return new ServiceReferenceProjectItem(Project, item);
					
				default:
					if (Project.AvailableFileItemTypes.Contains(item.ItemType)
					    || SafeFileExists(Project.Directory, item.EvaluatedInclude))
						return new FileProjectItem(Project, item);
					
					return base.CreateProjectItem(item);
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
		
		#region Starting (debugging)
		public string StartProgram {
			get {
				return ((MSBuildBasedProject)Project).GetEvaluatedProperty("StartProgram") ?? "";
			}
			set {
				((MSBuildBasedProject)Project).SetProperty("StartProgram", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		public string StartUrl {
			get {
				return ((MSBuildBasedProject)Project).GetEvaluatedProperty("StartURL") ?? "";
			}
			set {
				((MSBuildBasedProject)Project).SetProperty("StartURL", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		public StartAction StartAction {
			get {
				string propertyValue = ((MSBuildBasedProject)Project).GetEvaluatedProperty("StartAction") ?? "Project";
				StartAction action;
				if (Enum.TryParse(propertyValue, out action))
					return action;
				else
					return StartAction.Project;
			}
			set {
				((MSBuildBasedProject)Project).SetProperty("StartAction", value.ToString());
			}
		}
		
		public string StartArguments {
			get {
				return ((MSBuildBasedProject)Project).GetEvaluatedProperty("StartArguments") ?? "";
			}
			set {
				((MSBuildBasedProject)Project).SetProperty("StartArguments", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		public string StartWorkingDirectory {
			get {
				return ((MSBuildBasedProject)Project).GetEvaluatedProperty("StartWorkingDirectory") ?? "";
			}
			set {
				((MSBuildBasedProject)Project).SetProperty("StartWorkingDirectory", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		#endregion
	}
}

