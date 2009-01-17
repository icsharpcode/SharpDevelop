// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			                  TextBoxEditMode.EditRawProperty,
			                  delegate { return @"obj\"; }
			                 ).CreateLocationButton("baseIntermediateOutputPathTextBox");
			ConnectBrowseFolder("baseIntermediateOutputPathBrowseButton", "baseIntermediateOutputPathTextBox",
			                    "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
			                    TextBoxEditMode.EditRawProperty);
		}
		
		protected void InitIntermediateOutputPath()
		{
			ConfigurationGuiBinding binding = helper.BindString(
				Get<TextBox>("intermediateOutputPath"),
				"IntermediateOutputPath",
				TextBoxEditMode.EditRawProperty,
				delegate {
					return Path.Combine(helper.GetProperty("BaseIntermediateOutputPath", @"obj\", true),
					                    helper.Configuration);
				}
			);
			binding.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			binding.CreateLocationButton("intermediateOutputPathTextBox");
			ConnectBrowseFolder("intermediateOutputPathBrowseButton",
			                    "intermediateOutputPathTextBox",
			                    "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
			                    TextBoxEditMode.EditRawProperty);
		}
		
		protected void InitOutputPath()
		{
			helper.BindString("outputPathTextBox", "OutputPath", TextBoxEditMode.EditRawProperty)
				.CreateLocationButton("outputPathTextBox");
			ConnectBrowseFolder("outputPathBrowseButton", "outputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}", TextBoxEditMode.EditRawProperty);
		}
		
		protected void InitXmlDoc()
		{
			ConfigurationGuiBinding b;
			b = helper.BindString("xmlDocumentationTextBox", "DocumentationFile", TextBoxEditMode.EditRawProperty);
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
					Get<TextBox>("xmlDocumentation").Text = MSBuildInternals.Escape(
						Path.ChangeExtension(FileUtility.GetRelativePath(baseDirectory, project.OutputAssemblyFullPath),
						                     ".xml"));
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
			b = helper.BindString("suppressWarningsTextBox", "NoWarn", TextBoxEditMode.EditEvaluatedProperty);
			b.RegisterLocationButton(locationButton);
			
			b = new WarningsAsErrorsBinding(this);
			helper.AddBinding("TreatWarningsAsErrors", b);
			locationButton = b.CreateLocationButtonInPanel("treatWarningsAsErrorsGroupBox");
			b = helper.BindString("specificWarningsTextBox", "WarningsAsErrors", TextBoxEditMode.EditEvaluatedProperty); // must be saved AFTER TreatWarningsAsErrors
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
			
			b = CreatePlatformTarget();
			b.RegisterLocationButton(advancedLocationButton);
		}
		
		protected ConfigurationGuiBinding CreatePlatformTarget()
		{
			ConfigurationGuiBinding b;
			b = helper.BindStringEnum("targetCpuComboBox", "PlatformTarget",
			                          "AnyCPU",
			                          new StringPair("AnyCPU", "${res:Dialog.ProjectOptions.Build.TargetCPU.Any}"),
			                          new StringPair("x86", "${res:Dialog.ProjectOptions.Build.TargetCPU.x86}"),
			                          new StringPair("x64", "${res:Dialog.ProjectOptions.Build.TargetCPU.x64}"),
			                          new StringPair("Itanium", "${res:Dialog.ProjectOptions.Build.TargetCPU.Itanium}"));
			b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
			return b;
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
		
		protected void InitTargetFramework()
		{
			Button convertProjectToMSBuild35Button = (Button)ControlDictionary["convertProjectToMSBuild35Button"];
			ComboBox targetFrameworkComboBox = (ComboBox)ControlDictionary["targetFrameworkComboBox"];
			
			if (convertProjectToMSBuild35Button != null) {
				if (project.MinimumSolutionVersion == Solution.SolutionVersionVS2005) {
					// VS05 project
					targetFrameworkComboBox.Enabled = false;
					convertProjectToMSBuild35Button.Click += OnConvertProjectToMSBuild35ButtonClick;
					return;
				} else {
					// VS08 project
					targetFrameworkComboBox.Enabled = true;
					convertProjectToMSBuild35Button.Visible = false;
				}
			}
			
			const string TargetFrameworkProperty = "TargetFrameworkVersion";
			ConfigurationGuiBinding targetFrameworkBinding;
			
			targetFrameworkBinding = helper.BindStringEnum(
				"targetFrameworkComboBox", TargetFrameworkProperty,
				"v2.0",
				(from targetFramework in Internal.Templates.TargetFramework.TargetFrameworks
				 where targetFramework.DisplayName != null
				 select new StringPair(targetFramework.Name, targetFramework.DisplayName)).ToArray());
			targetFrameworkBinding.CreateLocationButton("targetFrameworkLabel");
			helper.Saved += delegate {
				CompilableProject cProject = (CompilableProject)project;
				cProject.AddOrRemoveExtensions();
			};
		}
		
		void OnConvertProjectToMSBuild35ButtonClick(object sender, EventArgs e)
		{
			using (ConvertToMSBuild35Dialog dlg = new ConvertToMSBuild35Dialog(project.Language + " newversion")) {
				if (dlg.ShowDialog() == DialogResult.OK) {
					if (dlg.ConvertAllProjects) {
						foreach (IProject p in ProjectService.OpenSolution.Projects) {
							MSBuildBasedProject msbp = p as MSBuildBasedProject;
							if (msbp != null)
								msbp.ConvertToMSBuild35(dlg.ChangeTargetFramework);
						}
					} else {
						project.ConvertToMSBuild35(dlg.ChangeTargetFramework);
					}
					if (project.MinimumSolutionVersion == Solution.SolutionVersionVS2005)
						throw new InvalidOperationException("Project did not convert to MSBuild 3.5");
					ProjectService.SaveSolution();
					InitTargetFramework();
				}
			}
		}
	}
}
