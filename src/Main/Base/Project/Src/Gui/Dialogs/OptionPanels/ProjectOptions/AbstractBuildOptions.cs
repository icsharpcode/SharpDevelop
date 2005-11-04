// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

using StringPair = System.Collections.Generic.KeyValuePair<string, string>;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class AbstractBuildOptions : AbstractProjectOptionPanel
	{
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
					Get<TextBox>("xmlDocumentation").Text = FileUtility.GetRelativePath(baseDirectory, project.OutputAssemblyFullPath) + ".xml";
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
					PropertyStorageLocations tmp;
					if (this.Helper.GetProperty("WarningsAsErrors", "", out tmp).Length > 0) {
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
		
		protected void InitAdvanced()
		{
			debugInfoBinding = helper.BindEnum<DebugSymbolType>("debugInfoComboBox", "DebugType");
			debugInfoBinding.CreateLocationButton("debugInfoLabel");
			
			ConfigurationGuiBinding b;
			b = helper.BindBoolean("registerCOMInteropCheckBox", "RegisterForComInterop", false);
			b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
			advancedLocationButton = b.CreateLocationButtonInPanel("advancedOutputGroupBox");
			
			b = helper.BindStringEnum("generateSerializationAssemblyComboBox", "GenerateSerializationAssemblies",
			                          "Auto",
			                          new StringPair("Off", "Off"),
			                          new StringPair("On", "On"),
			                          new StringPair("Auto", "Auto"));
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
			
			helper.Saved += delegate {
				if ((DebugSymbolType)Get<ComboBox>("debugInfo").SelectedIndex == DebugSymbolType.Full) {
					helper.SetProperty("DebugSymbols", "true", debugInfoBinding.Location);
				} else {
					helper.SetProperty("DebugSymbols", "false", debugInfoBinding.Location);
				}
			};
		}
		
		protected void InitTargetFramework(string defaultTargets, string extendedTargets)
		{
			const string TargetFrameworkProperty = "TargetFrameworkVersion";
			debugInfoBinding = helper.BindStringEnum("targetFrameworkComboBox", TargetFrameworkProperty,
			                                         "",
			                                         new StringPair("", "Default (.NET 2.0)"),
			                                         new StringPair("v1.0", ".NET 1.0"),
			                                         new StringPair("v1.1", ".NET 1.1"),
			                                         new StringPair("v2.0", ".NET 2.0"),
			                                         new StringPair("Mono v1.1", "Mono 1.1"),
			                                         new StringPair("Mono v2.0", "Mono 2.0"));
			debugInfoBinding.CreateLocationButton("targetFrameworkLabel");
			helper.Saved += delegate {
				// Test if SharpDevelop-Build extensions are needed
				MSBuildProject project = helper.Project;
				bool needExtensions = false;
				PropertyStorageLocations location;
				foreach (string configuration in project.GetConfigurationNames()) {
					foreach (string platform in project.GetPlatformNames()) {
						string value = project.GetProperty(configuration, platform, TargetFrameworkProperty, "", out location);
						if (value.Length > 0) {
							needExtensions = true;
						}
					}
				}
				for (int i = 0; i < project.Imports.Count; i++) {
					if (needExtensions) {
						if (defaultTargets.Equals(project.Imports[i], StringComparison.InvariantCultureIgnoreCase))
							project.Imports[i] = extendedTargets;
					} else {
						if (extendedTargets.Equals(project.Imports[i], StringComparison.InvariantCultureIgnoreCase))
							project.Imports[i] = defaultTargets;
					}
				}
			};
		}
	}
}
