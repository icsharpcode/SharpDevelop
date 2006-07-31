// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public sealed class WixBindingService
	{
		WixBindingService()
		{
		}
		
		/// <summary>
		/// Gets the Wix document text either from the open text editor or from the 
		/// file system.
		/// </summary>
		public static TextReader GetWixDocumentText(string fileName)
		{
			// Get the text from the document if it is currently being viewed.
			IWorkbenchWindow openWindow = FileService.GetOpenFile(fileName);
			if (openWindow != null) {
				ITextEditorControlProvider textEditorProvider = openWindow.ViewContent as ITextEditorControlProvider;
				if (textEditorProvider != null) {
					return new StringReader(textEditorProvider.TextEditorControl.Text);
				}
			}
			
			// Load the text from the file.
			FileStream s = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			return new StreamReader(s, true);
		}
		
		/// <summary>
		/// Clears the error list and the output messages.
		/// </summary>
		public static void ClearErrorList()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			TaskService.ClearExceptCommentTasks();
		}
		
		/// <summary>
		/// Adds the error to the error list and brings the error list to the front.
		/// </summary>
		public static void ShowErrorInErrorList(string fileName, Exception ex)
		{
			ClearErrorList();
			
			AddError(fileName, ex);
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
		
		/// <summary>
		/// Adds an error to the error list.
		/// </summary>
		public static void AddErrorToErrorList(string fileName, Exception ex)
		{			
			AddError(fileName, ex);
		}
		
		/// <summary>
		/// Shows the error list if there are any errors.
		/// </summary>
		public static void ShowErrorList()
		{
			if (TaskService.SomethingWentWrong) {
				WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
		}
		
		/// <summary>
		/// Adds error to error list window.
		/// </summary>
		static void AddError(string fileName, Exception ex)
		{
			int column = 0;
			int line = 0;
			
			XmlException xmlEx = ex as XmlException;
			if (xmlEx != null) {
				column = xmlEx.LinePosition - 1;
				line = xmlEx.LineNumber - 1;
			}
			LoggingService.Debug(ex.ToString());
			TaskService.Add(new Task(fileName, ex.Message, column, line, TaskType.Error));
		}
	}
}
