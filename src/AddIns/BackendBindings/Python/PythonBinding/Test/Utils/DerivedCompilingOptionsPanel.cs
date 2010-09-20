// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Project;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Derived version of the CompilingOptionsPanel class so we
	/// can access various protected methods and variables when
	/// testing the base class.
	/// </summary>
	public class DerivedCompilingOptionsPanel : CompilingOptionsPanel
	{
		string setupFromManifestResourceName;
		bool configurationSelectorAddedToControl;
		Dictionary<string, string> boundStringControls = new Dictionary<string, string>();
		Dictionary<string, string> boundBooleanControls = new Dictionary<string, string>();
		Dictionary<string, TextBoxEditMode> boundTextEditModes = new Dictionary<string, TextBoxEditMode>();
		List<string> locationButtonsCreated = new List<string>();
		Dictionary<string, BrowseFolderButtonInfo> browseFolderButtons = new Dictionary<string, BrowseFolderButtonInfo>();
		bool createdTargetCpuComboBox;
		
		public DerivedCompilingOptionsPanel()
		{
		}
	
		public ConfigurationGuiHelper Helper { 
			get { return helper; }
		}
		
		/// <summary>
		/// Returns the resource name used to create the stream when
		/// initialising the XmlUserControl.
		/// </summary>
		public string SetupFromManifestResourceName {
			get { return setupFromManifestResourceName; }
		}
		
		/// <summary>
		/// Returns whether the target cpu combo box was created.
		/// </summary>
		public bool IsTargetCpuComboBoxCreated {
			get { return createdTargetCpuComboBox; }
		}
		
		/// <summary>
		/// Gets the name of the control that was bound to the specified
		/// property.
		/// </summary>
		public string GetBoundStringControlName(string propertyName)
		{
			if (boundStringControls.ContainsKey(propertyName)) {
				return boundStringControls[propertyName];
			}
			return null;
		}
		
		/// <summary>
		/// Gets the name of the control that was bound to the specified
		/// property.
		/// </summary>
		public string GetBoundBooleanControlName(string propertyName)
		{
			if (boundBooleanControls.ContainsKey(propertyName)) {
				return boundBooleanControls[propertyName];
			}
			return null;
		}
		
		/// <summary>
		/// Gets the TextBoxEditMode used the control bound to the 
		/// specified property.
		/// </summary>
		public TextBoxEditMode GetBoundControlTextBoxEditMode(string propertyName)
		{
			return boundTextEditModes[propertyName];
		}
		
		/// <summary>
		/// Returns whether the specified control has an associated
		/// location button.
		/// </summary>
		public bool IsLocationButtonCreated(string controlName)
		{
			return locationButtonsCreated.Contains(controlName);
		}		
		
		/// <summary>
		/// Gets the browse button info for the specified browse button
		/// control.
		/// </summary>
		public BrowseFolderButtonInfo GetBrowseFolderButtonInfo(string browseButtonName)
		{
			return browseFolderButtons[browseButtonName];
		}
		
		/// <summary>
		/// Returns whether the configuration selector control was added
		/// to this control.
		/// </summary>
		public bool ConfigurationSelectorAddedToControl {
			get { return configurationSelectorAddedToControl; }
		}		
		
		/// <summary>
		/// Called in CompilingOptionsPanel.LoadPanelContents when
		/// initialising the XmlUserControl.
		/// </summary>
		protected override void SetupFromManifestResource(string resource)
		{
			setupFromManifestResourceName = resource;
			base.SetupFromManifestResource(resource);
		}		
		
		/// <summary>
		/// Called when binding a string property to a control.
		/// </summary>
		protected override ConfigurationGuiBinding BindString(string control, string property, TextBoxEditMode textBoxEditMode)
		{
			boundStringControls.Add(property, control);
			boundTextEditModes.Add(property, textBoxEditMode);
			return base.BindString(control, property, textBoxEditMode);
		}
		
		/// <summary>
		/// Called when binding a boolean property to a control.
		/// </summary>
		protected override ConfigurationGuiBinding BindBoolean(string control, string property, bool defaultValue)
		{
			boundBooleanControls.Add(property, control);
			return base.BindBoolean(control, property, defaultValue);
		}
				
		/// <summary>
		/// Called when associating a location button with a property.
		/// </summary>
		protected override ChooseStorageLocationButton CreateLocationButton(ConfigurationGuiBinding binding, string controlName)
		{
			locationButtonsCreated.Add(controlName);
			return base.CreateLocationButton(binding, controlName);
		}
		
		protected override ConfigurationGuiBinding CreatePlatformTargetComboBox()
		{
			createdTargetCpuComboBox = true;
			return base.CreatePlatformTargetComboBox();
		}
		
		/// <summary>
		/// Connects the browse folder button control to the target control.
		/// </summary>
		protected override void ConnectBrowseFolderButtonControl(string browseButton, string target, string description, TextBoxEditMode textBoxEditMode)
		{
			BrowseFolderButtonInfo browseButtonInfo = new BrowseFolderButtonInfo(target, description, textBoxEditMode);
			browseFolderButtons.Add(browseButton, browseButtonInfo);
			base.ConnectBrowseFolderButtonControl(browseButton, target, description, textBoxEditMode);
		}	
		
		/// <summary>
		/// If the control is this control then we flag that the
		/// configuration selector has been added.
		/// </summary>
		protected override void AddConfigurationSelector(Control control)
		{
			if (control == this) {
				configurationSelectorAddedToControl = true;
			}
			base.AddConfigurationSelector(control);
		}	
	}
}
