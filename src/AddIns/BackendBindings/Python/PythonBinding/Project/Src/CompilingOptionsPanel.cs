// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Python project's compiling options panel.
	/// </summary>
	public class CompilingOptionsPanel : AbstractBuildOptions
	{
		public override void LoadPanelContents()
		{
			SetupFromManifestResource("ICSharpCode.PythonBinding.Resources.CompilingOptionsPanel.xfrm");			
			InitializeHelper();
			
			ConfigurationGuiBinding b = BindString("outputPathTextBox", "OutputPath", TextBoxEditMode.EditRawProperty);			
			CreateLocationButton(b, "outputPathTextBox");
			ConnectBrowseFolderButtonControl("outputPathBrowseButton", "outputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}", TextBoxEditMode.EditRawProperty);
			
			b = BindBoolean("debugInfoCheckBox", "DebugInfo", false);
			CreateLocationButton(b, "debugInfoCheckBox");
		
			b = CreatePlatformTargetComboBox();
			CreateLocationButton(b, "targetCpuComboBox");
			
			AddConfigurationSelector(this);
		}
		
		/// <summary>
		/// Calls SetupFromXmlStream after creating a stream from the current
		/// assembly using the specified manifest resource name.
		/// </summary>
		/// <param name="resource">The manifest resource name used
		/// to create the stream.</param>
		protected virtual void SetupFromManifestResource(string resource)
		{
			SetupFromXmlStream(typeof(CompilingOptionsPanel).Assembly.GetManifestResourceStream(resource));
		}
		
		/// <summary>
		/// Binds the string property to a text box control.
		/// </summary>
		protected virtual ConfigurationGuiBinding BindString(string control, string property, TextBoxEditMode textBoxEditMode)
		{
			return helper.BindString(control, property, textBoxEditMode);
		}
		
		/// <summary>
		/// Binds the boolean property to a check box control.
		/// </summary>
		protected virtual ConfigurationGuiBinding BindBoolean(string control, string property, bool defaultValue)
		{
			return helper.BindBoolean(control, property, defaultValue);
		}
		
		/// <summary>
		/// Associates a location button with a control.
		/// </summary>
		protected virtual ChooseStorageLocationButton CreateLocationButton(ConfigurationGuiBinding binding, string controlName)
		{
			return binding.CreateLocationButton(controlName);
		}		
		
		/// <summary>
		/// Connects the browse folder button control to the target control.
		/// </summary>
		protected virtual void ConnectBrowseFolderButtonControl(string browseButton, string target, string description, TextBoxEditMode textBoxEditMode)
		{
			ConnectBrowseFolder(browseButton, target, description, textBoxEditMode);
		}
		
		/// <summary>
		/// Adds a configuration selector to the specified control.
		/// </summary>
		protected virtual void AddConfigurationSelector(Control control)
		{
			helper.AddConfigurationSelector(control);
		}
		
		/// <summary>
		/// Creates the platform target combo box.
		/// </summary>
		protected virtual ConfigurationGuiBinding CreatePlatformTargetComboBox()
		{
			return base.CreatePlatformTarget();
		}
	}
}
