using System;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace CSharpBinding
{
	public enum WarningsAsErrors {
		None,
		Specific,
		All
	}
	
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
	/// Description of CSharpProject.
	/// </summary>
	public class CSharpProject : MSBuildProject
	{
		[Browsable(false)]
		public int WarningLevel {
			get {
				return BaseConfiguration.Get("WarningLevel", 4);
			}
			set {
				BaseConfiguration.Set("WarningLevel", 4, value);
			}
		}
		[Browsable(false)]
		public string ApplicationIcon {
			get {
				return BaseConfiguration["ApplicationIcon"];
			}
			set {
				BaseConfiguration.Set("ApplicationIcon", value);
			}
		}
		
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
		
		public bool GetDebugSymbols(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get("DebugSymbols", false);
		}
		
		public void SetDebugSymbols(string configurationName, string platform, bool val)
		{
			GetConfiguration(configurationName, platform).Set("DebugSymbols", val);
		}

		public bool GetOptimize(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get("Optimize", false);
		}
		
		public void SetOptimize(string configurationName, string platform, bool val)
		{
			GetConfiguration(configurationName, platform).Set("Optimize", false, val);
		}

		public bool GetAllowUnsafeBlocks(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get("AllowUnsafeBlocks", false);
		}
		
		public void SetAllowUnsafeBlocks(string configurationName, string platform, bool val)
		{
			GetConfiguration(configurationName, platform).Set("AllowUnsafeBlocks", false, val);
		}
		
		public WarningsAsErrors GetTreatWarningsAsErrors(string configurationName, string platform)
		{
			PropertyGroup properties = GetConfiguration(configurationName, platform);
			if (!properties.IsSet("TreatWarningsAsErrors")) {
				return WarningsAsErrors.Specific;
			}
			bool warningAsErrors = properties.Get("TreatWarningsAsErrors", false);
			return warningAsErrors ? WarningsAsErrors.All : WarningsAsErrors.None;
		}
		
		public void SetTreatWarningsAsErrors(string configurationName, string platform, WarningsAsErrors warningsAsErrors)
		{
			PropertyGroup properties = GetConfiguration(configurationName, platform);
			switch (warningsAsErrors) {
				case WarningsAsErrors.None:
					properties.Set("TreatWarningsAsErrors", false, false);
					break;
				case WarningsAsErrors.Specific:
					properties.Remove("TreatWarningsAsErrors");
					break;
				case WarningsAsErrors.All:
					properties.Set("TreatWarningsAsErrors", false, true);
					break;
			}
		}
		
		public bool GetRegisterForComInterop(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get("RegisterForComInterop", false);
		}
		
		public void SetRegisterForComInterop(string configurationName, string platform, bool val)
		{
			GetConfiguration(configurationName, platform).Set("RegisterForComInterop", false, val);
		}
		
		public string GetDefineConstants(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform)["DefineConstants"];
		}
		
		public void SetDefineConstants(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["DefineConstants"] = val;
		}
		
		public string GetNoWarn(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform)["NoWarn"];
		}
		
		public void SetNoWarn(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["NoWarn"] = val;
		}
		
		public string GetWarningsAsErrors(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform)["WarningsAsErrors"];
		}
		
		public void SetWarningsAsErrors(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["WarningsAsErrors"] = val;
		}
		
		
		
		public string GetDocumentationFile(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform)["DocumentationFile"];
		}
		
		public void SetDocumentationFile(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["DocumentationFile"] = val;
		}
		
		public string GetLangVersion(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform)["LangVersion"];
		}
		
		public void SetLangVersion(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["LangVersion"] = val;
		}
		
		public string GetErrorReport(string configurationName, string platform)
		{ // none, prompt, send
			return GetConfiguration(configurationName, platform)["ErrorReport"];
		}
		
		public void SetErrorReport(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["ErrorReport"] = val;
		}
		
		public int GetBaseAddress(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get<int>("BaseAddress", 4194304);
		}
		
		public void SetBaseAddress(string configurationName, string platform, int val)
		{
			GetConfiguration(configurationName, platform).Set("BaseAddress", 4194304, val);
		}
		
		
		public int GetFileAlignment(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get<int>("FileAlignment", 4096);
		}
		
		public void SetFileAlignment(string configurationName, string platform, int val)
		{
			GetConfiguration(configurationName, platform).Set("FileAlignment", 4096, val);
		}
		
		public string GetDebugType(string configurationName, string platform)
		{ // none, full, pdbonly
			return GetConfiguration(configurationName, platform)["DebugType"];
		}
		
		public void SetDebugType(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["DebugType"] = val;
		}
		
		public bool GetCheckForOverflowUnderflow(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get("CheckForOverflowUnderflow", false);
		}
		
		public void SetCheckForOverflowUnderflow(string configurationName, string platform, bool val)
		{
			GetConfiguration(configurationName, platform).Set("CheckForOverflowUnderflow", false, val);
		}
		public bool GetNoStdLib(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform).Get("NoStdLib", false);
		}
		
		public void SetNoStdLib(string configurationName, string platform, bool val)
		{
			GetConfiguration(configurationName, platform).Set("NoStdLib", false, val);
		}
		
		#region Debug Options
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
		
		public CSharpProject(string fileName, string projectName)
		{
			this.Name = projectName;
			Language = "C#";
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public CSharpProject(ProjectCreateInformation info)
		{
			Language = "C#";
			Create(info);
			imports.Add(@"$(MSBuildBinPath)\Microsoft.CSHARP.Targets");
		}
	}
}
