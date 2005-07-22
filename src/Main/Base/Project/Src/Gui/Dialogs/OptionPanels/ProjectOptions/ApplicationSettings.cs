// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
	
namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class ApplicationSettings : AbstractOptionPanel
	{
		AdvancedMSBuildProject project;
		
		public ApplicationSettings()
		{
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ProjectOptions.ApplicationSettings.xfrm"));
			this.project = (AdvancedMSBuildProject)((Properties)CustomizationObject).Get("Project");
			
			ConnectBrowseButton("applicationIconBrowseButton", "applicationIconComboBox", "${res:SharpDevelop.FileFilter.Icons}|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			// TODO: Suitable file filter.
			ConnectBrowseButton("win32ResourceFileBrowseButton", "win32ResourceFileComboBox");
			
			Get<TextBox>("assemblyName").Text = project.AssemblyName;
			Get<TextBox>("assemblyName").TextChanged += new EventHandler(RefreshOutputNameTextBox);
			Get<TextBox>("assemblyName").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("rootNamespace").Text = project.RootNamespace;
			Get<TextBox>("rootNamespace").TextChanged += new EventHandler(Save);
			
			Get<ComboBox>("outputType").Items.Add(StringParser.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe}"));
			Get<ComboBox>("outputType").Items.Add(StringParser.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe}"));
			Get<ComboBox>("outputType").Items.Add(StringParser.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Library}"));
			Get<ComboBox>("outputType").Items.Add(StringParser.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}"));
			
			Get<ComboBox>("outputType").SelectedIndex = (int)project.OutputType;
			Get<ComboBox>("outputType").SelectedIndexChanged += new EventHandler(RefreshOutputNameTextBox);
			Get<ComboBox>("outputType").SelectedIndexChanged += new EventHandler(Save);
			
			Get<ComboBox>("startupObject").Text   = project.StartupObject;
			Get<ComboBox>("startupObject").SelectedIndexChanged += new EventHandler(Save);
			
			Get<ComboBox>("applicationIcon").Text = project.ApplicationIcon;
			Get<ComboBox>("applicationIcon").TextChanged += new EventHandler(ApplicationIconComboBoxTextChanged);
			Get<ComboBox>("applicationIcon").TextChanged += new EventHandler(Save);
			
			Get<ComboBox>("win32ResourceFile").Text = project.Win32Resource;
			Get<ComboBox>("win32ResourceFile").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("projectFolder").Text = project.Directory;
			Get<TextBox>("projectFile").Text = Path.GetFileName(project.FileName);
			Get<TextBox>("projectFile").TextChanged += new EventHandler(Save);
			
			RefreshOutputNameTextBox(null, EventArgs.Empty);
		}
		
		void Save(object sender, EventArgs e) 
		{
			StorePanelContents();
		}
		
		public override bool StorePanelContents()
		{
			project.AssemblyName      = Get<TextBox>("assemblyName").Text;
			project.RootNamespace     = Get<TextBox>("rootNamespace").Text;
			project.OutputType        = (OutputType)Get<ComboBox>("outputType").SelectedIndex;
			project.StartupObject     = Get<ComboBox>("startupObject").Text;
			project.ApplicationIcon   = Get<ComboBox>("applicationIcon").Text;
			project.Win32Resource     = Get<ComboBox>("win32ResourceFile").Text;
			project.Name              = Path.GetFileNameWithoutExtension(Get<TextBox>("projectFile").Text);
			project.Save();
			return true;
		}
		
		void RefreshOutputNameTextBox(object sender, EventArgs e)
		{
			Get<TextBox>("outputName").Text = Get<TextBox>("assemblyName").Text + MSBuildProject.GetExtension((OutputType)Get<ComboBox>("outputType").SelectedIndex);
		}
		
		void ApplicationIconComboBoxTextChanged(object sender, EventArgs e)
		{
			string applicationIcon = Get<ComboBox>("applicationIcon").Text;
			if (File.Exists(applicationIcon)) {
			    Get<PictureBox>("applicationIcon").Image = Image.FromFile(applicationIcon);
			}
		}
	}
}
