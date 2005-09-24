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
			helper.BindString("xmlDocumentationTextBox", "DocumentationFile");
			Get<CheckBox>("xmlDocumentation").Checked = Get<TextBox>("xmlDocumentation").Text.Length > 0;
			Get<CheckBox>("xmlDocumentation").CheckedChanged  += new EventHandler(UpdateXmlEnabled);
			Get<TextBox>("xmlDocumentation").Enabled = Get<CheckBox>("xmlDocumentation").Checked;
		}
		
		void UpdateXmlEnabled(object sender, EventArgs e)
		{
			Get<TextBox>("xmlDocumentation").Enabled = Get<CheckBox>("xmlDocumentation").Checked;
			if (Get<CheckBox>("xmlDocumentation").Checked && Get<TextBox>("xmlDocumentation").Text.Length == 0) {
				Get<TextBox>("xmlDocumentation").Text = FileUtility.GetRelativePath(baseDirectory, project.OutputAssemblyFullPath) + ".xml";
			} else {
				Get<TextBox>("xmlDocumentation").Text = "";
			}
		}
		
		protected void InitWarnings()
		{
			helper.BindStringEnum("warningLevelComboBox", "WarningLevel",
			                      "4",
			                      new StringPair("0", "0"),
			                      new StringPair("1", "1"),
			                      new StringPair("2", "2"),
			                      new StringPair("3", "3"),
			                      new StringPair("4", "4"));
			helper.BindString("suppressWarningsTextBox", "NoWarn");
			
			helper.AddBinding("TreatWarningsAsErrors", new WarningsAsErrorsBinding(this));
			helper.BindString("specificWarningsTextBox", "WarningsAsErrors"); // must be saved AFTER TreatWarningsAsErrors
			
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
		
		protected void InitAdvanced()
		{
			debugInfoBinding = helper.BindEnum<DebugSymbolType>("debugInfoComboBox", "DebugType");
			helper.BindBoolean("registerCOMInteropCheckBox", "RegisterForComInterop", false);
			helper.BindStringEnum("generateSerializationAssemblyComboBox", "GenerateSerializationAssemblies",
			                      "Auto",
			                      new StringPair("Off", "Off"),
			                      new StringPair("On", "On"),
			                      new StringPair("Auto", "Auto"));
			helper.BindHexadecimal(Get<TextBox>("dllBaseAddress"), "BaseAddress", 0x400000);
			helper.BindStringEnum("targetCpuComboBox", "PlatformTarget",
			                      "AnyCPU",
			                      new StringPair("AnyCPU", "${res:Dialog.ProjectOptions.Build.TargetCPU.Any}"),
			                      new StringPair("x86", "${res:Dialog.ProjectOptions.Build.TargetCPU.x86}"),
			                      new StringPair("x64", "${res:Dialog.ProjectOptions.Build.TargetCPU.x64}"),
			                      new StringPair("Itanium", "${res:Dialog.ProjectOptions.Build.TargetCPU.Itanium}"));
		}
		
		public override bool StorePanelContents()
		{
			if (base.StorePanelContents()) {
				if ((DebugSymbolType)Get<ComboBox>("debugInfo").SelectedIndex == DebugSymbolType.Full) {
					helper.SetProperty("DebugSymbols", "true", debugInfoBinding.Location);
				} else {
					helper.SetProperty("DebugSymbols", "false", debugInfoBinding.Location);
				}
				return true;
			} else {
				return false;
			}
		}
	}
}
