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
	/// Derived version of the ApplicationSettingsPanel class so we
	/// can access various protected methods and variables when
	/// testing the base class.
	/// </summary>
	public class DerivedApplicationSettingsPanel : ApplicationSettingsPanel
	{
		string setupFromManifestResourceName;
		bool configurationSelectorAddedToControl;
		Dictionary<string, string> boundStringControls = new Dictionary<string, string>();
		Dictionary<string, string> boundEnumControls = new Dictionary<string, string>();
		Dictionary<string, TextBoxEditMode> boundTextEditModes = new Dictionary<string, TextBoxEditMode>();
		List<string> locationButtonsCreated = new List<string>();
		Dictionary<string, BrowseButtonInfo> browseButtons = new Dictionary<string, BrowseButtonInfo>();
		
		public DerivedApplicationSettingsPanel()
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
		/// enum property.
		/// </summary>
		public string GetBoundEnumControlName(string propertyName)
		{
			if (boundEnumControls.ContainsKey(propertyName)) {
				return boundEnumControls[propertyName];
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
		/// Returns whether the configuration selector control was added
		/// to this control.
		/// </summary>
		public bool ConfigurationSelectorAddedToControl {
			get { return configurationSelectorAddedToControl; }
		}
		
		/// <summary>
		/// Gets the browse button info for the specified browse button
		/// control.
		/// </summary>
		public BrowseButtonInfo GetBrowseButtonInfo(string browseButtonName)
		{
			return browseButtons[browseButtonName];
		}

		/// <summary>
		/// Calls the AssemblyNameTextBoxTextChanged method which
		/// should update the output name text field due to changes
		/// in the assembly name.
		/// </summary>
		public void CallAssemblyNameTextBoxTextChanged()
		{
			base.AssemblyNameTextBoxTextChanged(null, new EventArgs());
		}
		
		/// <summary>
		/// Calls the OutputTypeComboBoxSelectedIndexChanged method which
		/// should update the output name text field due to changes in the
		/// output type.
		/// </summary>
		public void CallOutputTypeComboBoxSelectedIndexChanged()
		{
			base.OutputTypeComboBoxSelectedIndexChanged(null, new EventArgs());
		}
		
		/// <summary>
		/// Called in ApplicationSettingsPanel.LoadPanelContents when
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
		/// Called when binding a enum property to a control.
		/// </summary>
		protected override ConfigurationGuiBinding BindEnum<T>(string control, string property)
		{
			boundEnumControls.Add(property, control);
			return base.BindEnum<T>(control, property);
		}
		
		/// <summary>
		/// Called when a location button is associated with a control.
		/// </summary>
		protected override ChooseStorageLocationButton CreateLocationButton(ConfigurationGuiBinding binding, string controlName)
		{
			locationButtonsCreated.Add(controlName);
			return base.CreateLocationButton(binding, controlName);
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
		
		/// <summary>
		/// Connects the browse button control to the target control.
		/// </summary>
		protected override void ConnectBrowseButtonControl(string browseButton, string target, string fileFilter, TextBoxEditMode textBoxEditMode)
		{
			BrowseButtonInfo browseButtonInfo = new BrowseButtonInfo(target, fileFilter, textBoxEditMode);
			browseButtons.Add(browseButton, browseButtonInfo);
			base.ConnectBrowseButtonControl(browseButton, target, fileFilter, textBoxEditMode);
		}	
	}
}
