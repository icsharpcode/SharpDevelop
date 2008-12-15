/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 21.09.2008
 * Time: 09:19
 */
using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	/// <summary>
	/// Description of CreateTemplateFromFileCommand
	/// </summary>
	public class CreateTemplateFromFile : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			string language = ProjectService.CurrentProject.Language;
			string fileName = MessageService.ShowInputBox("Create new template", "Enter name for new template:", "NewFileTemplate");
			string ext = Path.GetExtension(WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName);
			string content = new StreamReader(WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFile.OpenRead()).ReadToEnd();
			FileService.OpenFile(TemplateCreator.CreateFileTemplate(language, fileName, ext, content));
			FileTemplate.UpdateTemplates();
		}
	}
	
	/// <summary>
	/// Description of CreateTemplateFromFileCommand
	/// </summary>
	public class CreateTemplateFromProject : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			MessageService.ShowMessage("All opened files need to be saved before creating a project template!");
			foreach (OpenedFile file in FileService.OpenedFiles) {
				file.SaveToDisk();
			}
			string fileName = MessageService.ShowInputBox("Create new template", "Enter name for new template:", "NewProjectTemplate");
			FileService.OpenFile(TemplateCreator.CreateProjectTemplate(fileName, ProjectService.OpenSolution));
			ProjectTemplate.UpdateTemplates();
		}
	}
}
