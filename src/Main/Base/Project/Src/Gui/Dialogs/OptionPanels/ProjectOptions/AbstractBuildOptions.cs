// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using StringPair = System.Collections.Generic.KeyValuePair<System.String, System.String>;
using MSBuild = Microsoft.Build.BuildEngine;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class AbstractBuildOptions : AbstractProjectOptionPanel
	{
		protected void InitBaseIntermediateOutputPath()
		{
			helper.BindString(Get<TextBox>("baseIntermediateOutputPath"),
			                  "BaseIntermediateOutputPath",
			                  delegate { return @"obj\"; }
			                 ).CreateLocationButton("baseIntermediateOutputPathTextBox");
			ConnectBrowseFolder("baseIntermediateOutputPathBrowseButton", "baseIntermediateOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
		}
		
		protected void InitIntermediateOutputPath()
		{
			ConfigurationGuiBinding binding = helper.BindString(
				Get<TextBox>("intermediateOutputPath"),
				"IntermediateOutputPath",
				delegate {
					return Path.Combine(helper.GetProperty("BaseIntermediateOutputPath", @"obj\", true),
					                    helper.Configuration);
				}
			);
			binding.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			binding.CreateLocationButton("intermediateOutputPathTextBox");
			ConnectBrowseFolder("intermediateOutputPathBrowseButton", "intermediateOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
		}
		
		protected void InitOutputPath()
		{
			helper.BindString("outputPathTextBox", "OutputPath").CreateLocationButton("outputPathTextBox");
			ConnectBrowseFolder("outputPathBrowseButton", "outputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
		}
		
		protected void InitXmlDoc()
		{
			ConfigurationGuiBinding b;
			b = helper.BindString("xmlDocumentationTextBox", "DocumentationFile");
			b.CreateLocationButton("xmlDocumentationCheckBox");
			helper.Loaded += XmlDocHelperLoaded;
			XmlDocHelperLoaded(null, null);
		}
		
		void XmlDocHelperLoaded(object sender, EventArgs e)
		{
			Get<CheckBox>("xmlDocumentation").CheckedChanged -= UpdateXmlEnabled;
			Get<CheckBox>("xmlDocumentation").Checked = Get<TextBox>("xmlDocumentation").Text.Length > 0;
			Get<CheckBox>("xmlDocumentation").CheckedChanged += UpdateXmlEnabled;
			Get<TextBox>("xmlDocumentation").Enabled = Get<CheckBox>("xmlDocumentation").Checked;
		}
		
		void UpdateXmlEnabled(object sender, EventArgs e)
		{
			Get<TextBox>("xmlDocumentation").Enabled = Get<CheckBox>("xmlDocumentation").Checked;
			if (Get<CheckBox>("xmlDocumentation").Checked) {
				if (Get<TextBox>("xmlDocumentation").Text.Length == 0) {
					Get<TextBox>("xmlDocumentation").Text = Path.ChangeExtension(FileUtility.GetRelativePath(baseDirectory, project.OutputAssemblyFullPath), ".xml");
				}
			} else {
				Get<TextBox>("xmlDocumentation").Text = "";
			}
		}
		
		protected void InitWarnings()
		{
			ConfigurationGuiBinding b;
			b = helper.BindStringEnum("warningLevelComboBox", "WarningLevel",
			                          "4",
			                          new StringPair("0", "0"),
			                          new StringPair("1", "1"),
			                          new StringPair("2", "2"),
			                          new StringPair("3", "3"),
			                          new StringPair("4", "4"));
			ChooseStorageLocationButton locationButton = b.CreateLocationButtonInPanel("warningsGroupBox");
			b = helper.BindString("suppressWarningsTextBox", "NoWarn");
			b.RegisterLocationButton(locationButton);
			
			b = new WarningsAsErrorsBinding(this);
			helper.AddBinding("TreatWarningsAsErrors", b);
			locationButton = b.CreateLocationButtonInPanel("treatWarningsAsErrorsGroupBox");
			b = helper.BindString("specificWarningsTextBox", "WarningsAsErrors"); // must be saved AFTER TreatWarningsAsErrors
			b.RegisterLocationButton(locationButton);
			
			EventHandler setDirty = delegate {
				helper.IsDirty = true;
			};
			Get<RadioButton>("none").CheckedChanged += setDirty;
			Get<RadioButton>("specificWarnings").CheckedChanged += setDirty;
			Get<RadioButton>("all").CheckedChanged += setDirty;
			
			Get<RadioButton>("specificWarnings").CheckedChanged  += new EventHandler(UpdateWarningChecked);
			
			UpdateWarningChecked(this, EventArgs.Empty);
		}
		
		void UpdateWarningChecked(object sender, EventArgs e)
		{
			Get<TextBox>("specificWarnings").Enabled = Get<RadioButton>("specificWarnings").Checked;
		}
		
		protected class WarningsAsErrorsBinding : ConfigurationGuiBinding
		{
			RadioButton none, specific, all;
			Control specificWarningsTextBox;
			
			public WarningsAsErrorsBinding(AbstractProjectOptionPanel panel)
			{
				this.none = panel.Get<RadioButton>("none");
				this.specific = panel.Get<RadioButton>("specificWarnings");
				this.all = panel.Get<RadioButton>("all");
				specificWarningsTextBox = panel.ControlDictionary["specificWarningsTextBox"];
			}
			
			public override void Load()
			{
				if (bool.Parse(Get("false"))) {
					all.Checked = true;
				} else {
					if (this.Helper.GetProperty("WarningsAsErrors", "", true).Length > 0) {
						specific.Checked = true;
					} else {
						none.Checked = true;
					}
				}
			}
			
			public override bool Save()
			{
				if (none.Checked) {
					specificWarningsTextBox.Text = "";
				}
				if (all.Checked) {
					Set("true");
				} else {
					Set("false");
				}
				return true;
			}
		}
		
		ConfigurationGuiBinding debugInfoBinding;
		protected ChooseStorageLocationButton advancedLocationButton;
		
		protected void InitDebugInfo()
		{
			debugInfoBinding = helper.BindEnum<DebugSymbolType>("debugInfoComboBox", "DebugType");
			debugInfoBinding.CreateLocationButton("debugInfoLabel");
			DebugSymbolsLoaded(null, null);
			helper.Loaded += DebugSymbolsLoaded;
			helper.Saved += DebugSymbolsSave;
		}
		
		protected void InitAdvanced()
		{
			ConfigurationGuiBinding b;
			b = helper.BindBoolean("registerCOMInteropCheckBox", "RegisterForComInterop", false);
			b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
			advancedLocationButton = b.CreateLocationButtonInPanel("platformSpecificOptionsPanel");
			
			b = helper.BindStringEnum("generateSerializationAssemblyComboBox", "GenerateSerializationAssemblies",
			                          "Auto",
			                          new StringPair("Off", "${res:Dialog.ProjectOptions.Build.Off}"),
			                          new StringPair("On", "${res:Dialog.ProjectOptions.Build.On}"),
			                          new StringPair("Auto", "${res:Dialog.ProjectOptions.Build.Auto}"));
			b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
			b.RegisterLocationButton(advancedLocationButton);
			
			b = helper.BindHexadecimal(Get<TextBox>("dllBaseAddress"), "BaseAddress", 0x400000);
			b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
			b.RegisterLocationButton(advancedLocationButton);
			
			b = helper.BindStringEnum("targetCpuComboBox", "PlatformTarget",
			                          "AnyCPU",
			                          new StringPair("AnyCPU", "${res:Dialog.ProjectOptions.Build.TargetCPU.Any}"),
			                          new StringPair("x86", "${res:Dialog.ProjectOptions.Build.TargetCPU.x86}"),
			                          new StringPair("x64", "${res:Dialog.ProjectOptions.Build.TargetCPU.x64}"),
			                          new StringPair("Itanium", "${res:Dialog.ProjectOptions.Build.TargetCPU.Itanium}"));
			b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
			b.RegisterLocationButton(advancedLocationButton);
		}
		
		void DebugSymbolsLoaded(object sender, EventArgs e)
		{
			PropertyStorageLocations location;
			helper.GetProperty("DebugType", "", true, out location);
			if (location == PropertyStorageLocations.Unknown) {
				bool debug = helper.GetProperty("DebugSymbols", false, true, out location);
				if (location != PropertyStorageLocations.Unknown) {
					debugInfoBinding.Location = location;
					helper.SetProperty("DebugType", debug ? DebugSymbolType.Full : DebugSymbolType.None, true, location);
					debugInfoBinding.Load();
				}
			}
		}
		
		void DebugSymbolsSave(object sender, EventArgs e)
		{
			if ((DebugSymbolType)Get<ComboBox>("debugInfo").SelectedIndex == DebugSymbolType.Full) {
				helper.SetProperty("DebugSymbols", "true", true, debugInfoBinding.Location);
			} else {
				helper.SetProperty("DebugSymbols", "false", true, debugInfoBinding.Location);
			}
		}
		
		protected void InitTargetFramework(string defaultTargets, string extendedTargets)
		{
			const string TargetFrameworkProperty = "TargetFrameworkVersion";
			ConfigurationGuiBinding targetFrameworkBinding;
			targetFrameworkBinding = helper.BindStringEnum("targetFrameworkComboBox", TargetFrameworkProperty,
			                                               "",
			                                               new StringPair("", "Default (.NET 2.0)"),
			                                               new StringPair("v1.0", ".NET Framework 1.0"),
			                                               new StringPair("v1.1", ".NET Framework 1.1"),
			                                               new StringPair("v2.0", ".NET Framework 2.0"),
			                                               new StringPair("CF 1.0", "Compact Framework 1.0"),
			                                               new StringPair("CF 2.0", "Compact Framework 2.0"),
			                                               new StringPair("Mono v1.1", "Mono 1.1"),
			                                               new StringPair("Mono v2.0", "Mono 2.0"));
			targetFrameworkBinding.CreateLocationButton("targetFrameworkLabel");
			helper.Saved += delegate {
				// Test if SharpDevelop-Build extensions are needed
				MSBuildBasedProject project = helper.Project;
				bool needExtensions = false;
				foreach (MSBuild.BuildProperty p in project.GetAllProperties(TargetFrameworkProperty)) {
					if (p.IsImported == false && p.Value.Length > 0) {
						needExtensions = true;
						break;
					}
				}
				foreach (MSBuild.Import import in project.MSBuildProject.Imports) {
					if (needExtensions) {
						if (defaultTargets.Equals(import.ProjectPath, StringComparison.InvariantCultureIgnoreCase)) {
							//import.ProjectPath = extendedTargets;
							MSBuildInternals.SetImportProjectPath(project.MSBuildProject, import, extendedTargets);
							break;
						}
					} else {
						if (extendedTargets.Equals(import.ProjectPath, StringComparison.InvariantCultureIgnoreCase)) {
							//import.ProjectPath = defaultTargets;
							MSBuildInternals.SetImportProjectPath(project.MSBuildProject, import, defaultTargets);
							break;
						}
					}
				}
			};
		}
	}
}
