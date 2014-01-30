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
			SD.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			TaskService.ClearExceptCommentTasks();
		}
		
		/// <summary>
		/// Adds the error to the error list and brings the error list to the front.
		/// </summary>
		public static void ShowErrorInErrorList(string fileName, Exception ex)
		{
			ClearErrorList();
			
			AddError(fileName, ex);
			SD.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
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
				SD.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
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
			TaskService.Add(new SDTask(FileName.Create(fileName), ex.Message, column, line, TaskType.Error));
		}
	}
}
