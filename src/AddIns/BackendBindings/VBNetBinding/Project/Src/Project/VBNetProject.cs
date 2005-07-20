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

namespace VBNetBinding
{
	public enum WarningsAsErrors {
		None,
		Specific,
		All
	}
	
	/// <summary>
	/// Description of CSharpProject.
	/// </summary>
	public class VBNetProject : AdvancedMSBuildProject
	{
		[Browsable(false)]
		public int WarningLevel
		{
			get {
				return BaseConfiguration.Get("WarningLevel", 4);
			}
			set {
				BaseConfiguration.Set("WarningLevel", 4, value);
			}
		}
		
		public override OutputType OutputType {
			get {
				return base.OutputType;
			}
			set {
				base.OutputType = value;
				switch (value) {
					case OutputType.WinExe:
						BaseConfiguration.Set("MyType", "", "WindowsForms");
						break;
					case OutputType.Exe:
						BaseConfiguration.Set("MyType", "", "Console");
						break;
					default:
						BaseConfiguration.Set("MyType", "", "Windows");
						break;
				}
			}
		}
		
		[Browsable(false)]
		public override IAmbience Ambience {
			get {
				return VBNetAmbience.Instance;
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
		
		
		
		public VBNetProject(string fileName, string projectName)
		{
			this.Name = projectName;
			InitVB();
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public VBNetProject(ProjectCreateInformation info)
		{
			InitVB();
			Create(info);
			imports.Add(@"$(MSBuildBinPath)\Microsoft.VisualBasic.Targets");
		}
		
		public override ParseProjectContent CreateProjectContent()
		{
			ParseProjectContent pc = base.CreateProjectContent();
			ReferenceProjectItem vbRef = new ReferenceProjectItem(this, "Microsoft.VisualBasic");
			pc.ReferencedContents.Add(ProjectContentRegistry.GetProjectContentForReference(vbRef));
			MyNamespaceBuilder.BuildNamespace(this, pc);
			return pc;
		}
		
		void InitVB()
		{
			Language = "VBNet";
			LanguageProperties = ICSharpCode.SharpDevelop.Dom.LanguageProperties.VBNet;
			BuildConstantSeparator = ',';
		}
	}
}
