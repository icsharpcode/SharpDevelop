// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	public sealed class WixBindingService
	{
		WixBindingService()
		{
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
				column = xmlEx.LinePosition;
				line = xmlEx.LineNumber;
			}
			LoggingService.Debug(ex.ToString());
			TaskService.Add(new Task(FileName.Create(fileName), ex.Message, column, line, TaskType.Error));
		}
	}
}
