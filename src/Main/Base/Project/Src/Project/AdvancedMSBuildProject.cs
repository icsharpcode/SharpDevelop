// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum AssemblyOriginatorKeyMode {
		None,
		File,
		Provider
	}
	
	public enum RunPostBuildEvent {
		Always,
		OnSuccessfulBuild,
		OnOutputUpdated
	}
	
	public enum StartAction {
		Project,
		Program,
		StartURL
	}
	
	/// <summary>
	/// Common base of C# and VB.NET projects.
	/// Any AdvancedMSBuildProject can use the common option panels.
	/// </summary>
	public class AdvancedMSBuildProject : MSBuildProject
	{
		#region Application
		[Browsable(false)]
		public string ApplicationIcon {
			get {
				return BaseConfiguration["ApplicationIcon"];
			}
			set {
				BaseConfiguration.Set("ApplicationIcon", value);
			}
		}
		
		[Browsable(false)]
		public string StartupObject {
			get {
				return BaseConfiguration["StartupObject"];
			}
			set {
				BaseConfiguration.Set("StartupObject", value);
			}
		}
		
		[Browsable(false)]
		public string Win32Resource {
			get {
				return BaseConfiguration["Win32Resource"];
			}
			set {
				BaseConfiguration.Set("Win32Resource", value);
			}
		}
		#endregion
		
		#region Signing
		[Browsable(false)]
		public bool SignAssembly {
			get {
				return BaseConfiguration.Get("SignAssembly", false);
			}
			set {
				BaseConfiguration.Set("SignAssembly", false, value);
			}
		}
		
		[Browsable(false)]
		public bool DelaySign {
			get {
				return BaseConfiguration.Get("DelaySign", false);
			}
			set {
				BaseConfiguration.Set("DelaySign", false, value);
			}
		}
		
		[Browsable(false)]
		public string AssemblyOriginatorKeyFile {
			get {
				return BaseConfiguration["AssemblyOriginatorKeyFile"];
			}
			set {
				BaseConfiguration.Set("AssemblyOriginatorKeyFile", value);
			}
		}
		
		[Browsable(false)]
		public string AssemblyKeyProviderName {
			get {
				return BaseConfiguration["AssemblyKeyProviderName"];
			}
			set {
				BaseConfiguration.Set("AssemblyKeyProviderName", value);
			}
		}
		
		[Browsable(false)]
		public AssemblyOriginatorKeyMode AssemblyOriginatorKeyMode {
			get {
				return BaseConfiguration.Get("AssemblyOriginatorKeyMode", AssemblyOriginatorKeyMode.None);
			}
			set {
				BaseConfiguration.Set("AssemblyOriginatorKeyMode", AssemblyOriginatorKeyMode.None, value);
			}
		}
		#endregion
		
		#region Build events
		[Browsable(false)]
		public RunPostBuildEvent RunPostBuildEvent {
			get {
				return BaseConfiguration.Get("RunPostBuildEvent", RunPostBuildEvent.OnSuccessfulBuild);
			}
			set {
				BaseConfiguration.Set("RunPostBuildEvent", RunPostBuildEvent.OnSuccessfulBuild, value);
			}
		}
		
		[Browsable(false)]
		public string PreBuildEvent {
			get {
				return BaseConfiguration["PreBuildEvent"];
			}
			set {
				BaseConfiguration.Set("PreBuildEvent", value);
			}
		}
		
		[Browsable(false)]
		public string PostBuildEvent {
			get {
				return BaseConfiguration["PostBuildEvent"];
			}
			set {
				BaseConfiguration.Set("PostBuildEvent", value);
			}
		}
		#endregion
		
		#region Publishing
		[Browsable(false)]
		public string PublishUrl {
			get {
				return BaseConfiguration["PublishUrl"];
			}
			set {
				BaseConfiguration.Set("PublishUrl", value);
			}
		}
		
		[Browsable(false)]
		public bool Install {
			get {
				return BaseConfiguration.Get("Install", false);
			}
			set {
				BaseConfiguration.Set("Install", false, value);
			}
		}
		
		[Browsable(false)]
		public bool UpdateEnabled {
			get {
				return BaseConfiguration.Get("UpdateEnabled", false);
			}
			set {
				BaseConfiguration.Set("UpdateEnabled", false, value);
			}
		}
		
		[Browsable(false)]
		public bool UpdatePeriodically {
			get {
				return BaseConfiguration.Get("UpdatePeriodically", false);
			}
			set {
				BaseConfiguration.Set("UpdatePeriodically", false, value);
			}
		}
		
		[Browsable(false)]
		public bool UpdateRequired {
			get {
				return BaseConfiguration.Get("UpdateRequired", false);
			}
			set {
				BaseConfiguration.Set("UpdateRequired", false, value);
			}
		}
		
		[Browsable(false)]
		public bool UpdateUrlEnabled {
			get {
				return BaseConfiguration.Get("UpdateUrlEnabled", false);
			}
			set {
				BaseConfiguration.Set("UpdateUrlEnabled", value);
			}
		}
		
		[Browsable(false)]
		public bool BootstrapperEnabled {
			get {
				return BaseConfiguration.Get("BootstrapperEnabled", false);
			}
			set {
				BaseConfiguration.Set("BootstrapperEnabled", false, value);
			}
		}
		
		[Browsable(false)]
		public string InstallFrom {
			get {
				return BaseConfiguration["InstallFrom"];
			}
			set {
				BaseConfiguration.Set("InstallFrom", value);
			}
		}
		[Browsable(false)]
		public string FallbackCulture {
			get {
				return BaseConfiguration["FallbackCulture"];
			}
			set {
				BaseConfiguration.Set("FallbackCulture", value);
			}
		}
		[Browsable(false)]
		public string UpdateMode {
			get {
				return BaseConfiguration["UpdateMode"];
			}
			set {
				BaseConfiguration.Set("UpdateMode", value);
			}
		}
		[Browsable(false)]
		public string UpdateIntervalUnits {
			get {
				return BaseConfiguration["UpdateIntervalUnits"];
			}
			set {
				BaseConfiguration.Set("UpdateIntervalUnits", value);
			}
		}
		
		[Browsable(false)]
		public string ApplicationVersion {
			get {
				return BaseConfiguration["ApplicationVersion"];
			}
			set {
				BaseConfiguration.Set("ApplicationVersion", value);
			}
		}
		[Browsable(false)]
		public int UpdateInterval {
			get {
				return BaseConfiguration.Get("UpdateInterval", 0);
			}
			set {
				BaseConfiguration.Set("UpdateInterval", 0, value);
			}
		}
		[Browsable(false)]
		public int ApplicationRevision {
			get {
				return BaseConfiguration.Get("ApplicationRevision", 0);
			}
			set {
				BaseConfiguration.Set("ApplicationRevision", 0, value);
			}
		}
		#endregion
		
		#region Debug Options
		const string runConfiguration = "Debug";
		const string runProcessor = "AnyCPU";
		
		public override bool IsStartable {
			get {
				StartAction action = GetStartAction(runConfiguration, runProcessor);
				switch (action) {
					case StartAction.Project:
						return base.IsStartable;
					case StartAction.Program:
						return GetStartProgram(runConfiguration, runProcessor).Length > 0;
					case StartAction.StartURL:
						return GetStartURL(runConfiguration, runProcessor).Length > 0;
				}
				return false;
			}
		}
		
		void StartProgram(string program, bool withDebugging)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = Path.Combine(Directory, program);
			string workingDir = GetStartWorkingDirectory(runConfiguration, runProcessor);
			if (workingDir.Length == 0) {
				psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);
			} else {
				psi.WorkingDirectory = Path.Combine(Directory, workingDir);
			}
			psi.Arguments = GetStartArguments(runConfiguration, runProcessor);
			
			if (withDebugging) {
				DebuggerService.CurrentDebugger.Start(psi);
			} else {
				DebuggerService.CurrentDebugger.StartWithoutDebugging(psi);
			}
		}
		
		public override void Start(bool withDebugging)
		{
			StartAction action = GetStartAction(runConfiguration, runProcessor);
			switch (action) {
				case StartAction.Project:
					StartProgram(this.OutputAssemblyFullPath, withDebugging);
					break;
				case StartAction.Program:
					StartProgram(GetStartProgram(runConfiguration, runProcessor), withDebugging);
					break;
				case StartAction.StartURL:
					FileService.OpenFile("browser://" + GetStartURL(runConfiguration, runProcessor));
					break;
				default:
					throw new ApplicationException("Unknown start action: " + action);
			}
		}
		
		public string GetStartProgram(string configurationName, string platform)
		{
			return GetUserConfiguration(configurationName, platform)["StartProgram"];
		}
		
		public void SetStartProgram(string configurationName, string platform, string val)
		{
			GetUserConfiguration(configurationName, platform)["StartProgram"] = val;
		}
		
		public string GetStartURL(string configurationName, string platform)
		{
			return GetUserConfiguration(configurationName, platform)["StartURL"];
		}
		
		public void SetStartURL(string configurationName, string platform, string val)
		{
			GetUserConfiguration(configurationName, platform)["StartURL"] = val;
		}
		
		public StartAction GetStartAction(string configurationName, string platform)
		{
			return GetUserConfiguration(configurationName, platform).Get("StartAction", StartAction.Project);
		}
		
		public void SetStartAction(string configurationName, string platform, StartAction val)
		{
			GetUserConfiguration(configurationName, platform).Set("StartAction", StartAction.Project, val);
		}
		
		public string GetStartArguments(string configurationName, string platform)
		{
			return GetUserConfiguration(configurationName, platform)["StartArguments"];
		}
		
		public void SetStartArguments(string configurationName, string platform, string val)
		{
			GetUserConfiguration(configurationName, platform)["StartArguments"] = val;
		}
		
		public string GetStartWorkingDirectory(string configurationName, string platform)
		{
			return GetUserConfiguration(configurationName, platform)["StartWorkingDirectory"];
		}
		
		public void SetStartWorkingDirectory(string configurationName, string platform, string val)
		{
			GetUserConfiguration(configurationName, platform)["StartWorkingDirectory"] = val;
		}
		
		
		public bool GetRemoteDebugEnabled(string configurationName, string platform)
		{
			return GetUserConfiguration(configurationName, platform).Get("RemoteDebugEnabled", false);
		}
		
		public void SetRemoteDebugEnabled(string configurationName, string platform, bool val)
		{
			GetUserConfiguration(configurationName, platform).Set("RemoteDebugEnabled", false, val);
		}
		
		public string GetRemoteDebugMachine(string configurationName, string platform)
		{
			return GetUserConfiguration(configurationName, platform)["RemoteDebugMachine"];
		}
		
		public void SetRemoteDebugMachine(string configurationName, string platform, string val)
		{
			GetUserConfiguration(configurationName, platform)["RemoteDebugMachine"] = val;
		}
		#endregion
	}
}
