// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Python project's application settings panel.
	/// </summary>
	public class ApplicationSettingsPanel : AbstractXmlFormsProjectOptionPanel
	{
		const string AssemblyTextBoxName = "assemblyNameTextBox";
		const string RootNamespaceTextBoxName = "rootNamespaceTextBox";
		const string OutputTypeComboBoxName = "outputTypeComboBox";
		const string MainFileComboBoxName = "mainFileComboBox";
		
		public override void LoadPanelContents()
		{
			SetupFromManifestResource("ICSharpCode.PythonBinding.Resources.ApplicationSettingsPanel.xfrm");			
			InitializeHelper();
			
			ConfigurationGuiBinding b = BindString(AssemblyTextBoxName, "AssemblyName", TextBoxEditMode.EditEvaluatedProperty);
			CreateLocationButton(b, AssemblyTextBoxName);
			AssemblyNameTextBox.TextChanged += AssemblyNameTextBoxTextChanged;
			
			b = BindString(RootNamespaceTextBoxName, "RootNamespace", TextBoxEditMode.EditEvaluatedProperty);
			CreateLocationButton(b, RootNamespaceTextBoxName);
			
			b = BindEnum<OutputType>(OutputTypeComboBoxName, "OutputType");
			CreateLocationButton(b, OutputTypeComboBoxName);
			OutputTypeComboBox.SelectedIndexChanged += OutputTypeComboBoxSelectedIndexChanged;

			b = BindString(MainFileComboBoxName, "MainFile", TextBoxEditMode.EditEvaluatedProperty);
			CreateLocationButton(b, MainFileComboBoxName);
			ConnectBrowseButtonControl("mainFileBrowseButton", "mainFileComboBox",
				"${res:SharpDevelop.FileFilter.AllFiles}|*.*",
				TextBoxEditMode.EditEvaluatedProperty);
		
			Get<TextBox>("projectFolder").Text = project.Directory;
			Get<TextBox>("projectFile").Text = Path.GetFileName(project.FileName);
			Get<TextBox>("projectFile").ReadOnly = true;
			
			RefreshOutputNameTextBox();
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
			SetupFromXmlStream(typeof(ApplicationSettingsPanel).Assembly.GetManifestResourceStream(resource));
		}
			
		/// <summary>
		/// Binds the string property to a text box control.
		/// </summary>
		protected virtual ConfigurationGuiBinding BindString(string control, string property, TextBoxEditMode textBoxEditMode)
		{
			return helper.BindString(control, property, textBoxEditMode);
		}
		
		/// <summary>
		/// Binds an enum property to a control.
		/// </summary>
		protected virtual ConfigurationGuiBinding BindEnum<T>(string control, string property) where T : struct
		{
			return helper.BindEnum<T>(control, property);
		}
		
		/// <summary>
		/// Associates a location button with a control.
		/// </summary>
		protected virtual ChooseStorageLocationButton CreateLocationButton(ConfigurationGuiBinding binding, string controlName)
		{
			return binding.CreateLocationButton(controlName);
		}
		
		/// <summary>
		/// Adds a configuration selector to the specified control.
		/// </summary>
		protected virtual void AddConfigurationSelector(Control control)
		{
			helper.AddConfigurationSelector(control);
		}
		
		/// <summary>
		/// Connects the browse button control to the target control.
		/// </summary>
		protected virtual void ConnectBrowseButtonControl(string browseButton, string target, string fileFilter, TextBoxEditMode textBoxEditMode)
		{
			ConnectBrowseButton(browseButton, target, fileFilter, textBoxEditMode);
		}
		
		/// <summary>
		/// Refreshes the output name text box after the assembly name
		/// has changed.
		/// </summary>
		protected void AssemblyNameTextBoxTextChanged(object source, EventArgs e)
		{
			RefreshOutputNameTextBox();
		}
		
		/// <summary>
		/// Refreshes the output name text box after the output type has changed.
		/// </summary>
		protected void OutputTypeComboBoxSelectedIndexChanged(object source, EventArgs e)
		{
			RefreshOutputNameTextBox();
		}

		/// <summary>
		/// Updates the output name text box based on the assembly name and
		/// output type.
		/// </summary>
		void RefreshOutputNameTextBox()
		{
			string assemblyName = AssemblyNameTextBox.Text;
			string extension = CompilableProject.GetExtension((OutputType)OutputTypeComboBox.SelectedIndex);
			Get<TextBox>("outputName").Text = String.Concat(assemblyName, extension);
		}
		
		TextBox AssemblyNameTextBox {
			get { return Get<TextBox>("assemblyName"); }
		}
		
		ComboBox OutputTypeComboBox {
			get { return Get<ComboBox>("outputType"); }
		}
	}
}
