// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;


namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs
{
	public class ExportProjectDialog : BaseSharpDevelopForm, IProgressMonitor
	{
		ArrayList outputConvertes;
		
		public ExportProjectDialog() : this(null, null)
		{
			
		}
		
		public ExportProjectDialog(string formatName) : this(formatName, null)
		{
			
		}
		
		public ExportProjectDialog(string formatName, string projectName)
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("ExportProjectDialog.xfrm"));
			Icon = null;
			ControlDictionary["outputLocationBrowseButton"].Click += new EventHandler(BrowseOutputLocation);
			ControlDictionary["startButton"].Click += new EventHandler(StartConversion);
			
			outputConvertes = RetrieveOutputConverters();
			FillOutputFormat(formatName);
			FillProjectList(projectName);
			((RadioButton)ControlDictionary["singleProjectRadioButton"]).CheckedChanged += new EventHandler(RadioButtonChecked);
			((RadioButton)ControlDictionary["wholeCombineRadioButton"]).CheckedChanged += new EventHandler(RadioButtonChecked);
			
			RadioButtonChecked(null, null);
			ControlDictionary["outputLocationTextBox"].Text = PropertyService.Get("ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs.ExportProjectDialog.OutputPath", "C:\\output");
		}
		
		void FillOutputFormat(string formatName)
		{
			int index = 0; 
			for (int i = 0; i < outputConvertes.Count; ++i) {
				AbstractOutputConverter outputConverter = (AbstractOutputConverter)outputConvertes[i];
				((ComboBox)ControlDictionary["outputFormatComboBox"]).Items.Add(StringParser.Parse(outputConverter.FormatName));
				if (formatName == outputConverter.FormatName) {
					index = i;
				}
			}
			((ComboBox)ControlDictionary["outputFormatComboBox"]).SelectedIndex = index;
		}
		
		void FillProjectList(string projectName)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(IProjectService));
			if (projectService.CurrentOpenCombine == null) {
				return;
			}
			
			ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
			int index = 0; 
			for (int i = 0; i < allProjects.Count; ++i) {
				ProjectCombineEntry entry = (ProjectCombineEntry)allProjects[i];
				((ComboBox)ControlDictionary["projectListComboBox"]).Items.Add(entry.Project.Name);
				if (entry.Project.Name == projectName) {
					index = i;
					((RadioButton)ControlDictionary["singleProjectRadioButton"]).Checked = true;
				}
			}
			if (allProjects.Count > 0) {
				((ComboBox)ControlDictionary["projectListComboBox"]).SelectedIndex = index;
			}
		}
		
		void RadioButtonChecked(object sender, EventArgs e)
		{
			SetEnabledStatus(((RadioButton)ControlDictionary["singleProjectRadioButton"]).Checked, "projectListComboBox");
		}
		
		
		ArrayList RetrieveOutputConverters()
		{
			ArrayList converters = new ArrayList();
			Assembly asm = Assembly.GetCallingAssembly();
			foreach (Type t in asm.GetTypes()) {
				if (!t.IsAbstract && t.IsSubclassOf(typeof(AbstractOutputConverter))) {
					converters.Add(asm.CreateInstance(t.FullName));
				}
			}
			return converters;
		}
		void BrowseOutputLocation(object sender, EventArgs e)
		{
			FolderDialog fd = new FolderDialog();
			if (fd.DisplayDialog("Choose combine output location.") == DialogResult.OK) {
				ControlDictionary["outputLocationTextBox"].Text = fd.Path;
			}
		}
		
		void StartConversion(object sender, EventArgs e)
		{
			string outputPath = ControlDictionary["outputLocationTextBox"].Text;
			
			if (!FileUtilityService.IsValidFileName(outputPath)) {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs.ImportProjectDialog.OutputFileInvalidError}");
				return;
			}
			
			if (!FileUtilityService.IsDirectory(outputPath)) {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs.ImportProjectDialog.OutputPathDoesntExistError}");
				return;
			}
			IProjectService projectService = (IProjectService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(IProjectService));
			AbstractOutputConverter outputConverter = (AbstractOutputConverter)outputConvertes[((ComboBox)ControlDictionary["outputFormatComboBox"]).SelectedIndex];
			if (((RadioButton)ControlDictionary["singleProjectRadioButton"]).Checked) {
				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				IProject  project = ((ProjectCombineEntry)allProjects[((ComboBox)ControlDictionary["projectListComboBox"]).SelectedIndex]).Project;
				outputConverter.ConvertProject(this, projectService.GetFileName(project), outputPath);
			} else {
				outputConverter.ConvertCombine(this, projectService.GetFileName(projectService.CurrentOpenCombine), outputPath);
			}
			
			MessageService.ShowMessage("${res:ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs.ExportProjectDialog.ConversionDoneMessage}");
			
			PropertyService.Set("ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs.ExportProjectDialog.OutputPath", ControlDictionary["outputLocationTextBox"].Text);
		}
		
		#region IProgressMonitor interface
		int curWork = 0;
		public void BeginTask(string name, int totalWork)
		{
			((ProgressBar)ControlDictionary["progressBar"]).Minimum = 0;
			((ProgressBar)ControlDictionary["progressBar"]).Maximum = totalWork;
			((ProgressBar)ControlDictionary["progressBar"]).Value   = curWork = 0;
		}
		public void Worked(int work)
		{
			curWork += work;
			((ProgressBar)ControlDictionary["progressBar"]).Value = curWork;
		}
		
		public void Done()
		{
			((ProgressBar)ControlDictionary["progressBar"]).Value = ((ProgressBar)ControlDictionary["progressBar"]).Maximum;
		}
		
		public bool Canceled {
			get {
				return false;
			}
			set {
				
			}
		}
		
		public string TaskName {
			get {
				return "Export";
			}
			set {
				
			}
		}
		#endregion
	}
}
