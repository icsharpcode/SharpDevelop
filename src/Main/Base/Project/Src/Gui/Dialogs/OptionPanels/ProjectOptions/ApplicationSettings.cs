// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class ApplicationSettings : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.ApplicationSettings.xfrm");
			
			InitializeHelper();
			
			ConnectBrowseButton("applicationIconBrowseButton", "applicationIconComboBox", "${res:SharpDevelop.FileFilter.Icons}|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			// TODO: Suitable file filter.
			ConnectBrowseButton("win32ResourceFileBrowseButton", "win32ResourceFileComboBox");
			
			ConfigurationGuiBinding b;
			ChooseStorageLocationButton locationButton;
			b = helper.BindString("assemblyNameTextBox", "AssemblyName");
			b.CreateLocationButton("assemblyNameTextBox");
			Get<TextBox>("assemblyName").TextChanged += new EventHandler(RefreshOutputNameTextBox);
			
			b = helper.BindString("rootNamespaceTextBox", "RootNamespace");
			b.CreateLocationButton("rootNamespaceTextBox");
			
			b = helper.BindEnum<OutputType>("outputTypeComboBox", "OutputType");
			locationButton = b.CreateLocationButton("outputTypeComboBox");
			Get<ComboBox>("outputType").SelectedIndexChanged += new EventHandler(RefreshOutputNameTextBox);
			
			b = helper.BindString("startupObjectComboBox", "StartupObject");
			b.RegisterLocationButton(locationButton);
			
			b = helper.BindString("applicationIconComboBox", "ApplicationIcon");
			Get<ComboBox>("applicationIcon").TextChanged += new EventHandler(ApplicationIconComboBoxTextChanged);
			b.CreateLocationButton("applicationIconComboBox");
			
			b = helper.BindString("win32ResourceFileComboBox", "Win32Resource");
			b.CreateLocationButton("win32ResourceFileComboBox");
			
			Get<TextBox>("projectFolder").Text = project.Directory;
			Get<TextBox>("projectFile").Text = Path.GetFileName(project.FileName);
			
			// maybe make this writable again? Needs special care when saving!
			Get<TextBox>("projectFile").ReadOnly = true;
			
			RefreshOutputNameTextBox(null, EventArgs.Empty);
			
			helper.AddConfigurationSelector(this);
		}
		
		void RefreshOutputNameTextBox(object sender, EventArgs e)
		{
			Get<TextBox>("outputName").Text = Get<TextBox>("assemblyName").Text + MSBuildProject.GetExtension((OutputType)Get<ComboBox>("outputType").SelectedIndex);
		}
		
		void ApplicationIconComboBoxTextChanged(object sender, EventArgs e)
		{
			string applicationIcon = Path.Combine(baseDirectory, Get<ComboBox>("applicationIcon").Text);
			if (File.Exists(applicationIcon)) {
				Get<PictureBox>("applicationIcon").Image = Image.FromFile(applicationIcon);
			} else {
				Get<PictureBox>("applicationIcon").Image = null;
			}
		}
	}
}
